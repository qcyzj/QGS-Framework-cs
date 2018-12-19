using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Share.Net.Packets
{
    public class PacketManager : Singleton<PacketManager>
    {
        private const int DEFAULT_ALLOCATE_PACKET_NUM = 1000;
        private const int DYNAMIC_ALLOCATE_PACKET_NUM = 100;


        private List<byte[][]> m_BufferList;
        private Queue<Packet> m_PacketQueue;
        private object m_QueueLock;


        public PacketManager()
        {
            m_BufferList = new List<byte[][]>();
            m_PacketQueue = new Queue<Packet>();
            m_QueueLock = new object();
        }


        public void Initialize()
        {
            DynamicAllocatePacket(DEFAULT_ALLOCATE_PACKET_NUM);

            ValidInitializeOnce();
        }

        public void Release()
        {
            ValidReleaseOnce();

            Packet pkt = null;

            while (m_PacketQueue.Count > 0)
            {
                pkt = m_PacketQueue.Dequeue();
                pkt.Release();
                pkt = null;
            }

            m_PacketQueue.Clear();

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
            Packet pkt = null;

            lock (m_QueueLock)
            {
                if (0 == m_PacketQueue.Count)
                {
                    DynamicAllocatePacket(DYNAMIC_ALLOCATE_PACKET_NUM);
                }

                Debug.Assert(m_PacketQueue.Count > 0);
                pkt = m_PacketQueue.Dequeue();
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
            
            lock (m_QueueLock)
            {
                m_PacketQueue.Enqueue(pkt);
            }
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
