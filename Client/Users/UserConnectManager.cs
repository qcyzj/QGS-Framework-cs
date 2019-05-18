using System.Threading;
using System.Diagnostics;

using Share;
using Share.Config;
using Share.Net.Sessions;

namespace Client.Users
{
    public sealed class UserConnectManager : Singleton<UserConnectManager>
    {
        private const int USER_NUM_MAX = 100;
        private const int TCP_USER_ID_BASE = 0;
        private const int UDP_USER_ID_BASE = 10000;


        public UserConnectManager()
        {}


        public void Start()
        {
            CreateAsyncTcpClients();
            //CreateAsyncUdpClinets();
        }

        public void Stop()
        {}


        private void CreateTcpClients()
        {
            int user_num = USER_NUM_MAX;

            for (int i = 1; i <= user_num; ++i)
            {
                User user = UserManager.Instance.AllocateUser();
                Debug.Assert(null != user);
                user.SetUserID((uint)(TCP_USER_ID_BASE + i));

                int ret = user.ConnectToGateway(ConfigManager.LOCAL_IP_ADDRESS, 
                                                ConfigManager.TCP_USER_CONNECT_PORT);

                if ((int)Session.SESSION_CONNECT_ERROR.SUCCESS != ret)
                {
                    LogManager.Error("user connect failed!: error = " + (int)ret);
                }
                else
                {
                    UserManager.Instance.AddConnectedUser(user);

                    user.SendAuthPacket();
                    user.StartSendAsync();
                }

                Thread.Sleep(1);
            }
        }

        private void CreateAsyncTcpClients()
        {
            int user_num = USER_NUM_MAX;

            for (int i = 1; i <= user_num; ++i)
            {
                User user = UserManager.Instance.AllocateUser();
                Debug.Assert(null != user);
                user.SetUserID((uint)(TCP_USER_ID_BASE + i));

                Session tcp_sess = SessionManager.Instance.AllocateTcpUserSession();
                Debug.Assert(null != tcp_sess);

                user.SetUserSession(tcp_sess);

                ((TcpSession)tcp_sess).SetConnectedEvent(User.OnAsyncConnected);


                user.ConnectToGatewayAsync(ConfigManager.LOCAL_IP_ADDRESS, 
                                           ConfigManager.TCP_USER_CONNECT_PORT);
            }
        }

        private void CreateAsyncUdpClinets()
        {
            int user_num = USER_NUM_MAX;

            for (int i = 1; i <= user_num; ++i)
            {
                User user = UserManager.Instance.AllocateUser();
                Debug.Assert(null != user);
                user.SetUserID((uint)(UDP_USER_ID_BASE + i));

                Session udp_sess = SessionManager.Instance.AllocateUdpUserSession();
                Debug.Assert(null != udp_sess);

                user.SetUserSession(udp_sess);

                
                user.SendUdpConnectPacket();
                user.StartSendToAsync(ConfigManager.LOCAL_IP_ADDRESS, 
                                      ConfigManager.UDP_USER_CONNECT_PORT);
            }
        }
    }
}
