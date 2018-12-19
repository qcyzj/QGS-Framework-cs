using System.Diagnostics;
using System.Collections.Generic;

namespace Share.Net.Packets
{
    public class PacketProcessManager : Singleton<PacketProcessManager>
    {
        public enum PACKET_PROC_ERROR : int
        {
            SUCCESS = 0,
            E_DEAL_FUNC_NOT_EXIST = -1,
        }


        public delegate int PacketProcessFunc(object obj, Packet pkt);


        private Dictionary<int, PacketProcessFunc> m_ProcessFuncDict;


        public PacketProcessManager()
        {
            m_ProcessFuncDict = new Dictionary<int, PacketProcessFunc>();
        }


        public void Release()
        {
            m_ProcessFuncDict.Clear();
            m_ProcessFuncDict = null;
        }


        public void RegisterProc(int packet_id, PacketProcessFunc func)
        {
            Debug.Assert(!m_ProcessFuncDict.ContainsKey(packet_id));
                
            m_ProcessFuncDict.Add(packet_id, func);
        }


        public int ProcessPacket(object obj, Packet pkt)
        {
            PacketProcessFunc func = null;

            if (m_ProcessFuncDict.TryGetValue(pkt.GetPacketID(), out func))
            {
                return func(obj, pkt);
            }
            else
            {
                LogManager.Error("Can not find func to process packet, packet ID = " + pkt.GetPacketID().ToString());
                return (int)PACKET_PROC_ERROR.E_DEAL_FUNC_NOT_EXIST;
            }
        }
    }
}
