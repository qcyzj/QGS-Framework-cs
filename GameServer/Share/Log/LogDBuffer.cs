using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace Share
{
    public sealed class LogDBuffer
    {
        private LogBuf m_WritingBuffer;
        private LogBuf m_ReadingBuffer;
        private volatile bool m_BufferCanRead;
        private bool m_ServiceWillStop;


        public LogDBuffer()
        {
            m_WritingBuffer = new LogBuf();
            m_ReadingBuffer = new LogBuf();

            m_BufferCanRead = false;
            m_ServiceWillStop = false;
        }


        public void WriteLog(string log)
        {
            // 多线程写log buffer
            m_WritingBuffer.Add(log);

            if (!m_BufferCanRead && m_WritingBuffer.CanRead())
            {
                m_BufferCanRead = true;
            }
        }

        public byte[] ReadLog(StringBuilder tmp_sb, ref bool has_log)
        {
            // 单线程读log buffer
            tmp_sb.Clear();
            has_log = false;

            byte[] log_array = null;

            if (0 == m_ReadingBuffer.Count())
            {
                if (m_BufferCanRead || m_ServiceWillStop)
                {
                    m_ReadingBuffer = Interlocked.Exchange(ref m_WritingBuffer, m_ReadingBuffer);
                    m_BufferCanRead = false;

                    Debug.Assert(0 == m_WritingBuffer.Count());
                }

                if (m_ReadingBuffer.Count() > 0)
                {
                    m_ReadingBuffer.Collect(tmp_sb);

                    log_array = Encoding.Default.GetBytes(tmp_sb.ToString());
                    has_log = true;
                }
            }

            return log_array;
        }       

        public bool HasLog()
        {
            return m_WritingBuffer.Count() + m_ReadingBuffer.Count() > 0;
        }

        public void SetServiceStop()
        {
            m_ServiceWillStop = true;
        }


        private class LogBuf
        {
            private const int LOG_BUF_CAN_READ_SIZE = 16;


            private ConcurrentQueue<string> m_BufferList;


            public LogBuf()
            {
                m_BufferList = new ConcurrentQueue<string>();
            }

            public void Add(string log)
            {
                m_BufferList.Enqueue(log);
            }

            public void Collect(StringBuilder tmp_sb)
            {
                while (m_BufferList.TryDequeue(out string log))
                {
                    tmp_sb.Append(log);
                }
            }

            public int Count()
            {
                return m_BufferList.Count;
            }

            public bool CanRead()
            {
                return m_BufferList.Count > LOG_BUF_CAN_READ_SIZE;
            }
        }
    }   
}
