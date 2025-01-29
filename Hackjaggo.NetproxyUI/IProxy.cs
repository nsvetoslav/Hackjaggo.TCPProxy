using Hackjaggo.NetproxyUI;

namespace Hackjaggo.Proxy
{
    public interface IProxy
    {
        Task Start(ProxyConfig config,
            HackjaggoProxyForm? form = null);
    }
}
