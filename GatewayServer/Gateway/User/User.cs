using System.Diagnostics;

using Share.Net.Packets;
using Share.Net.Sessions;

namespace GatewayServer.Gateway.User
{
    public sealed partial class User
    {
        public const uint INVALID_USER_ID = 0;


        private Session m_UserSession;
        private uint m_UserID;


        public uint UserID { get { return m_UserID; } }


        public User()
        {
            m_UserSession = null;
            m_UserID = INVALID_USER_ID;   
        }


        public void Release()
        {
            if (null != m_UserSession)
            {
                m_UserSession.Release();
            }

            m_UserSession = null;
            m_UserID = INVALID_USER_ID;
        }


        public void SetUserSession(Session user_sess)
        {
            Debug.Assert(null != user_sess);
            m_UserSession = user_sess;
        }


        public void SetUserID(uint user_id)
        {
            Debug.Assert(INVALID_USER_ID == m_UserID);
            Debug.Assert(INVALID_USER_ID != user_id);

            m_UserID = user_id;
        }


        private void SendPacket(Packet pkt)
        {
            if (Protocol.UDP_CLI_GW_AUTH == pkt.GetPacketID())
            {
                Debug.Assert(INVALID_USER_ID == m_UserID);
            }
            else
            {
                Debug.Assert(INVALID_USER_ID != m_UserID);
            }

            if (null != m_UserSession)
            {
                m_UserSession.SendPacket(pkt);
            }

            PacketManager.Instance.ReleasePacket(pkt);
        }
    }
}
