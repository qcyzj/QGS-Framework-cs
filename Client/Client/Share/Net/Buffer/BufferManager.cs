using System;
using System.Diagnostics;
using System.Collections.Generic;

using Share.Net.Sessions;

namespace Share.Net.Buffer
{ 
    public sealed class BufferManager : Singleton<BufferManager>
    {
        private const int BUFFER_PER_SESSION = 4;

        private byte[][] m_Buffer;
        private Queue<byte[]> m_FreeBufferQueue;


        public BufferManager()
        {
            m_Buffer = null;
            m_FreeBufferQueue = new Queue<byte[]>();
        }


        public void Initialize()
        {
            int session_num = 
                (SessionManager.USER_TCP_SESSION_MAX_NUM + 
                    SessionManager.USER_UDP_SESSION_MAX_NUM) * 
                        BUFFER_PER_SESSION;

            m_Buffer = new byte[session_num][];

            for (int i = 0; i < session_num; ++i)
            {
                m_Buffer[i] = new byte[ReadWriteBuffer.BUFFER_MAX_SIZE];
                m_FreeBufferQueue.Enqueue(m_Buffer[i]);
            }

            ValidInitializeOnce();
        }

        public void Release()
        {
            ValidReleaseOnce();

            int session_num = 
                (SessionManager.USER_TCP_SESSION_MAX_NUM +
                    SessionManager.USER_UDP_SESSION_MAX_NUM) * 
                        BUFFER_PER_SESSION;

            m_FreeBufferQueue.Clear();

            for (int i = 0; i < session_num; ++i)
            {
                Array.Clear(m_Buffer[i], 0, ReadWriteBuffer.BUFFER_MAX_SIZE);

                m_Buffer[i] = null;
            }

            m_Buffer = null;
        }


        public byte[] AllocateBuffer()
        {
            Debug.Assert(m_FreeBufferQueue.Count > 0);
            return m_FreeBufferQueue.Dequeue();
        }


        public int GetFreeBufferCount()
        {
            return m_FreeBufferQueue.Count;
        }
    }
}
