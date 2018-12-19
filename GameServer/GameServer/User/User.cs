using System.Diagnostics;

using Share.Net.Packets;

namespace GameServer.GameServer.User
{
    public partial class User
    {
        public const int INVALID_USER_ID = 0;


        private uint m_UserID;


        public User()
        {
            m_UserID = INVALID_USER_ID;
        }


        public void Release()
        {
            //Debug.Assert(INVALID_USER_ID != m_UserID);
            m_UserID = INVALID_USER_ID;
        }


        public void SetUserID(uint user_id)
        {
            Debug.Assert(INVALID_USER_ID == m_UserID);
            Debug.Assert(INVALID_USER_ID != user_id);
            m_UserID = user_id;
        }


        private void SendPacket(Packet pkt)
        {

            PacketManager.Instance.ReleasePacket(pkt);
        }
    }
}
