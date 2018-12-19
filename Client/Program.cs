
using Client.ClientService;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientServiceManager.Instance.Init();

            ClientServiceManager.Instance.Loop();

            ClientServiceManager.Instance.Exit();
        }
    }
}
