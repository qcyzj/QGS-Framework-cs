using System.Diagnostics;

using Share.Net.Packets;
using Share.Net.Sessions;

namespace GameServer.GameServer.CenterServer
{
    public sealed partial class CenterServer
    {
        private Session m_ServerSession;


        public CenterServer()
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


        public void ConnectToCenterServerAsync(string ip_addr, int port)
        {
            Debug.Assert(m_ServerSession is TcpSession);
            TcpSession tcp_sess = m_ServerSession as TcpSession;

            tcp_sess.SetConnectedEvent(OnAsyncConnected);

            tcp_sess.ConnectAsync(this, ip_addr, port);
        }

        public static void OnAsyncConnected(object obj)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is CenterServer);

            CenterServer center_server = obj as CenterServer;

            center_server.SendAuthPacket();
            center_server.StartSendAsync();
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
            TcpSession tcp_Sess = m_ServerSession as TcpSession;

            tcp_Sess.StartSendAsync();
        }
    }
}
