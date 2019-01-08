using System.Diagnostics;

using Share;
using Share.Config;
using Share.Net.Packets;

namespace GameServer.GameServer.CenterServer
{
    public partial class CenterServer
    {
        public static int PacketProcessAuth(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is CenterServer);
            Debug.Assert(Protocol.GS_CNT_AUTH == pkt.GetPacketID());

            CenterServer center_server = obj as CenterServer;

            uint game_server_id = pkt.GetUint();
            Debug.Assert(ConfigManager.Instance.GAME_SERVER_ID == game_server_id);

            LogManager.Info("Receive auth packet: Game server ID = " + game_server_id.ToString());

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }


        private void SendAuthPacket()
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.GS_CNT_AUTH);
            pkt.AddUint(ConfigManager.Instance.GAME_SERVER_ID);

            SendPacket(pkt);
        }
    }
}
