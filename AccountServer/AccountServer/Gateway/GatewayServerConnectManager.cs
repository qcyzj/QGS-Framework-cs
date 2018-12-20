using Share;

namespace AccountServer.AccountServer.Gateway
{
    public sealed class GatewayServerConnectManager : Singleton<GatewayServerConnectManager>
    {
        private GatewayTcpServer m_GatewayTcpServer;


        public GatewayServerConnectManager()
        {
            m_GatewayTcpServer = new GatewayTcpServer();
        }


        public void Start()
        {
            m_GatewayTcpServer.Start();
        }

        public void Stop()
        {
            m_GatewayTcpServer.Stop();
        }
    }
}
