using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Share.Net.Packets
{
    public class PacketManager : Singleton<PacketManager>
    {
        private const int DEFAULT_ALLOCATE_PACKET_NUM = 1000;
        private const int DYNAMIC_ALLOCATE_PACKET_NUM = 100;


        private List<byte[][]> m_BufferList;
        private ConcurrentQueue<Packet> m_PacketQueue;


        public PacketManager()
        {
            m_BufferList = new List<byte[][]>();
            m_PacketQueue = new ConcurrentQueue<Packet>();
        }


        public void Initialize()
        {
            DynamicAllocatePacket(DEFAULT_ALLOCATE_PACKET_NUM);

            ValidInitializeOnce();
        }

        public void Release()
        {
            ValidReleaseOnce();

            while (m_PacketQueue.TryDequeue(out Packet pkt))
            {
                pkt.Release();
                pkt = null;
            }

            for (int i = 0; i < m_BufferList.Count; ++i)
            {
                byte[][] buffer_array = m_BufferList[i];

                if (0 == i)
                {
                    for (int j = 0; j < DEFAULT_ALLOCATE_PACKET_NUM; ++j)
                    {
                        Array.Clear(buffer_array[j], 0, Packet.DEFAULT_PACKET_BUF_SIZE);
                        buffer_array[j] = null;
                    }
                }
                else
                {
                    for (int j = 0; j < DYNAMIC_ALLOCATE_PACKET_NUM; ++j)
                    {
                        Array.Clear(buffer_array[j], 0, Packet.DEFAULT_PACKET_BUF_SIZE);
                        buffer_array[j] = null;
                    }
                }

                buffer_array = null;
            }

            m_BufferList.Clear();
        }


        public Packet AllocatePacket()
        {
            if (m_PacketQueue.TryDequeue(out Packet pkt))
            {}
            else
            {
                DynamicAllocatePacket(DYNAMIC_ALLOCATE_PACKET_NUM);
                Debug.Assert(m_PacketQueue.Count > 0);

                m_PacketQueue.TryDequeue(out pkt);
            }

            pkt.Initialize();
            return pkt;
        }

        public void ReleasePacket(Packet pkt)
        {
            if (null == pkt)
            {
                Debug.Assert(false);
                return;
            }
            
            m_PacketQueue.Enqueue(pkt);
        }


        private void DynamicAllocatePacket(int packet_num)
        {
            Packet pkt = null;

            byte[][] buffer_array = new byte[packet_num][];

            for (int i = 0; i < packet_num; ++i)
            {
                buffer_array[i] = new byte[Packet.DEFAULT_PACKET_BUF_SIZE];

                pkt = new Packet(buffer_array[i]);
                Debug.Assert(null != pkt);

                m_PacketQueue.Enqueue(pkt);
            }

            m_BufferList.Add(buffer_array);
        }


        public int GetFreePacketCount()
        {
            return m_PacketQueue.Count;
        }
    }
}
