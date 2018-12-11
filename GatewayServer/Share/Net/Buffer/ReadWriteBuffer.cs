using System;
using System.Diagnostics;

using Share.Net.Packets;

namespace Share.Net.Buffer
{
    public class ReadWriteBuffer
    {
        public const int BUFFER_MAX_SIZE = 20480;


        private byte[] m_Buffer;
        private int m_BufReadIndex;
        private int m_BufWriteIndex;


        public byte[] Buffer { get { return m_Buffer; } }
        public int ReadIndex { get { return m_BufReadIndex; } }
        public int WriteIndex { get { return m_BufWriteIndex; } }


        public ReadWriteBuffer(byte[] buffer)
        {
            Debug.Assert(null != buffer);
            m_Buffer = buffer;
            SetEmpty();
        }


        public void Release()
        {
            m_Buffer = null;
            SetEmpty();
        }

        public void SetEmpty()
        {
            m_BufReadIndex = m_BufWriteIndex = 0;
        }

        
        public void WriteBytes(byte[] buf, int buf_len)
        {
            if (GetCanWriteSize() < buf_len)
            {
                Compact();
            }

            Debug.Assert(ValidWriteSize(buf_len));

            Array.Copy(buf, 0, m_Buffer, m_BufWriteIndex, buf_len);
            m_BufWriteIndex += buf_len;
        }

        public void ReadBytes(byte[] buf, int buf_len)
        {
            Debug.Assert(ValidReadSize(buf_len));

            Array.Copy(m_Buffer, m_BufReadIndex, buf, 0, buf_len);
            m_BufReadIndex += buf_len;

            if (m_BufReadIndex == m_BufWriteIndex)
            {
                SetEmpty();
            }
        }

        public void PeekPacketHead(byte[] buf)
        {
            Debug.Assert(buf.Length >= Packet.PACKET_HEAD_LENGTH);
            Debug.Assert(ValidReadSize(Packet.PACKET_HEAD_LENGTH));

            Array.Copy(m_Buffer, m_BufReadIndex, buf, 0, Packet.PACKET_HEAD_LENGTH);
        }

        private void Compact()
        {
            int read_size = GetCanReadSize();

            if (read_size > 0)
            {
                Array.Copy(m_Buffer, m_BufReadIndex, m_Buffer, 0, read_size);
            }

            m_BufWriteIndex -= m_BufReadIndex;
            m_BufReadIndex = 0;
        }


        public int GetCanReadSize()
        {
            ValidIndex();
            return m_BufWriteIndex - m_BufReadIndex;
        }

        public int GetCanWriteSize()
        {
            Debug.Assert(m_BufWriteIndex <= BUFFER_MAX_SIZE);
            return BUFFER_MAX_SIZE - m_BufWriteIndex;
        }

        public void AddReadSize(int size)
        {
            Debug.Assert(ValidReadSize(size));
            m_BufReadIndex += size;
        }

        public void AddWriteSize(int size)
        {
            Debug.Assert(ValidWriteSize(size));
            m_BufWriteIndex += size;
        }


        private bool ValidWriteSize(int size)
        {
            ValidIndex();
            return m_BufWriteIndex + size < BUFFER_MAX_SIZE;
        }

        private bool ValidReadSize(int size)
        {
            ValidIndex();
            return m_BufReadIndex + size <= m_BufWriteIndex;
        }

        private void ValidIndex()
        {
            Debug.Assert(m_BufReadIndex >= 0);
            Debug.Assert(m_BufWriteIndex >= m_BufReadIndex);
        }
    }
}
