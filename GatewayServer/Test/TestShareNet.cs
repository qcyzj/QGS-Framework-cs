using System;
using System.Reflection;
using System.Collections.Generic;

using Share;
using Share.Net.Buffer;
using Share.Net.Packets;
using Share.Net.Sessions;

using GatewayServer.Gateway;
using GatewayServer.Gateway.Users;

namespace GatewayServer.Test
{
    public class TestShareNet
    {
        public TestShareNet()
        { }

        public void RunAllTest()
        {
            Test_Share_Net_BufferManager();

            Test_Share_Net_Packet();

            Test_Share_Net_PacketEncryptManager();

            Test_Share_Net_PacketManager();

            //Test_Share_Net_PacketProcessManager();

            Test_Share_Net_ReadWriteBuffer();



            Test_Share_Net_ServerSession();

            Test_Share_Net_SessionManager();

            Test_Share_Net_UserSession();
        }


        private void Test_Share_Net_BufferManager()
        {
            CAssert.AreEqual(0, BufferManager.Instance.GetFreeBufferCount());
        }

        private void Test_Share_Net_Packet()
        {
            byte[] buffer = new byte[Packet.DEFAULT_PACKET_BUF_SIZE];

            Packet pkt = new Packet(buffer);
            pkt.Initialize();

            CAssert.AreEqual(buffer, pkt.Buf);
            CAssert.AreEqual((int)pkt.Size, Packet.PACKET_HEAD_LENGTH);

            pkt.SetPacketID(Protocol.CLI_GW_ENTER_TEST);
            CAssert.AreEqual(pkt.GetPacketID(), Protocol.CLI_GW_ENTER_TEST);

            int int_test = 10000001;
            uint uint_test = 88776655;
            short short_test = 10001;
            ushort ushort_test = 65530;
            long long_test = 9223372036854775805;
            double double_test = 123456789.987654321;
            float float_test = 12345.12345f;
            string str_test = "This is a testing string.";

            pkt.AddInt(int_test);
            pkt.AddUint(uint_test);
            pkt.AddShort(short_test);
            pkt.AddUshort(ushort_test);
            pkt.AddLong(long_test);
            pkt.AddDouble(double_test);
            pkt.AddFloat(float_test);
            pkt.AddString(str_test);

            int total_size = Packet.PACKET_HEAD_LENGTH + sizeof(int) + sizeof(uint) + sizeof(short) +
                             sizeof(ushort) +  sizeof(long) + sizeof(double) + sizeof(float) + 
                             sizeof(short) + str_test.Length;
            CAssert.AreEqual(total_size, (int)pkt.Size);

            pkt.ResetBufferIndex();

            int int_get = pkt.GetInt();
            CAssert.AreEqual(int_get, int_test);

            uint uint_get = pkt.GetUint();
            CAssert.AreEqual(uint_get, uint_test);

            short short_get = pkt.GetShort();
            CAssert.AreEqual(short_get, short_test);

            ushort ushort_get = pkt.GetUshort();
            CAssert.AreEqual(ushort_get, ushort_test);

            long long_get = pkt.GetLong();
            CAssert.AreEqual(long_get, long_test);

            double double_get = pkt.GetDouble();
            CAssert.AreEqual(double_get, double_test);

            float float_get = pkt.GetFloat();
            CAssert.AreEqual(float_get, float_test);

            string str_get = pkt.GetString();
            CAssert.AreEqual(str_get, str_test);

            pkt.Release();
            CAssert.IsNull(pkt.Buf);

            buffer = null;
        }

        private void Test_Share_Net_PacketEncryptManager()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                LogManager.Warn(MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private void Test_Share_Net_PacketManager()
        {
            List<Packet> pkt_list = new List<Packet>();
            Packet pkt = null;


            for (int i = 0; i < 1000; ++i)
            {
                pkt = PacketManager.Instance.AllocatePacket();
                CAssert.IsNotNull(pkt);

                PacketManager.Instance.ReleasePacket(pkt);
            }

            CAssert.AreEqual(1000, PacketManager.Instance.GetFreePacketCount());


            for (int i = 0; i < 1000; ++i)
            {
                pkt = PacketManager.Instance.AllocatePacket();
                CAssert.IsNotNull(pkt);

                pkt_list.Add(pkt);
            }

            CAssert.AreEqual(0, PacketManager.Instance.GetFreePacketCount());


            for (int i = 0; i < 200; ++i)
            {
                pkt = PacketManager.Instance.AllocatePacket();
                CAssert.IsNotNull(pkt);

                pkt_list.Add(pkt);
            }

            CAssert.AreEqual(0, PacketManager.Instance.GetFreePacketCount());


            for (int i = 0; i < 1200; ++i)
            {
                PacketManager.Instance.ReleasePacket(pkt_list[i]);
            }

            CAssert.AreEqual(1200, PacketManager.Instance.GetFreePacketCount());
        }

        private void Test_Share_Net_PacketProcessManager()
        {            
            User user = new User();
            CAssert.IsNotNull(user);

            byte[] buffer = new byte[Packet.DEFAULT_PACKET_BUF_SIZE];
            CAssert.IsNotNull(buffer);

            Packet pkt = new Packet(buffer);
            pkt.Initialize();

            pkt.SetPacketID(Protocol.CLI_GW_ENTER_TEST);
            int ret = PacketProcessManager.Instance.ProcessPacket(user, pkt);
            CAssert.AreEqual((int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS, ret);

            pkt.SetPacketID(Protocol.CLI_GW_ENTER_REGISTER);
            ret = PacketProcessManager.Instance.ProcessPacket(user, pkt);
            CAssert.AreEqual((int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS, ret);

            pkt.SetPacketID(Protocol.CLI_GW_ENTER_LOGIN);
            ret = PacketProcessManager.Instance.ProcessPacket(user, pkt);
            CAssert.AreEqual((int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS, ret);

            pkt.SetPacketID(Protocol.CLI_GW_ENTER_TEST_2);
            ret = PacketProcessManager.Instance.ProcessPacket(user, pkt);
            CAssert.AreEqual((int)PacketProcessManager.PACKET_PROC_ERROR.E_DEAL_FUNC_NOT_EXIST, ret);

            PacketProcessManager.Instance.Release();
        }

        private void Test_Share_Net_ReadWriteBuffer()
        {
            Packet pkt_orig = PacketManager.Instance.AllocatePacket();
            CAssert.IsNotNull(pkt_orig);

            pkt_orig.SetPacketID(Protocol.CLI_GW_ENTER_TEST);
            CAssert.AreEqual(pkt_orig.GetPacketID(), Protocol.CLI_GW_ENTER_TEST);

            int int_test = 10000001;
            short short_test = 10001;
            double double_test = 123456789.987654321;
            float float_test = 12345.12345f;
            string str_test = "This is a testing string.";

            pkt_orig.AddInt(int_test);
            pkt_orig.AddShort(short_test);
            pkt_orig.AddDouble(double_test);
            pkt_orig.AddFloat(float_test);
            pkt_orig.AddString(str_test);


            byte[] buffer = new byte[ReadWriteBuffer.BUFFER_MAX_SIZE];
            ReadWriteBuffer rw_buf = new ReadWriteBuffer(buffer);
            CAssert.IsNotNull(rw_buf);
            CAssert.AreEqual(0, rw_buf.GetCanReadSize());
            CAssert.AreEqual(ReadWriteBuffer.BUFFER_MAX_SIZE, rw_buf.GetCanWriteSize());

            rw_buf.WriteBytes(pkt_orig.Buf, pkt_orig.Size);
            CAssert.AreEqual(rw_buf.GetCanReadSize(), (int)pkt_orig.Size);

            Packet pkt_read = PacketManager.Instance.AllocatePacket();
            CAssert.IsNotNull(pkt_read);

            rw_buf.ReadBytes(pkt_read.Buf, rw_buf.GetCanReadSize());
            //rw_buf.Compact();

            pkt_read.SetSize();

            CAssert.AreEqual(0, rw_buf.GetCanReadSize());
            CAssert.AreEqual(ReadWriteBuffer.BUFFER_MAX_SIZE, rw_buf.GetCanWriteSize());

            CAssert.AreEqual(pkt_orig.Size, pkt_read.Size);
            CAssert.AreEqual(pkt_orig.Buf, pkt_read.Buf);

            int w_count = 0;

            while (rw_buf.GetCanWriteSize() >= pkt_orig.Size)
            {
                rw_buf.WriteBytes(pkt_orig.Buf, pkt_orig.Size);
                ++w_count;
            }

            CAssert.AreEqual(pkt_orig.Size * w_count, rw_buf.GetCanReadSize());

            int r_count = 0;

            while (rw_buf.GetCanReadSize() > 0)
            {
                Packet pkt_tmp = PacketManager.Instance.AllocatePacket();
                CAssert.IsNotNull(pkt_tmp);

                rw_buf.PeekPacketHead(pkt_tmp.Buf);
                pkt_tmp.SetSize();

                int pkt_size = pkt_tmp.Size;
                CAssert.IsTrue(rw_buf.GetCanReadSize() >= pkt_size);

                pkt_tmp.Initialize();
                rw_buf.ReadBytes(pkt_tmp.Buf, pkt_size);

                pkt_tmp.SetSize();

                ++r_count;

                CAssert.AreEqual(pkt_tmp.Size, pkt_orig.Size);
                CAssert.AreEqual(pkt_tmp.Buf, pkt_orig.Buf);
            }

            CAssert.AreEqual(r_count, w_count);

            CAssert.AreEqual(0, rw_buf.GetCanReadSize());
            CAssert.AreEqual(ReadWriteBuffer.BUFFER_MAX_SIZE, rw_buf.GetCanWriteSize());

            rw_buf.Release();
            CAssert.IsNull(rw_buf.Buffer);
        }



        private void Test_Share_Net_ServerSession()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                LogManager.Warn(MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private void Test_Share_Net_Session()
        {
            // abstract class;
        }

        private void Test_Share_Net_SessionManager()
        {
            Session sess = null;
            List<Session> sess_list = new List<Session>();

            for (int i = 0; i < SessionManager.USER_TCP_SESSION_MAX_NUM; ++i)
            {
                sess = SessionManager.Instance.AllocateTcpUserSession();
                CAssert.IsNotNull(sess);

                sess_list.Add(sess);
                sess = null;
            }

            CAssert.AreEqual(0, SessionManager.Instance.GetTcpUserSessionCount());

            for (int i = 0; i < sess_list.Count; ++i)
            {
                SessionManager.Instance.FreeTcpUserSession(sess_list[i]);
            }

            sess_list.Clear();

            CAssert.AreEqual(SessionManager.USER_TCP_SESSION_MAX_NUM,
                             SessionManager.Instance.GetTcpUserSessionCount());


            for (int i = 0; i < SessionManager.USER_UDP_SESSION_MAX_NUM; ++i)
            {
                sess = SessionManager.Instance.AllocateUdpUserSession();
                CAssert.IsNotNull(sess);

                sess_list.Add(sess);
                sess = null;
            }

            CAssert.AreEqual(0, SessionManager.Instance.GetUdpUserSessionCount());

            for (int i = 0; i < sess_list.Count; ++i)
            {
                SessionManager.Instance.FreeUdpUserSession(sess_list[i]);
            }

            sess_list.Clear();

            CAssert.AreEqual(SessionManager.USER_UDP_SESSION_MAX_NUM,
                             SessionManager.Instance.GetUdpUserSessionCount());


            for (int i = 0; i < SessionManager.SERVER_TCP_SESSION_MAX_NUM; ++i)
            {
                sess = SessionManager.Instance.AllocateServerSession();
                CAssert.IsNotNull(sess);

                sess_list.Add(sess);
            }

            CAssert.AreEqual(0, SessionManager.Instance.GetServerSessionCount());

            for (int i = 0; i < sess_list.Count; ++i)
            {
                SessionManager.Instance.FreeServerSession(sess_list[i]);
            }

            CAssert.AreEqual(SessionManager.SERVER_TCP_SESSION_MAX_NUM,
                             SessionManager.Instance.GetServerSessionCount());

            sess_list.Clear();
            sess_list = null;
        }

        private void Test_Share_Net_SocketServer()
        {
            // abstract class 
        }

        private void Test_Share_Net_UserSession()
        {
            //Session sess = SessionManager.Instance.AllocateUserSession();
            //CAssert.IsNotNull(sess);
            //CAssert.IsTrue(sess is UserSession);

            //UserSession user_sess = (UserSession)sess;
            //CAssert.IsNotNull(user_sess);

            //DateTime now = Time.GetUtcNow();

            //user_sess.ProcessHeartBeat(now);

            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                LogManager.Warn(MethodBase.GetCurrentMethod().Name, ex);
            }
        }
    }
}
