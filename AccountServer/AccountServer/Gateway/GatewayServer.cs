using System.Diagnostics;

using Share.Net.Packets;
using Share.Net.Sessions;

namespace AccountServer.AccountServer.Gateway
{
    public sealed partial class GatewayServer
    {
        private const uint INVALID_GATEWAY_SERVER_ID = 0;


        private Session m_ServerSession;
        private uint m_GatewayServerID;


        public uint GatewayServerID { get { return m_GatewayServerID; } }


        public GatewayServer()
        {
            m_ServerSession = null;
            m_GatewayServerID = INVALID_GATEWAY_SERVER_ID;
        }


        public void Release()
        {
            if (null != m_ServerSession)
            {
                m_ServerSession.Release();
            }

            m_ServerSession = null;
            m_GatewayServerID = INVALID_GATEWAY_SERVER_ID;
        }


        public void SetServerSession(Session server_sesss)
        {
            Debug.Assert(null != server_sesss);
            m_ServerSession = server_sesss;
        }


        public void SetServerID(uint gateway_server_id)
        {
            Debug.Assert(INVALID_GATEWAY_SERVER_ID == m_GatewayServerID);
            m_GatewayServerID = gateway_server_id;
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
