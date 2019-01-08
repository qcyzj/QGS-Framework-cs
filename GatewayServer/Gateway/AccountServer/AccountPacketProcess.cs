using System.Diagnostics;

using Share;
using Share.Config;
using Share.Net.Packets;

namespace GatewayServer.Gateway.AccountServer
{
    public partial class AccountServer
    {
        public static int PacketProcessAuth(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is AccountServer);
            Debug.Assert(Protocol.GW_ACT_AUTH == pkt.GetPacketID());

            AccountServer account_server = obj as AccountServer;

            uint gateway_server_id = pkt.GetUint();
            Debug.Assert(ConfigManager.Instance.GATEWAY_SERVER_ID == gateway_server_id);

            LogManager.Info("Receive auth packet: Gateway server ID = " + 
                            gateway_server_id.ToString());

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }


        private void SendAuthPacket()
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.GW_ACT_AUTH);
            pkt.AddUint(ConfigManager.Instance.GATEWAY_SERVER_ID);

            SendPacket(pkt);
        }
    }
}
