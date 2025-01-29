using Hackjaggo.NetproxyUI;
using NetTools;
using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;

namespace Hackjaggo.Proxy
{
    public class TcpProxy : IProxy
    {
        private readonly ConcurrentDictionary<IPAddress, bool> _checkedIpCache = new ConcurrentDictionary<IPAddress, bool>();

        internal class TcpConnection
        {
            public readonly TcpClient _localServerConnection;
            public readonly EndPoint? _sourceEndpoint;
            public readonly IPEndPoint _remoteEndpoint;
            public readonly TcpClient _forwardClient;
            public readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
            public readonly EndPoint? _serverLocalEndpoint;
            public EndPoint? _forwardLocalEndpoint;
            public long _totalBytesForwarded;
            public long _totalBytesResponded;
            public long LastActivity { get; private set; } = Environment.TickCount64;

            public static async Task<TcpConnection> AcceptTcpClientAsync(HackjaggoProxyForm? form, TcpListener tcpListener, IPEndPoint remoteEndpoint)
            {
                var localServerConnection = await tcpListener.AcceptTcpClientAsync(form!.CancellationToken.Token).ConfigureAwait(false);
                localServerConnection.NoDelay = true;
                return new TcpConnection(localServerConnection, remoteEndpoint);
            }

            private TcpConnection(TcpClient localServerConnection, IPEndPoint remoteEndpoint)
            {
                _localServerConnection = localServerConnection;
                _remoteEndpoint = remoteEndpoint;

                _forwardClient = new TcpClient { NoDelay = true };

                _sourceEndpoint = _localServerConnection.Client.RemoteEndPoint;
                _serverLocalEndpoint = _localServerConnection.Client.LocalEndPoint;
            }

            public void Run(HackjaggoProxyForm form)
            {
                RunInternal(_cancellationTokenSource.Token, form);
            }

            public void Stop()
            {
                try
                {
                    _cancellationTokenSource.Cancel();
                }
                catch (Exception)
                {
                    //Console.WriteLine($"An exception occurred while closing TcpConnection : {ex}");
                }
            }

            private void RunInternal(CancellationToken cancellationToken, HackjaggoProxyForm form)
            {

                Task.Run(async () =>
                {
                    using (_localServerConnection)
                    using (_forwardClient)
                    {
                        try
                        {
                            {
                                var remoteEndPoint = _localServerConnection.Client.RemoteEndPoint as IPEndPoint;
                                form.AddCurrentConnectionsListView(remoteEndPoint?.Address.ToString() ?? "", remoteEndPoint?.Port ?? 0);

                                await _forwardClient.ConnectAsync(_remoteEndpoint.Address, _remoteEndpoint.Port, cancellationToken).ConfigureAwait(false);
                                _forwardLocalEndpoint = _forwardClient.Client.LocalEndPoint;

                                //Console.WriteLine($"Established TCP {_sourceEndpoint} => {_serverLocalEndpoint} => {_forwardLocalEndpoint} => {_remoteEndpoint}");

                                using (var serverStream = _forwardClient.GetStream())
                                using (var clientStream = _localServerConnection.GetStream())
                                    await Task.WhenAny(
                                        CopyToAsync(clientStream, serverStream, 81920, Direction.Forward, cancellationToken),
                                        CopyToAsync(serverStream, clientStream, 81920, Direction.Responding, cancellationToken)
                                    ).ConfigureAwait(false);
                            }
                        }
                        catch (Exception)
                        {
                            //Console.WriteLine($"An exception occurred during TCP stream : {ex}");
                            using (_localServerConnection)
                            {
                                var remoteEndPoint = _localServerConnection.Client.RemoteEndPoint as IPEndPoint;
                                form.RemoveCurrentConnectionFromListView(remoteEndPoint?.Address.ToString() ?? "", remoteEndPoint?.Port ?? 0);
                            }
                        }
                        finally
                        {
                            //Console.WriteLine($"Closed TCP {_sourceEndpoint} =>" +
                            //    $" {_serverLocalEndpoint} => " +
                            //    $"{_forwardLocalEndpoint} => " +
                            //    $"{_remoteEndpoint}. {_totalBytesForwarded} bytes forwarded, " +
                            //    $"{_totalBytesResponded} bytes responded.");

                            using (_localServerConnection)
                            {
                                var remoteEndPoint = _localServerConnection.Client.RemoteEndPoint as IPEndPoint;
                                form.RemoveCurrentConnectionFromListView(remoteEndPoint?.Address.ToString() ?? "", remoteEndPoint?.Port ?? 0);
                            }
                        }
                    }
                });
            }

            private async Task CopyToAsync(Stream source, Stream destination, int bufferSize = 81920, Direction direction = Direction.Unknown, CancellationToken cancellationToken = default)
            {
                byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        int bytesRead = await source.ReadAsync(new Memory<byte>(buffer), cancellationToken).ConfigureAwait(false);
                        if (bytesRead == 0) break;
                        LastActivity = Environment.TickCount64;
                        await destination.WriteAsync(new ReadOnlyMemory<byte>(buffer, 0, bytesRead), cancellationToken).ConfigureAwait(false);

                        switch (direction)
                        {
                            case Direction.Forward:
                                Interlocked.Add(ref _totalBytesForwarded, bytesRead);
                                break;
                            case Direction.Responding:
                                Interlocked.Add(ref _totalBytesResponded, bytesRead);
                                break;
                        }
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        internal enum Direction
        {
            Unknown = 0,
            Forward,
            Responding,
        }

        public int ConnectionTimeout { get; set; } = (4 * 60 * 1000);

        public async Task Start(string remoteServerHostNameOrAddress,
            ushort remoteServerPort,
            ushort localPort,
            string? localIp,
            bool? filterIPAddressRanges = false,
            List<string>? IPAddressRanges = null, HackjaggoProxyForm? form = null)
        {
            var connections = new ConcurrentBag<TcpConnection>();

            IPAddress localIpAddress = string.IsNullOrEmpty(localIp) ? IPAddress.IPv6Any : IPAddress.Parse(localIp);
            using var localServer = new TcpListener(new IPEndPoint(localIpAddress, localPort));
            localServer.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            localServer.Start();

            await form!.AddRejectedConnectionsListViewItemAsync("46.10.8.57", 0);
            await form!.AddRejectedConnectionsListViewItemAsync("46.10.8.57", 0);
            await form!.AddRejectedConnectionsListViewItemAsync("188.126.94.91", 0);

            form!.routingInformationLabel.Text = $"Routing over TCP from [{localIpAddress}]:{localPort} to [{remoteServerHostNameOrAddress}]:{remoteServerPort}";

            var _ = Task.Run(async () =>
            {
                while (!form.CancellationToken.Token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);

                    var tempConnections = new List<TcpConnection>(connections.Count);
                    while (connections.TryTake(out var connection))
                    {
                        tempConnections.Add(connection);
                    }

                    foreach (var tcpConnection in tempConnections)
                    {
                        if (tcpConnection.LastActivity + ConnectionTimeout < Environment.TickCount64)
                        {
                            var remoteEndPoint = tcpConnection._localServerConnection.Client.RemoteEndPoint as IPEndPoint;
                            form.RemoveCurrentConnectionFromListView(remoteEndPoint?.Address.ToString() ?? "", remoteEndPoint?.Port ?? 0);
                            tcpConnection.Stop();
                        }
                        else
                        {
                            connections.Add(tcpConnection);
                        }
                    }
                }
            });

            form.statusPictureBox.Image = Image.FromFile(@"./Resources/circle_11762483.png");
            form.startStopButton.Text = "Stop";

            while (!form.CancellationToken.Token.IsCancellationRequested)
            {
                try
                {
                    var ips = await Dns.GetHostAddressesAsync(remoteServerHostNameOrAddress).ConfigureAwait(false);

                    var tcpConnection = await TcpConnection.AcceptTcpClientAsync(form, localServer,
                            new IPEndPoint(ips[0], remoteServerPort)).ConfigureAwait(false);

                    var remoteEndPoint = tcpConnection._localServerConnection.Client.RemoteEndPoint as IPEndPoint;

                    bool filterAddresses = filterIPAddressRanges != null && filterIPAddressRanges == true;
                    if (filterAddresses && IsInPrivateRanges(remoteEndPoint, IPAddressRanges!))
                    {
                        tcpConnection.Run(form);
                        connections.Add(tcpConnection);
                    }
                    else
                    {
                        await form.AddRejectedConnectionsListViewItemAsync(remoteEndPoint!.Address.ToString(), remoteEndPoint!.Port).ConfigureAwait(false);
                        Console.Beep();
                        continue;
                    }
                }
                catch (Exception)
                {
                }
            }

            foreach (var conn in connections)
                conn.Stop();
        }

        private bool IsInPrivateRanges(IPEndPoint? clientEndPoint, List<string> ranges)
        {
            if (clientEndPoint == null)
                return false;

            var clientIp = clientEndPoint.Address;
            if (clientIp.IsIPv4MappedToIPv6)
                clientIp = clientIp.MapToIPv4();

            if (_checkedIpCache.TryGetValue(clientIp, out bool isPrivate))
            {
                return isPrivate;
            }

            bool result = ranges.Any(cidr =>
            {
                var addressRange = IPAddressRange.Parse(cidr);
                return addressRange.Contains(clientIp);
            });

            _checkedIpCache[clientIp] = result;
            return result;
        }        
    }
}