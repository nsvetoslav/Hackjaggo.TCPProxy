using Hackjaggo.Proxy;

namespace Hackjaggo.NetproxyUI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new HackjaggoProxyForm());
        }
    }
}