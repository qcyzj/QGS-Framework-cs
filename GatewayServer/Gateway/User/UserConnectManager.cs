using Share;

namespace GatewayServer.Gateway.User
{
    public sealed class UserConnectManager : Singleton<UserConnectManager>
    {
        private UserTcpServer m_UserTcpServer;
        private UserUdpServer m_UserUdpServer;


        public UserConnectManager()
        {
            m_UserTcpServer = new UserTcpServer();
            m_UserUdpServer = new UserUdpServer();
        }


        public void Start()
        {
            m_UserTcpServer.Start();
            //m_UserUdpServer.Start();
        }

        public void Stop()
        {
            m_UserTcpServer.Stop();
            //m_UserUdpServer.Stop();
        }
    }
}
