
namespace Share.Config
{
    public class ConfigManager : Singleton<ConfigManager>
    {
        public const string LOCAL_IP_ADDRESS = "127.0.0.1";

        public const int TCP_GATEWAY_SERVER_LISTEN_PORT = 14000;

        public const long ID_GENERATOR_WORKER_ID = 1;
        public const long ID_GENERATOR_DATACENTER_ID = 1;
    }
}
