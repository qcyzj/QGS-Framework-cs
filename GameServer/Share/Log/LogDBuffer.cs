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
        private ManualResetEventSlim m_WriteEvent;
        private ManualResetEventSlim m_ReadEvent;
        private AutoResetEvent m_AutoEvent;


        public LogDBuffer()
        {
            m_WritingBuffer = new LogBuf();
            m_ReadingBuffer = new LogBuf();

            m_WriteEvent = new ManualResetEventSlim(false);
            m_ReadEvent = new ManualResetEventSlim(true);
            m_AutoEvent = new AutoResetEvent(false);
        }


        public void WriteLog(string log)
        {
            // 多线程写log buffer

            m_ReadEvent.Wait();
            m_WriteEvent.Reset();

            m_WritingBuffer.Add(log);

            m_WriteEvent.Set();

            if (m_WritingBuffer.CanRead() && 0 == m_ReadingBuffer.Count())
            {
                m_AutoEvent.Set();
            }
        }

        public byte[] ReadLog(StringBuilder tmp_sb, ref bool has_log)
        {
            // 单线程读log buffer

            tmp_sb.Clear();
            has_log = false;
            byte[] log_array = null;

            m_AutoEvent.WaitOne();

            m_ReadEvent.Reset();
            m_WriteEvent.Wait();

            Debug.Assert(0 == m_ReadingBuffer.Count());
            m_ReadingBuffer = Interlocked.Exchange(ref m_WritingBuffer, m_ReadingBuffer);
            Debug.Assert(0 == m_WritingBuffer.Count());

            m_ReadEvent.Set();

            if (m_ReadingBuffer.Count() > 0)
            {
                m_ReadingBuffer.Collect(tmp_sb);

                log_array = Encoding.Default.GetBytes(tmp_sb.ToString());
                has_log = true;
            }

            return log_array;
        }       

        public bool HasLog()
        {
            return m_WritingBuffer.Count() + m_ReadingBuffer.Count() > 0;
        }

        public void SetServiceStop()
        {
            m_AutoEvent.Set();
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
