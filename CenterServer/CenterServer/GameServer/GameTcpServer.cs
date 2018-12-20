using System.Diagnostics;

using Share.Config;
using Share.Net.Server;
using Share.Net.Sessions;

namespace CenterServer.CenterServer.GameServers
{
    public sealed class GameTcpServer : TcpServer
    {
        public GameTcpServer()
            :base(TcpServer.SOCKET_BLOCKING_TYPE.BLOCKING, ConfigManager.TCP_CENTER_SERVER_LISTEN_PORT)
        { }


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
            Session server_sess = SessionManager.Instance.AllocateServerSession();
            Debug.Assert(null != server_sess);
            Debug.Assert(server_sess is TcpSession);

            return server_sess;
        }

        protected override void FreeSession(Session sess)
        {
            Debug.Assert(null != sess);
            Debug.Assert(sess is TcpSession);
            SessionManager.Instance.FreeServerSession(sess);
        }


        protected override SOCK_SERV_ERROR AddToRelevantManager(Session sess)
        {
            SOCK_SERV_ERROR ret = SOCK_SERV_ERROR.SUCCESS;
            Debug.Assert(null != sess);
            Debug.Assert(sess is TcpSession);

            GameServer server = GameServerManager.Instance.AllocateGameServer();

            if (null != server)
            {
                TcpSession server_sess = (TcpSession)sess;
                server_sess.SetObject(server);

                server.SetServerSessioin(server_sess);

                GameServerManager.Instance.AddConnectedGameServer(server);
            }
            else
            {
                ret = SOCK_SERV_ERROR.E_SERVER_SESSION_IS_EMPTY;
            }

            return ret;
        }
    }
}
