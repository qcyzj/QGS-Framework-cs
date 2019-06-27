using System;
using System.Diagnostics;

using Share.Net.Packets;

namespace Share.Net.Buffer
{
    public class ReadWriteBuffer
    {
        public const int BUFFER_MAX_SIZE = 8192;


        private byte[] m_Buffer;
        private int m_ReadIndex;
        private int m_WriteIndex;


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
            m_ReadIndex = m_WriteIndex = 0;
        }

        
        public void WriteBytes(byte[] buf, int buf_len)
        {
            WriteBytes(buf, 0, buf_len);
        }

        public void WriteBytes(byte[] buf, int off_set, int buf_len)
        {
            if (GetCanWriteSize() < buf_len)
            {
                Compact();
            }

            Debug.Assert(ValidWriteSize(buf_len));

            Array.Copy(buf, off_set, m_Buffer, m_WriteIndex, buf_len);
            m_WriteIndex += buf_len;
        }

        public void ReadBytes(byte[] buf, int buf_len)
        {
            Debug.Assert(ValidReadSize(buf_len));

            Array.Copy(m_Buffer, m_ReadIndex, buf, 0, buf_len);
            m_ReadIndex += buf_len;

            if (m_ReadIndex == m_WriteIndex)
            {
                SetEmpty();
            }
        }
        
        [Obsolete("This function is obsoleted, using PeekPacketSize() instead.", true)]
        public void PeekPacketHead(byte[] buf)
        {
            Debug.Assert(buf.Length >= Packet.PACKET_HEAD_LENGTH);
            Debug.Assert(ValidReadSize(Packet.PACKET_HEAD_LENGTH));

            Array.Copy(m_Buffer, m_ReadIndex, buf, 0, Packet.PACKET_HEAD_LENGTH);
        }

        public int PeekPacketSize()
        {
            ReadOnlySpan<byte> buf_arr = new ReadOnlySpan<byte>(m_Buffer, m_ReadIndex, 
                                                                Packet.PACKET_SIZE_LENGTH);
            return BitConverter.ToInt16(buf_arr);
        }

        private void Compact()
        {
            int read_size = GetCanReadSize();

            if (read_size > 0)
            {
                Array.Copy(m_Buffer, m_ReadIndex, m_Buffer, 0, read_size);
            }

            m_WriteIndex -= m_ReadIndex;
            m_ReadIndex = 0;
        }


        public int GetCanReadSize()
        {
            ValidIndex();
            return m_WriteIndex - m_ReadIndex;
        }

        public int GetCanWriteSize()
        {
            Debug.Assert(m_WriteIndex <= BUFFER_MAX_SIZE);
            return BUFFER_MAX_SIZE - m_WriteIndex;
        }


        private bool ValidWriteSize(int size)
        {
            ValidIndex();
            return m_WriteIndex + size <= BUFFER_MAX_SIZE;
        }

        private bool ValidReadSize(int size)
        {
            ValidIndex();
            return m_ReadIndex + size <= m_WriteIndex;
        }

        private void ValidIndex()
        {
            Debug.Assert(m_ReadIndex >= 0);
            Debug.Assert(m_WriteIndex >= m_ReadIndex);
            Debug.Assert(m_ReadIndex < BUFFER_MAX_SIZE);
            Debug.Assert(m_WriteIndex < BUFFER_MAX_SIZE);
        }
    }
}
