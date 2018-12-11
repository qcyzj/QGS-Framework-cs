using System.Diagnostics;

using Share;
using Share.Net.Packets;

namespace GatewayServer.Gateway.User
{
    public partial class User
    {
        public static int PacketProcessAuth(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is User);
            Debug.Assert(Protocol.CLI_GW_ENTER_AUTH == pkt.GetPacketID());

            User user = (User)obj;

            LogManager.Info("Receive auth packet. UserID = " + user.UserID.ToString());

            uint user_id = pkt.GetUint();
            user.SetUserID(user_id);

            UserManager.Instance.AddAuthedUser(user);

            user.SendAuthPacket();

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }

        public static int PacketProcessTest(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is User);
            Debug.Assert(Protocol.CLI_GW_ENTER_TEST == pkt.GetPacketID());

            User user = (User)obj;

            int value = pkt.GetInt();
            LogManager.Info("Receive test packet: " + value.ToString() + ". UserID = " + user.UserID.ToString());

            user.SendTestPacket(value);
            
            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }

        public static int PacketProcessTest_2(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is User);
            Debug.Assert(Protocol.CLI_GW_ENTER_TEST_2 == pkt.GetPacketID());

            User user = (User)obj;

            string value = pkt.GetString();
            LogManager.Info("Receive test 2 packet: " + value + ". UserID = " + user.UserID.ToString());

            user.SendTest_2_Packet(value);

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }


        public static int PacketProcessEnterRegister(object obj, Packet pkt)
        {
            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }

        public static int PacketProcessEnterLogin(object obj, Packet pkt)
        {
            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }


        public static int PacketProcessUdpRegister(object obj, Packet pkt)
        {
            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }

        public static int PacketProcessUdpTest(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is User);
            Debug.Assert(Protocol.UDP_CLI_GW_TEST == pkt.GetPacketID());

            User user = (User)obj;

            int value = pkt.GetInt();
            LogManager.Info("Receive udp test packet: " + value.ToString() + ", UserId = " + user.UserID.ToString());

            user.SendTestPacket(value);

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }

        public static int PacketProcessUdpTest_2(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is User);
            Debug.Assert(Protocol.UDP_CLI_GW_TEST_2 == pkt.GetPacketID());

            User user = (User)obj;

            string value = pkt.GetString();
            LogManager.Info("Receive udp test 2 packet: " + value + ". UserID = " + user.UserID.ToString());

            user.SendTest_2_Packet(value);

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }


        private void SendAuthPacket()
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.CLI_GW_ENTER_AUTH);

            SendPacket(pkt);
        }

        private void SendTestPacket(int value)
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.CLI_GW_ENTER_TEST);
            pkt.AddInt(value);

            SendPacket(pkt);
            LogManager.Info("Send test packet.");
        }

        private void SendTest_2_Packet(string value)
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.CLI_GW_ENTER_TEST_2);
            pkt.AddString(value);

            SendPacket(pkt);
            LogManager.Info("Send test 2 packet.");
        }
    }
}
