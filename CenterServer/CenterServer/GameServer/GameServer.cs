using System.Diagnostics;

using Share.Net.Packets;
using Share.Net.Sessions;

namespace CenterServer.CenterServer.GameServers
{
    public sealed partial class GameServer
    {
        private const uint INVALID_GAME_SERVER_ID = 0;


        private Session m_ServerSession;
        private uint m_GameServerID;


        public uint GameServerID { get { return m_GameServerID; } }


        public GameServer()
        {
            m_ServerSession = null;
            m_GameServerID = INVALID_GAME_SERVER_ID;
        }


        public void Release()
        {
            if (null != m_ServerSession)
            {
                m_ServerSession.Release();
            }

            m_ServerSession = null;
            m_GameServerID = INVALID_GAME_SERVER_ID;
        }


        public void SetServerSessioin(Session server_sess)
        {
            Debug.Assert(null != server_sess);
            m_ServerSession = server_sess;
        }


        public void SetServerID(uint game_server_id)
        {
            Debug.Assert(INVALID_GAME_SERVER_ID == m_GameServerID);
            m_GameServerID = game_server_id;
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
