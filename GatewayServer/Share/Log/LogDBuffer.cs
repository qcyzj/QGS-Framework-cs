using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace Share
{
    public sealed class LogDBuffer
    {
        private enum BUFFER_ID
        {
            INVALID = 0,
            BUF_FIRST,
            BUF_SECOND,
        }


        private LogBuf m_WritingBuffer;
        private LogBuf m_ReadingBuffer;
        //private BUFFER_ID m_WriteBufID;
        private volatile bool m_BufferCanRead;
        private bool m_ServiceWillStop;


        public LogDBuffer()
        {
            m_WritingBuffer = new LogBuf();
            m_ReadingBuffer = new LogBuf();

            //m_WriteBufID = BUFFER_ID.BUF_FIRST;
            m_BufferCanRead = false;
            m_ServiceWillStop = false;
        }


        public void WriteLog(string log)
        {
            // 多线程写log buffer
            //LogBuf write_buf = BUFFER_ID.BUF_FIRST == m_WriteBufID ? m_BufferFirst : m_BufferSecond;
            //write_buf.Add(log);
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

            //LogBuf write_buf = BUFFER_ID.BUF_FIRST == m_WriteBufID ? m_BufferFirst : m_BufferSecond;
            //LogBuf read_buf = BUFFER_ID.BUF_FIRST == m_WriteBufID ? m_BufferSecond : m_BufferFirst;
            byte[] log_array = null;

            if (0 == m_ReadingBuffer.Count())
            {
                if (m_BufferCanRead || m_ServiceWillStop)
                {
                    SwapBuffer(m_WritingBuffer, m_ReadingBuffer);

                    m_BufferCanRead = false;

                    Debug.Assert(0 == m_WritingBuffer.Count());
                }

                if (m_ReadingBuffer.Count() > 0)
                {
                    m_ReadingBuffer.Collect(tmp_sb);
                    m_ReadingBuffer.Clear();

                    log_array = Encoding.Default.GetBytes(tmp_sb.ToString());
                    has_log = true;
                }
                else
                {
                    has_log = false;
                }
            }

            return log_array;
        }

        private static void SwapBuffer(LogBuf write_buf, LogBuf read_buf)
        {
            read_buf = Interlocked.Exchange(ref write_buf, read_buf);

            //if (BUFFER_ID.BUF_FIRST == m_WriteBufID)
            //{
            //    m_WriteBufID = BUFFER_ID.BUF_SECOND;
            //}
            //else
            //{
            //    m_WriteBufID = BUFFER_ID.BUF_FIRST;
            //}
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
            private const int LOG_BUF_INVALID_INDEX = -1;
            private const int LOG_BUF_SIZE_MAX = 16;
            private const int LOG_BUF_CAN_READ_SIZE = 12;


            private List<string> m_BufferList;
            private int m_BufIndex;


            public LogBuf()
            {
                m_BufferList = new List<string>(LOG_BUF_SIZE_MAX << 1);
                m_BufIndex = LOG_BUF_INVALID_INDEX;
            }

            public void Add(string log)
            {
                int ret_index = Interlocked.Increment(ref m_BufIndex);
                m_BufferList.Insert(ret_index, log);
            }

            public void Clear()
            {
                Interlocked.Exchange(ref m_BufIndex, LOG_BUF_INVALID_INDEX);
                m_BufferList.Clear();
            }

            public void Collect(StringBuilder tmp_sb)
            {
                foreach (string log in m_BufferList)
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
