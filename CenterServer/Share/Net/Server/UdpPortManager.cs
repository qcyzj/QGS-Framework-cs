using System.Diagnostics;
using System.Collections.Concurrent;

namespace Share.Net.Server
{
    public class UdpPortManager : Singleton<UdpPortManager>
    {
        private const int UDP_PORT_MIN = 20000;
        private const int UDP_PORT_MAX = 30000;
        public const int INVALID_UDP_PORT = 0;


        private ConcurrentStack<int> m_PortStack;


        public UdpPortManager()
        {
            m_PortStack = new ConcurrentStack<int>();
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
            if (m_PortStack.TryPop(out int port))
            {}
            else
            {
                port = INVALID_UDP_PORT;
            }

            return port;
        }

        public void DeallocatePort(int port)
        {
            Debug.Assert(INVALID_UDP_PORT != port);
            m_PortStack.Push(port);
        }
    }
}
