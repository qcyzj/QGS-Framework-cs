using System.Diagnostics;
using System.Collections.Generic;

namespace Share.Net.Server
{
    public class UdpPortManager : Singleton<UdpPortManager>
    {
        private const int UDP_PORT_MIN = 20000;
        private const int UDP_PORT_MAX = 30000;
        public const int INVALID_UDP_PORT = 0;


        private Stack<int> m_PortStack;
        private object m_PortLock;


        public UdpPortManager()
        {
            m_PortStack = new Stack<int>();
            m_PortLock = new object();
        }


        public void Initialize()
        {
            for (int i = UDP_PORT_MIN; i <= UDP_PORT_MAX; ++i)
            {
                m_PortStack.Push(i);
            }

            ValidInitializeOnce();
        }

        public void Release()
        {
            ValidReleaseOnce();

            m_PortStack.Clear();
            m_PortStack = null;
        }


        public int AllocatePort()
        {
            int port = INVALID_UDP_PORT;

            lock (m_PortLock)
            {
                if (m_PortStack.Count > 0)
                {
                    port = m_PortStack.Pop();
                }
            }

            return port;
        }

        public void FreePort(int port)
        {
            Debug.Assert(INVALID_UDP_PORT != port);

            lock (m_PortLock)
            {
                m_PortStack.Push(port);
            }
        }
    }
}
