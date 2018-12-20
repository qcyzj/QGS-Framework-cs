using AccountServer.AccountServer.WinService;

namespace LoginServer
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
