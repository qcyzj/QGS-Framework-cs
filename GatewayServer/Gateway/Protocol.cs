
namespace GatewayServer.Gateway
{
    public static class Protocol
    {
        public const short GW_LS_PROTOCOL_BASE = 0;


        public const int GS_GW_PROTOCOL_BASE    = 1000;
        public const int GS_GW_AUTH             = GS_GW_PROTOCOL_BASE + 1;


        public const short GS_GC_PROTOCOL_BASE  = 2000;









        public const int CLI_GW_PROTOCOL_BASE   = 10000;
        public const int CLI_GW_ENTER_AUTH      = CLI_GW_PROTOCOL_BASE + 0;
        public const int CLI_GW_ENTER_TEST      = CLI_GW_PROTOCOL_BASE + 1;
        public const int CLI_GW_ENTER_TEST_2    = CLI_GW_PROTOCOL_BASE + 2;

        public const int CLI_GW_ENTER_REGISTER  = CLI_GW_PROTOCOL_BASE + 3;
        public const int CLI_GW_ENTER_LOGIN     = CLI_GW_PROTOCOL_BASE + 4;


        public const int UDP_CLI_GW_PROTOCOL_BASE   = 11000;
        public const int UDP_CLI_GW_CONNECT         = UDP_CLI_GW_PROTOCOL_BASE + 0;
        public const int UDP_CLI_GW_AUTH            = UDP_CLI_GW_PROTOCOL_BASE + 1;
        public const int UDP_CLI_GW_TEST            = UDP_CLI_GW_PROTOCOL_BASE + 2;
        public const int UDP_CLI_GW_TEST_2          = UDP_CLI_GW_PROTOCOL_BASE + 3;

    }
}
