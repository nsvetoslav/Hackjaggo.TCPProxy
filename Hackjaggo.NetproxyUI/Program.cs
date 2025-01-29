using Hackjaggo.Proxy;

namespace Hackjaggo.NetproxyUI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Logger.LogInfo("Test");
            Application.Run(new HackjaggoProxyForm());
        }
    }
}