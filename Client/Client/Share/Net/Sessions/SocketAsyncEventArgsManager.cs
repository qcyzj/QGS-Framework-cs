using System.Net.Sockets;
using System.Collections.Generic;

using Share;
using Share.Net.Buffer;

namespace Share.Net.Sessions
{
    public sealed class SocketAsyncEventArgsManager : Singleton<SocketAsyncEventArgsManager>
    {
        private Queue<SocketAsyncEventArgs> m_EventQueue;


        public SocketAsyncEventArgsManager()
        {
            m_EventQueue = new Queue<SocketAsyncEventArgs>();
        }


        public void Initialize()
        {
            int event_args_num = 
                (SessionManager.USER_TCP_SESSION_MAX_NUM + SessionManager.USER_UDP_SESSION_MAX_NUM) * 2;

            SocketAsyncEventArgs event_args = null;

            for (int i = 0; i < event_args_num; ++i)
            {
                event_args = new SocketAsyncEventArgs();
                event_args.SetBuffer(BufferManager.Instance.AllocateBuffer(), 0, ReadWriteBuffer.BUFFER_MAX_SIZE);

                m_EventQueue.Enqueue(event_args);
            }

            ValidInitializeOnce();
        }

        public void Release()
        {
            ValidReleaseOnce();

            m_EventQueue.Clear();
            m_EventQueue = null;
        }


        public SocketAsyncEventArgs AllocateEventArgs()
        {
            if (m_EventQueue.Count > 0)
            {
                return m_EventQueue.Dequeue();
            }
            else
            {
                return null;
            }
        }
    }
}
