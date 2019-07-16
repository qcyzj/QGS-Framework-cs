
using System.Diagnostics;

using Share.Logs;
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

            uint game_server_id = pkt.GetUint();
            Debug.Assert(ConfigManager.Instance.GAME_SERVER_ID == game_server_id);

            LogManager.Info("Receive auth packet: Game server ID = " + game_server_id.ToString());

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }


        private void SendAuthPacket()
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.GS_GW_AUTH);
            pkt.AddUint(ConfigManager.Instance.GAME_SERVER_ID);

            SendPacket(pkt);
        }
    }
}
