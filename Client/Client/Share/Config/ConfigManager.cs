
namespace Share.Config
{
    public class ConfigManager : Singleton<ConfigManager>
    {
        public const string LOCAL_IP_ADDRESS = "127.0.0.1";

        public const int TCP_USER_CONNECT_PORT = 11000;
        public const int UDP_USER_CONNECT_PORT = 12000;
    }
}
