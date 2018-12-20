using CenterServer.CenterServer.WinService;

namespace CenterServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WinServiceManager.Instance.Init();

            WinServiceManager.Instance.Loop();

            WinServiceManager.Instance.Exit();
        }
    }
}
