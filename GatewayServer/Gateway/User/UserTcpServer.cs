using System.Diagnostics;

using Share.Config;
using Share.Net.Server;
using Share.Net.Sessions;

namespace GatewayServer.Gateway.User
{
    public sealed class UserTcpServer : TcpServer
    {
        public UserTcpServer()
            :base(TcpServer.SOCKET_BLOCKING_TYPE.BLOCKING, ConfigManager.TCP_USER_LISTEN_PORT)
        {}


        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }


        protected override Session AllocateSession()
        {
            Session user_sess = SessionManager.Instance.AllocateTcpUserSession();
            Debug.Assert(null != user_sess);
            Debug.Assert(user_sess is TcpSession);

            return user_sess;
        }

        protected override void FreeSession(Session sess)
        {
            Debug.Assert(null != sess);
            Debug.Assert(sess is TcpSession);
            SessionManager.Instance.FreeTcpUserSession(sess);
        }   

        protected override SOCK_SERV_ERROR AddToRelevantManager(Session sess)
        {
            SOCK_SERV_ERROR ret = SOCK_SERV_ERROR.SUCCESS;
            Debug.Assert(null != sess);
            Debug.Assert(sess is TcpSession);

            User user = UserManager.Instance.AllocateUser();

            if (null != user)
            {
                TcpSession user_sess = (TcpSession)sess;
                user_sess.SetObject(user);

                user.SetUserSession(user_sess);

                UserManager.Instance.AddConnectedUser(user);
            }
            else
            {
                ret = SOCK_SERV_ERROR.E_USER_SESSION_IS_EMPTY;
            }

            return ret;
        }
    }
}
