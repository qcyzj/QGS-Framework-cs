using System.Net;
using System.Diagnostics;

using Share.Logs;
using Share.Net.Packets;
using Share.Net.Sessions;

namespace GatewayServer.Gateway.Users
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



        public static int PaacketProcessUdpConnect(object obj, Packet pkt)
        {
            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }

        public static int PacketProcessUdpAuth(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is User);
            Debug.Assert(Protocol.UDP_CLI_GW_AUTH == pkt.GetPacketID());

            User user = (User)obj;

            uint user_id = pkt.GetUint();
            user.SetUserID(user_id);

            UserManager.Instance.AddConnectlessUser(user);

            LogManager.Info("Receive udp auth packet: UserID = " + user.UserID.ToString());

            user.SendUdpTestPacket();

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
            LogManager.Info("Receive udp test packet: " + value.ToString() + ", UserID = " + user.UserID.ToString());

            user.SendUdpTest_2_Packet();

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }

        public static int PacketProcessUdpTest_2(object obj, Packet pkt)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is User);
            Debug.Assert(Protocol.UDP_CLI_GW_TEST_2 == pkt.GetPacketID());

            User user = (User)obj;

            string value = pkt.GetString();
            Debug.Assert("this is udp test 2 string." == value);
            LogManager.Info("Receive udp test 2 packet: " + value + ". UserID = " + user.UserID.ToString());

            user.SendUdpTestPacket();

            return (int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS;
        }


        public void SendUdpAuthPacket()
        {
            Debug.Assert(null != m_UserSession);
            Debug.Assert(m_UserSession is UdpSession);

            UdpSession sess = m_UserSession as UdpSession;
            IPEndPoint end_point = (IPEndPoint)sess.Socket.LocalEndPoint;

            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.UDP_CLI_GW_AUTH);
            pkt.AddInt(end_point.Port);

            SendPacket(pkt);

            LogManager.Info("Send udp auth packet. Local port = " + end_point.Port.ToString() + 
                            " UserID = " + this.m_UserID.ToString());
        }

        private void SendUdpTestPacket()
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.UDP_CLI_GW_TEST);
            pkt.AddInt(987654321);

            SendPacket(pkt);
            LogManager.Info("Send udp test packet. UserID = "  + this.m_UserID.ToString());
        }

        private void SendUdpTest_2_Packet()
        {
            Packet pkt = PacketManager.Instance.AllocatePacket();
            pkt.SetPacketID(Protocol.UDP_CLI_GW_TEST_2);
            pkt.AddString("this is udp test 2 string.");

            SendPacket(pkt);
            LogManager.Info("Send udp test 2 packet. UserID = " + this.m_UserID.ToString());
        }
    }
}
