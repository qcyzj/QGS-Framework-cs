using Share;
using Share.Config;
using Share.Net.Packets;

namespace GameServer.GameServer.Gateway
{
    public sealed class GatewayServerManager : Singleton<GatewayServerManager>
    {
        private GatewayServer m_GatewayServer;


        public GatewayServerManager()
        {
            m_GatewayServer = new GatewayServer();
        }


        public void Start()
        {
            RegisterGatewayProc();

            m_GatewayServer.Initialize();

            m_GatewayServer.ConnectToGatewayAsync(ConfigManager.LOCAL_IP_ADDRESS,
                                                  ConfigManager.TCP_GATEWAY_SERVER_CONNECT_PORT);

            ValidInitializeOnce();
        }

        public void Stop()
        {
            ValidReleaseOnce();

            m_GatewayServer.Release();
        }


        private void RegisterGatewayProc()
        {
            RegisterProcImpl(Protocol.GS_GW_AUTH, GatewayServer.PacketProcessAuth);
        }

        private void RegisterProcImpl(int protocol, PacketProcessManager.PacketProcessFunc func)
        {
            PacketProcessManager.Instance.RegisterProc(protocol, func);
        }
    }
}
