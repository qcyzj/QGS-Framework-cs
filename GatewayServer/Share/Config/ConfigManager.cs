
namespace Share.Config
{
    public class ConfigManager : Singleton<ConfigManager>
    {
        public const string LOCAL_IP_ADDRESS = "127.0.0.1";

        public const int TCP_USER_LISTEN_PORT = 11000;
        public const int UDP_USER_CONNECT_PORT = 12000;

        public const int TCP_GAME_SERVER_LISTEN_PORT = 13000;

        public const int TCP_ACCOUNT_SERVER_CONNECT_PORT = 14000;

        public const int GATEWAY_SERVER_ID = 1;
    }
}
