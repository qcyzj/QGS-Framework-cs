using System.Diagnostics;

using Share.Net.Buffer;
using Share.Net.Packets;
using Share.Net.Sessions;

using GatewayServer.Gateway.Users;

namespace GatewayServer.Test
{
    public class TestMain
    {
        private TestShare m_Share;
        private TestShareNet m_ShareNet;
        private TestGatewayUser m_GatewayUser;

        public TestMain()
        {
            m_Share = new TestShare();
            m_ShareNet = new TestShareNet();
            m_GatewayUser = new TestGatewayUser();
        }

        public void RunAllTest()
        {
            BufferManager.Instance.Initialize();

            PacketManager.Instance.Initialize();

            SocketAsyncEventArgsManager.Instance.Initialize();

            SessionManager.Instance.Initialize();

            UserManager.Instance.Initialize();


            m_Share.RunAllTest();

            m_ShareNet.RunAllTest();

            m_GatewayUser.RunAllTest();




            UserManager.Instance.Release();

            SessionManager.Instance.Release();

            SocketAsyncEventArgsManager.Instance.Release();

            PacketManager.Instance.Release();

            BufferManager.Instance.Release();


            Process.GetCurrentProcess().Kill();
        }
    }
}
