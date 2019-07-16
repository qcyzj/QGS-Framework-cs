using System.Diagnostics;

using Share.Logs;
using Share.Net.Packets;

namespace AccountServer.AccountServer.Gateway
{
    public partial class GatewayServer
    {
        public static int PacketProcessAuth(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is GatewayServer);
            Debug.Assert(Protocol.GW_ACT_AUTH == pkt.GetPacketID());

            GatewayServer gateway_server = obj as GatewayServer;

            uint gateway_server_id = pkt.GetUint();

            LogManager.Info("Receive gateway server auth pakcet: Gateway server ID = " +
                            gateway_server_id.ToString());

            gateway_server.SetServerID(gateway_server_id);
            GatewayServerManager.Instance.AddAuthedGatewayServer(gateway_server);

            gateway_server.SendAuthPacket(gateway_server_id);

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }


        private void SendAuthPacket(uint gateway_server_id)
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.GW_ACT_AUTH);
            pkt.AddUint(gateway_server_id);

            SendPacket(pkt);
        }
    }
}
