using System.Diagnostics;

using Share.Net.Packets;
using Share.Net.Sessions;

namespace GatewayServer.Gateway.AccountServer
{
    public sealed partial class AccountServer
    {
        private Session m_ServerSession;


        public AccountServer()
        {
            m_ServerSession = null;
        }


        public void Initialize()
        {
            m_ServerSession = SessionManager.Instance.AllocateServerSession();
            Debug.Assert(null != m_ServerSession);
        }

        public void Release()
        {
            m_ServerSession.Release();

            SessionManager.Instance.FreeServerSession(m_ServerSession);

            m_ServerSession = null;
        }


        public void ConnectToAccountServerAsync(string ip_addr, int port)
        {
            Debug.Assert(m_ServerSession is TcpSession);
            TcpSession tcp_sess = m_ServerSession as TcpSession;

            tcp_sess.SetConnectedEvent(OnAsynConnected);

            tcp_sess.ConnectAsync(this, ip_addr, port);
        }

        public static void OnAsynConnected(object obj)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is AccountServer);

            AccountServer account_server = obj as AccountServer;

            account_server.SendAuthPacket();
            account_server.StartSendAsync();
        }


        private void SendPacket(Packet pkt)
        {
            if (null != m_ServerSession)
            {
                m_ServerSession.SendPacket(pkt);
            }

            PacketManager.Instance.ReleasePacket(pkt);
        }

        private void StartSendAsync()
        {
            Debug.Assert(m_ServerSession is TcpSession);
            TcpSession tcp_sess = m_ServerSession as TcpSession;

            tcp_sess.StartSendAsync();
        }
    }
}
