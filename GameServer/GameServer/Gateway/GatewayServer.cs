using System.Diagnostics;

using Share.Net.Packets;
using Share.Net.Sessions;

namespace GameServer.GameServer.Gateway
{
    public sealed partial class GatewayServer
    {
        private Session m_ServerSession;


        public GatewayServer()
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


        public void ConnectToGatewayAsync(string ip_addr, int port)
        {
            Debug.Assert(m_ServerSession is TcpSession);
            TcpSession tcp_session = m_ServerSession as TcpSession;

            tcp_session.SetConnectedEvent(GatewayServer.OnAsyncConnected);

            tcp_session.ConnectAsync(this, ip_addr, port);
        }

        public static void OnAsyncConnected(object obj)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is GatewayServer);

            GatewayServer gate_server = obj as GatewayServer;

            gate_server.SendAuthPacket();
        }


        private void SendPacket(Packet pkt)
        {
            if (null != m_ServerSession)
            {
                m_ServerSession.SendPacket(pkt);
            }

            PacketManager.Instance.ReleasePacket(pkt);
        }
    }
}
