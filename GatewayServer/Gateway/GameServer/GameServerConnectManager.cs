
using Share;

namespace GatewayServer.Gateway.GameServers
{
    public sealed class GameServerConnectManager : Singleton<GameServerConnectManager>
    {
        private GameTcpServer m_GameTcpServer;


        public GameServerConnectManager()
        {
            m_GameTcpServer = new GameTcpServer();
        }


        public void Start()
        {
            m_GameTcpServer.Start();
        }

        public void Stop()
        {
            m_GameTcpServer.Stop();
        }
    }
}
