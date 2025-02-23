using System.Net;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Hackjaggo.Proxy;
using System.Windows.Forms;
using static Hackjaggo.NetproxyUI.ProxyCheck;

namespace Hackjaggo.NetproxyUI
{
    public partial class HackjaggoProxyForm : Form
    {
        private Proxy.ProxyConfig? ProxyConfig { get; set; }
        public List<Task> RunningProxyTasks { get; private set; } = new();

        public CancellationTokenSource CancellationToken { get; set; } = new();

        class ListViewItemTags
        {
            public object? PrimaryObject { get; set; }
            public object? SecondaryObject { get; set; }
            public object? TertiaryObject { get; set; }
        }

        public HackjaggoProxyForm()
        {
            InitializeComponent();

            InitializeControls();

            StartProxyAsync().ConfigureAwait(false);
        }

        private void SetRoutingInformationLableData()
        {
            string sourceIP = "127.0.0.1";
            string destinationIP = "192.168.0.1";
            routingInformationLabel.Text = $"Routing over TCP from {sourceIP} to {destinationIP}";
        }

        private void SetRejectedConnectionsListViewData()
        {
            rejectedConnectionsListView.View = View.Details;
            rejectedConnectionsListView.FullRowSelect = true;

            rejectedConnectionsListView.Columns.AddRange(new[]
            {
                new ColumnHeader { Text = "Date", Width = 200 },
                new ColumnHeader { Text = "Proxy", Width = 60},
                new ColumnHeader { Text = "IP Address", Width = 200 },
                new ColumnHeader { Text = "Port", Width = 50 },
            });

            rejectedConnectionsListView.ListViewItemSorter = new DateComparer();
            rejectedConnectionsListView.ItemActivate += RejectedConnectionsListView_ItemActivate!;

            rejectedConnectionsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            foreach (ColumnHeader column in rejectedConnectionsListView.Columns)
                column.Width += 10;

            ImageList imageList = new ImageList();

            imageList.Images.Add("error", Image.FromFile("./Resources/icons8-alert-48.png"));
            imageList.Images.Add("warning", Image.FromFile("./Resources/icons8-warning-666.png"));

            rejectedConnectionsListView.SmallImageList = imageList;

            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
            this.locateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SuspendLayout();

            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.locateToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(110, 28);

            this.locateToolStripMenuItem.Name = "locateToolStripMenuItem";
            this.locateToolStripMenuItem.Size = new System.Drawing.Size(109, 24);
            this.locateToolStripMenuItem.Text = "Locate";
            this.locateToolStripMenuItem.Click += new System.EventHandler(this.OnLocateIPAddress!);


            rejectedConnectionsListView.ContextMenuStrip = contextMenuStrip1;
        }

        private void SetCurrentConnectionsListViewData()
        {
            currentConnectionsListView.View = View.Details;
            currentConnectionsListView.FullRowSelect = true;

            currentConnectionsListView.ListViewItemSorter = new DateComparer();

            currentConnectionsListView.Columns.AddRange(new ColumnHeader[]
            {
                new ColumnHeader { Text = "Date", Width = 200 },
                new ColumnHeader { Text = "IP Address", Width = 200 },
                new ColumnHeader { Text = "Port", Width = 60 },
            });

            currentConnectionsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            foreach (ColumnHeader column in rejectedConnectionsListView.Columns)
                column.Width += 10;

            ImageList imageList = new ImageList();

            imageList.Images.Add("info", Image.FromFile("./Resources/icons8-info-48.png"));

            currentConnectionsListView.SmallImageList = imageList;
        }

        private void SetStatusPictureBoxDefaultState()
        {
            statusPictureBox.Image = Image.FromFile("./Resources/icons8-dot-16.png");
            startStopButton.Text = "Start";
        }

        private void InitializeControls()
        {
            SetRoutingInformationLableData();

            SetRejectedConnectionsListViewData();

            SetCurrentConnectionsListViewData();
        }

        private void ResizeRejectedConnectionsListViewColumnContent()
        {
            rejectedConnectionsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            foreach (ColumnHeader column in rejectedConnectionsListView.Columns)
                column.Width += 10;
        }

        private void ResizeCurrentConnectionsListViewColumnContent()
        {
            currentConnectionsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            foreach (ColumnHeader column in currentConnectionsListView.Columns)
                column.Width += 10;
        }

        public async Task StartProxyAsync()
        {
            try
            {
                var configJson = File.ReadAllText("config.json");
                this.ProxyConfig = System.Text.Json.JsonSerializer.Deserialize<Proxy.ProxyConfig?>(configJson);
                if (this.ProxyConfig == null)
                {
                    throw new Exception("configs is null");
                }

                var runningProxyTasksLocal = RunProxyFromConfig("", this.ProxyConfig, this);
                RunningProxyTasks.AddRange(runningProxyTasksLocal);

                await Task.WhenAll(RunningProxyTasks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred : {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task AddRejectedConnectionsListViewItemAsync(string ipAddress, int port)
        {
            try
            {
                if (rejectedConnectionsListView.InvokeRequired)
                {
                    rejectedConnectionsListView.Invoke(new Action(() => AddRejectedConnectionsListViewItemAsync(ipAddress, port).ConfigureAwait(false)));
                }
                else
                {
                    var proxyChecker = new ProxyCheck();
                    var ipf4addr = IPAddress.Parse(ipAddress).MapToIPv4().ToString();
                    var proxyCheckResult = await proxyChecker.GetProxyInfoAsync(ipf4addr);
                    var isProxyAvailable = proxyCheckResult != null && proxyCheckResult.Proxy.Contains("yes");

                    var datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var listItem = new ListViewItem(datetime)
                    {
                        ImageKey = isProxyAvailable ? "error" : "warning",
                        ForeColor = isProxyAvailable ? Color.FromKnownColor(KnownColor.Red) : Color.FromKnownColor(KnownColor.Green),
                        Tag = new ListViewItemTags()
                        {
                            PrimaryObject = DateTime.Now,
                            SecondaryObject = $"https://whatismyipaddress.com/ip/{ipf4addr}",
                            TertiaryObject = proxyCheckResult
                        },
                    };

                    listItem.SubItems.Add(isProxyAvailable ? "YES" : "NO");
                    listItem.SubItems.Add(ipAddress);
                    listItem.SubItems.Add(port.ToString());

                    rejectedConnectionsListView.Items.Add(listItem);

                    rejectedConnectionsListView.Sort();

                    ResizeRejectedConnectionsListViewColumnContent();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception: {ex.Message}");
            }
        }

        public void RemoveCurrentConnectionFromListView(string ipAddress, int port)
        {
            if (currentConnectionsListView.InvokeRequired)
            {
                currentConnectionsListView.Invoke(new Action(() => RemoveCurrentConnectionFromListView(ipAddress, port)));
            }
            else
            {
                foreach (ListViewItem item in currentConnectionsListView.Items)
                {
                    if (item.SubItems[1].Text == ipAddress && item.SubItems[2].Text == port.ToString())
                    {
                        currentConnectionsListView.Items.Remove(item);
                        break;
                    }
                }
                currentConnectionsListView.Sort();
            }
        }

        public void AddCurrentConnectionsListView(string ipAddress, int port)
        {
            try
            {
                if (currentConnectionsListView.InvokeRequired)
                {
                    currentConnectionsListView.Invoke(new Action(() => AddCurrentConnectionsListView(ipAddress, port)));
                }
                else
                {
                    var datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var listItem = new ListViewItem(datetime)
                    {
                        Tag = new ListViewItemTags()
                        {
                            PrimaryObject = DateTime.Now,
                            SecondaryObject = null,

                        },
                        ImageKey = "info",
                        ForeColor = Color.FromKnownColor(KnownColor.Green),
                    };

                    listItem.SubItems.Add(ipAddress);
                    listItem.SubItems.Add(port.ToString());

                    currentConnectionsListView.Items.Add(listItem);

                    ResizeCurrentConnectionsListViewColumnContent();
                    currentConnectionsListView.Sort();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception: {ex.Message}");
            }
        }

        public class DateComparer : IComparer
        {
            public int Compare(object? x, object? y)
            {
                ListViewItem? itemX = (ListViewItem?)x;
                ListViewItem? itemY = (ListViewItem?)y;

                if (itemX != null && itemY != null)
                {
                    // If you have a Tag with DateTime in the first column, use it for sorting
                    if (itemX.Tag is ListViewItemTags itemXTag && itemY.Tag is ListViewItemTags itemYTag)
                    {
                        if (itemXTag.PrimaryObject is DateTime dateX && itemYTag.PrimaryObject is DateTime dateY)
                        {
                            return dateY.CompareTo(dateX);
                        }
                    }
                }

                return 0;
            }
        }

        private void RejectedConnectionsListView_ItemActivate(object sender, EventArgs e)
        {
            try
            {
                ListViewItem selectedItem = rejectedConnectionsListView.SelectedItems[0];

                if (selectedItem.Tag is ListViewItemTags tag)
                {
                    if (tag.SecondaryObject is string addr)
                    {
                        if (!string.IsNullOrEmpty(addr))
                        {
                            OpenUrl(addr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception: {ex.Message}");
            }
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private async void startStopButton_ClickAsync(object sender, EventArgs e)
        {
            if (startStopButton.Text == "Stop")
            {
                if (CancellationToken.Token.CanBeCanceled)
                {
                    await CancellationToken.CancelAsync();
                }

                await Task.WhenAll(RunningProxyTasks).ConfigureAwait(true);

                CancellationToken = new CancellationTokenSource();
                startStopButton.Text = "Start";
                statusPictureBox.Image = Image.FromFile("./Resources/icons8-dot-16.png");
            }

            else if (startStopButton.Text == "Start")
            {
                _ = StartProxyAsync().ConfigureAwait(false);
                startStopButton.Text = "Stop";
                statusPictureBox.Image = Image.FromFile(@"./Resources/circle_11762483.png");
            }
        }

        private void OnCurrentConnectionsClearButton(object sender, EventArgs e)
        {
            currentConnectionsListView.Items.Clear();
        }

        private void OnRejectedConnectionsClearButton(object sender, EventArgs e)
        {
            rejectedConnectionsListView.Items.Clear();
        }

        private IEnumerable<Task> RunProxyFromConfig(string proxyName, ProxyConfig proxyConfig, HackjaggoProxyForm? form = null)
        {
            var forwardPort = proxyConfig.forwardPort;
            var localPort = proxyConfig.localPort;
            var forwardIp = proxyConfig.forwardIp;
            var localIp = proxyConfig.localIp;

            try
            {
                if (forwardIp == null)
                {
                    throw new Exception("forwardIp is null");
                }
                if (forwardPort <= 0)
                {
                    throw new Exception("forwardPort is null");
                }
                if (localPort <= 0)
                {
                    throw new Exception("localPort is null");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to start {proxyName} : {ex.Message}");
                throw;
            }

            Task task;

            try
            {
                task = new TcpProxy().Start(proxyConfig,
                    form);

            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to start {proxyName} : {ex.Message}");
                throw;
            }

            yield return task;
        }

        private void OnLocateIPAddress(object sender, EventArgs e)
        {
            try
            {
                ListViewItem selectedItem = rejectedConnectionsListView.SelectedItems[0];

                if (selectedItem.Tag is ListViewItemTags tag)
                {
                    if (tag.TertiaryObject is IpInfoDetails ipInfoDetails)
                    {
                        GoogleMapsForm googleMapsForm = new GoogleMapsForm(ipInfoDetails.Longitude, ipInfoDetails.Latitude, this.ProxyConfig!.GoogleAPIKey);

                        googleMapsForm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception: {ex.Message}");
            }
        }
    }
}
