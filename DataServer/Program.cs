using DataServer.DataServer.WinService;

namespace DataServer
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
