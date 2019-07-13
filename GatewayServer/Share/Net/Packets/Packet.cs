using System;
using System.Text;
using System.Diagnostics;

using Share.Json;
using Share.Net.Message;

namespace Share.Net.Packets
{
    // |----------|-------------|---------|---------------|----------------|
    //  包长度（2）  包编号（4）   保留（4）   消息类型（1）      消息内容   
    //
    public class Packet
    {
        public  const int DEFAULT_PACKET_BUF_SIZE = 8192;
        private const int PACKET_SIZE_START = 0;
        public  const int PACKET_SIZE_LENGTH = 2;
        private const int PACKET_ID_START = PACKET_SIZE_LENGTH;
        private const int PACKET_ID_LENGTH = 4;
        private const int PACKET_PRESERVE_START = PACKET_ID_START + PACKET_ID_LENGTH;
        private const int PACKET_PRESERVE_LENGTH = 4;
        private const int PACKET_TYPE_START = PACKET_PRESERVE_START + PACKET_PRESERVE_LENGTH;
        private const int PACKET_TYPE_LENGTH = 1;
        public  const int PACKET_HEAD_LENGTH = PACKET_TYPE_START + PACKET_TYPE_LENGTH;

        private enum MESSAGE_TYPE : byte
        {
            PROTO_BUF = 0,
            JSON_DATA = 1,
            CUSTOM_MSG = 2,
        }


        private byte[] m_Buffer;
        private short m_PacketSize;
        private short m_BufferIndex;


        public byte[] Buf { get { return m_Buffer; } }
        public short Size { get{ return m_PacketSize; } }


        public Packet(byte[] buffer)
        {
            m_Buffer = buffer;
        }


        public void Initialize()
        {
            Array.Clear(m_Buffer, 0, m_Buffer.Length);
            m_PacketSize = 0;
            m_BufferIndex = PACKET_HEAD_LENGTH;

            AddDefaultSize();
        }

        public void Release()
        {
            m_Buffer = null;
            m_PacketSize = 0;
            m_BufferIndex = 0;
        }


        private void AddDefaultSize()
        {
            AddSize(PACKET_HEAD_LENGTH);
        }

        private void AddSize(short size)
        {
            m_PacketSize += size;
            Array.Copy(BitConverter.GetBytes(m_PacketSize), m_Buffer, PACKET_SIZE_LENGTH);
        }

        public void SetSize()
        {
            Debug.Assert(Valid());
            m_PacketSize = BitConverter.ToInt16(m_Buffer, 0);
        }


        public void SetPacketID(int packet_id)
        {
            Debug.Assert(packet_id > 0);
            Array.Copy(BitConverter.GetBytes(packet_id), 0, m_Buffer, PACKET_ID_START, PACKET_ID_LENGTH);
        }

        public int GetPacketID()
        {
            return BitConverter.ToInt32(m_Buffer, PACKET_ID_START);
        }


        private void SetMessageType(MESSAGE_TYPE m_type)
        {
            Span<byte> type = new Span<byte>(m_Buffer, PACKET_TYPE_START, 
                                             PACKET_TYPE_LENGTH);
            type[0] = (byte)m_type;
        }

        private byte GetMessageType()
        {
            ReadOnlySpan<byte> type = new ReadOnlySpan<byte>(m_Buffer, PACKET_TYPE_START,
                                                             PACKET_TYPE_LENGTH);
            return type[0];
        }


        public void ResetBufferIndex()
        {
            m_BufferIndex = PACKET_HEAD_LENGTH;
        }
        

        private bool ValidSize(int size)
        {
            return m_BufferIndex + size < DEFAULT_PACKET_BUF_SIZE;
        }

        public bool Valid()
        {
            if ((m_PacketSize <= DEFAULT_PACKET_BUF_SIZE) &&
                (PACKET_HEAD_LENGTH == m_BufferIndex) &&
                (0 != GetPacketID()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public Packet AddInt(int value)
        {
            short size = (short)sizeof(int);
            Debug.Assert(ValidSize(size));

            Array.Copy(BitConverter.GetBytes(value), 0, m_Buffer, m_BufferIndex, size);
            m_BufferIndex += size;

            AddSize(size);
            return this;
        }

        public Packet AddUint(uint value)
        {
            short size = (short)sizeof(uint);
            Debug.Assert(ValidSize(size));

            Array.Copy(BitConverter.GetBytes(value), 0, m_Buffer, m_BufferIndex, size);
            m_BufferIndex += size;

            AddSize(size);
            return this;
        }

        public Packet AddShort(short value)
        {
            short size = (short)sizeof(short);
            Debug.Assert(ValidSize(size));

            Array.Copy(BitConverter.GetBytes(value), 0, m_Buffer, m_BufferIndex, size);
            m_BufferIndex += size;

            AddSize(size);
            return this;
        }

        public Packet AddUshort(ushort value)
        {
            short size = (short)sizeof(ushort);
            Debug.Assert(ValidSize(size));

            Array.Copy(BitConverter.GetBytes(value), 0, m_Buffer, m_BufferIndex, size);
            m_BufferIndex += size;

            AddSize(size);
            return this;
        }

        public Packet AddLong(long value)
        {
            short size = (short)sizeof(long);
            Debug.Assert(ValidSize(size));

            Array.Copy(BitConverter.GetBytes(value), 0, m_Buffer, m_BufferIndex, size);
            m_BufferIndex += size;

            AddSize(size);
            return this;
        }

        public Packet AddDouble(double value)
        {
            //value = Math.Round(value, 3);

            short size = (short)sizeof(double);
            Debug.Assert(ValidSize(size));

            Array.Copy(BitConverter.GetBytes(value), 0, m_Buffer, m_BufferIndex, size);
            m_BufferIndex += size;

            AddSize(size);
            return this;
        }

        public Packet AddFloat(float value)
        {
            //value = (float)Math.Round(value, 3);

            short size = (short)sizeof(float);
            Debug.Assert(ValidSize(size));

            Array.Copy(BitConverter.GetBytes(value), 0, m_Buffer, m_BufferIndex, size);
            m_BufferIndex += size;

            AddSize(size);
            return this;
        }

        public Packet AddString(string str)
        {
            byte[] str_arry = Encoding.Default.GetBytes(str);

            short size = (short)str_arry.Length;
            Debug.Assert(ValidSize(size));

            AddShort(size);

            Array.Copy(str_arry, 0, m_Buffer, m_BufferIndex, size);
            m_BufferIndex += size;

            AddSize(size);
            return this;
        }

        public int GetInt()
        {
            short size = (short)sizeof(int);
            Debug.Assert(ValidSize(size));

            int value = BitConverter.ToInt32(m_Buffer, m_BufferIndex);
            m_BufferIndex += size;

            return value;
        }

        public uint GetUint()
        {
            short size = (short)sizeof(uint);
            Debug.Assert(ValidSize(size));

            uint value = BitConverter.ToUInt32(m_Buffer, m_BufferIndex);
            m_BufferIndex += size;

            return value;
        }

        public short GetShort()
        {
            short size = (short)sizeof(short);
            Debug.Assert(ValidSize(size));

            short value = BitConverter.ToInt16(m_Buffer, m_BufferIndex);
            m_BufferIndex += size;

            return value;
        }

        public ushort GetUshort()
        {
            short size = (short)sizeof(ushort);
            Debug.Assert(ValidSize(size));

            ushort value = BitConverter.ToUInt16(m_Buffer, m_BufferIndex);
            m_BufferIndex += size;

            return value;
        }

        public long GetLong()
        {
            short size = (short)sizeof(long);
            Debug.Assert(ValidSize(size));

            long value = BitConverter.ToInt64(m_Buffer, m_BufferIndex);
            m_BufferIndex += size;

            return value;
        }

        public double GetDouble()
        {
            short size = (short)sizeof(double);
            Debug.Assert(ValidSize(size));

            double value = BitConverter.ToDouble(m_Buffer, m_BufferIndex);
            m_BufferIndex += size;

            return value;
        }

        public float GetFloat()
        {
            short size = (short)sizeof(float);
            Debug.Assert(ValidSize(size));

            float value = BitConverter.ToSingle(m_Buffer, m_BufferIndex);
            m_BufferIndex += size;

            return value;
        }

        public string GetString()
        {
            short str_length = GetShort();

            string str = Encoding.Default.GetString(m_Buffer, m_BufferIndex, str_length);
            m_BufferIndex += str_length;

            return str;
        }


        public Packet AddJsonData(JsonData json)
        {
            SetMessageType(MESSAGE_TYPE.JSON_DATA);
            return AddString(json.ToString());
        }

        public JsonData GetJsonData()
        {
            Debug.Assert((byte)MESSAGE_TYPE.JSON_DATA == GetMessageType());
            string str_msg = GetString();
            return new JsonData(str_msg)
        }


        public Packet AddProtoBuf(byte[] buf)
        {


            return this;
        }

        public void GetProtoBuf()
        {

        }
    }
}
