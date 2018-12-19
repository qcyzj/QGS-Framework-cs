
using System.Diagnostics;

using Share;
using Share.Config;
using Share.Net.Packets;

namespace GameServer.GameServer.Gateway
{
    public partial class GatewayServer
    {
        public static int PacketProcessAuth(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is GatewayServer);
            Debug.Assert(Protocol.GS_GW_AUTH == pkt.GetPacketID());

            GatewayServer gate_server = obj as GatewayServer;

            uint game_serer_id = pkt.GetUint();
            Debug.Assert(ConfigManager.GAME_SERVER_ID == game_serer_id);

            LogManager.Info("Receive auth packet: Game server ID = " + game_serer_id.ToString());

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }


        private void SendAuthPacket()
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.GS_GW_AUTH);
            pkt.AddUint(ConfigManager.GAME_SERVER_ID);

            SendPacket(pkt);
        }
    }
}
