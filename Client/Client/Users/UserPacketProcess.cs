using System.Diagnostics;

using Share;
using Share.Net.Packets;

namespace Client.Users
{
    public partial class User
    {
        public static int PacketPocessAuth(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is User);
            Debug.Assert(Protocol.CLI_GW_ENTER_AUTH == pkt.GetPacketID());

            User user = (User)obj;

            LogManager.Debug("Receive auth packet:  UserID = " + user.UserID.ToString());

            UserManager.Instance.AddAuthedUser(user);

            user.SendTestPacket();

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }

        public static int PacketProcessTest(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is User);
            Debug.Assert(Protocol.CLI_GW_ENTER_TEST == pkt.GetPacketID());

            User user = (User)obj;

            int value = pkt.GetInt();
            Debug.Assert(123456789 == value);
            LogManager.Debug("Receive test packet: " + value.ToString() + ". UserID = " + user.UserID.ToString());

            user.SendTest_2_Packet();

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }

        public static int PacketProcessTest_2(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is User);
            Debug.Assert(Protocol.CLI_GW_ENTER_TEST_2 == pkt.GetPacketID());

            User user = (User)obj;

            string value = pkt.GetString();
            LogManager.Debug("Receive test 2 packet: " + value + ". UserID = " + user.UserID.ToString());

            user.SendTestPacket();

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


        public void SendAuthPacket()
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.CLI_GW_ENTER_AUTH);

            pkt.AddUint(this.m_UserID);

            SendPacket(pkt);
            LogManager.Info("Send auth packet. UserID = " + this.UserID.ToString());
        }

        private void SendTestPacket()
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.CLI_GW_ENTER_TEST);
            pkt.AddInt(123456789);

            SendPacket(pkt);
            LogManager.Info("Send test packet. UserID = " + this.UserID.ToString());
        }

        private void SendTest_2_Packet()
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.CLI_GW_ENTER_TEST_2);
            pkt.AddString("this is test 2 string.");

            SendPacket(pkt);
            LogManager.Info("Send test 2 packet. UserID = " + this.UserID.ToString());
        }



        public static int PacketProcessUdpAuth(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is User);
            Debug.Assert(Protocol.UDP_CLI_GW_AUTH == pkt.GetPacketID());

            User user = (User)obj;

            int remote_port = pkt.GetInt();
            LogManager.Debug("Receive udp auth packet:  Remote port = " + remote_port.ToString() + 
                             " UserID = " + user.UserID.ToString());

            UserManager.Instance.AddConnectlessUser(user);
            user.SetRemotePort(remote_port);

            user.SendUdpAuthPacket();

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }

        public static int PacketProcessUdpTest(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is User);
            Debug.Assert(Protocol.UDP_CLI_GW_TEST == pkt.GetPacketID());

            User user = (User)obj;

            int value = pkt.GetInt();
            Debug.Assert(987654321 == value);
            LogManager.Debug("Receive udp test packet: " + value.ToString() + ". UserID = " + user.UserID.ToString());

            user.SendUdpTestPacket(value);

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }

        public static int PacketProcessUdpTest_2(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is User);
            Debug.Assert(Protocol.UDP_CLI_GW_TEST_2 == pkt.GetPacketID());

            User user = (User)obj;

            string value = pkt.GetString();
            LogManager.Debug("Receive udp test 2 packet: " + value + ". UserID = " + user.UserID.ToString());

            user.SendUdpTest_2_Packet(value);

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }


        public void SendUdpConnectPacket()
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.UDP_CLI_GW_CONNECT);

            SendPacket(pkt);
            LogManager.Info("Send udp connect packet. UserID = " + this.m_UserID.ToString());
        }

        private void SendUdpAuthPacket()
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.UDP_CLI_GW_AUTH);

            pkt.AddUint(this.m_UserID);
            SendPacket(pkt);

            LogManager.Info("Send udp auth packet. UserID = " + this.m_UserID.ToString());
        }

        private void SendUdpTestPacket(int value)
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.UDP_CLI_GW_TEST);
            pkt.AddInt(value);

            SendPacket(pkt);
            LogManager.Info("Send udp test packet. UserID = " + this.UserID.ToString());
        }

        private void SendUdpTest_2_Packet(string value)
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.UDP_CLI_GW_TEST_2);
            pkt.AddString(value);

            SendPacket(pkt);
            LogManager.Info("Send udp test 2 packet. UserID = " + this.UserID.ToString());
        }
    }
}
