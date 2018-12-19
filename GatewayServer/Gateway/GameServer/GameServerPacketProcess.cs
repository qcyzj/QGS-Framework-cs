using System.Diagnostics;

using Share;
using Share.Net.Packets;

namespace GatewayServer.Gateway.GameServers
{
    public partial class GameServer
    {
        public static int PacketProcessAuth(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is GameServer);
            Debug.Assert(Protocol.GS_GW_AUTH == pkt.GetPacketID());

            GameServer game_server = obj as GameServer;

            uint game_server_id = pkt.GetUint();

            LogManager.Info("Receive game server auth packet: Game server ID = " +
                            game_server_id.ToString());

            game_server.SetServerID(game_server_id);
            GameServerManager.Instance.AddAuthedGameServer(game_server);

            game_server.SendAuthPacket(game_server_id);

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }


        private void SendAuthPacket(uint game_server_id)
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.GS_GW_AUTH);
            pkt.AddUint(game_server_id);

            SendPacket(pkt);
        }
    }
}
