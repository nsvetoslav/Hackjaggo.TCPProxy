using Hackjaggo.NetproxyUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hackjaggo.Proxy
{
    public interface IProxy
    {
        Task Start(string remoteServerHostNameOrAddress,
            ushort remoteServerPort,
            ushort localPort,
            string? localIp = null,
            bool? filterIPAddressRanges = false,
            List<string>? IPAddressRanges = null, HackjaggoProxyForm? form = null);
    }
}
