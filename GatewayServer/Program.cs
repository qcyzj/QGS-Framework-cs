
using GatewayServer.Test;
using GatewayServer.Gateway.WinService;

namespace GatewayServer
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            //TestMain main_test = new TestMain();
            //main_test.RunAllTest();
#endif


            WinServiceManager.Instance.Init();

            WinServiceManager.Instance.Loop();

            WinServiceManager.Instance.Exit();
        }
    }
}
