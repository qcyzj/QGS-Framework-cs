using System.Diagnostics;
using System.Collections.Generic;

using Share;
using Share.Net.Packets;

namespace AccountServer.AccountServer.Gateway
{
    public sealed class GatewayServerManager : Singleton<GatewayServerManager>
    {
        private const int DEFAULT_GATEWAY_SERVER_NUM = 5;
        private const int DYNAMIC_GATEWAY_SERVER_NUM = 2;


        private Queue<GatewayServer> m_FreeGatewayQueue;

        private List<GatewayServer> m_ConnectedGatewayList;
        private Dictionary<uint, GatewayServer> m_AuthedGatewayDict;
        private object m_AuthedLock;
        private object m_ConnectLock;


        public GatewayServerManager()
        {
            m_FreeGatewayQueue = new Queue<GatewayServer>();

            m_ConnectedGatewayList = new List<GatewayServer>();
            m_AuthedGatewayDict = new Dictionary<uint, GatewayServer>();

            m_AuthedLock = new object();
            m_ConnectLock = new object();
        }


        public void Initialize()
        {
            DynamicAllocateGatewayServer(DEFAULT_GATEWAY_SERVER_NUM);

            RegisterGatewayServerProc();

            ValidInitializeOnce();
        }

        public void Release()
        {
            ValidReleaseOnce();

            GatewayServer gateway_s = null;

            while (m_FreeGatewayQueue.Count > 0)
            {
                gateway_s = m_FreeGatewayQueue.Dequeue();
                gateway_s.Release();
                gateway_s = null;
            }

            m_FreeGatewayQueue = null;

            lock (m_ConnectLock)
            {
                foreach (GatewayServer tmp_gateway_s in m_ConnectedGatewayList)
                {
                    tmp_gateway_s.Release();
                }
            }

            m_ConnectedGatewayList.Clear();
            m_ConnectedGatewayList = null;

            lock (m_AuthedLock)
            {
                foreach (GatewayServer tmp_gateway_s in m_AuthedGatewayDict.Values)
                {
                    tmp_gateway_s.Release();
                }
            }

            m_AuthedGatewayDict.Clear();
            m_AuthedGatewayDict = null;
        }

        private void DynamicAllocateGatewayServer(int gateway_server_num)
        {
            GatewayServer gateway_s = null;

            for (int i = 0; i < gateway_server_num; ++i)
            {
                gateway_s = new GatewayServer();
                m_FreeGatewayQueue.Enqueue(gateway_s);
                gateway_s = null;
            }
        }


        public GatewayServer AllocateGatewayServer()
        {
            if (0 == m_FreeGatewayQueue.Count)
            {
                DynamicAllocateGatewayServer(DYNAMIC_GATEWAY_SERVER_NUM);
            }

            GatewayServer gateway_s = m_FreeGatewayQueue.Dequeue();
            Debug.Assert(null != gateway_s);
            return gateway_s;
        }

        private void FreeGatewayServer(GatewayServer gateway_s)
        {
            if (null == gateway_s)
            {
                return;
            }

            m_FreeGatewayQueue.Enqueue(gateway_s);
        }


        public void AddConnectedGatewayServer(GatewayServer gateway_s)
        {
            lock (m_ConnectLock)
            {
                Debug.Assert(!m_ConnectedGatewayList.Contains(gateway_s));

                if (!m_ConnectedGatewayList.Contains(gateway_s))
                {
                    m_ConnectedGatewayList.Add(gateway_s);
                }
            }

            LogManager.Debug("Add connected gateway server. Gateway server ID = " +
                             gateway_s.GatewayServerID.ToString());
        }

        public void AddAuthedGatewayServer(GatewayServer gateway_s)
        {
            lock (m_AuthedLock)
            {
                int index = m_ConnectedGatewayList.IndexOf(gateway_s);

                if (index > -1)
                {
                    m_ConnectedGatewayList.RemoveAt(index);
                }
                else
                {
                    Debug.Assert(false);
                }
            }

            GatewayServer tmp_gateway_s = null;

            lock (m_AuthedLock)
            {
                if (m_AuthedGatewayDict.TryGetValue(gateway_s.GatewayServerID, out tmp_gateway_s))
                {
                    Debug.Assert(false);
                    m_AuthedGatewayDict.Remove(gateway_s.GatewayServerID);

                    FreeGatewayServer(tmp_gateway_s);
                }

                m_AuthedGatewayDict.Add(gateway_s.GatewayServerID, gateway_s);
            }

            LogManager.Debug("Add authed gateway server. Gateway server ID = " +
                             gateway_s.GatewayServerID.ToString());
        }

        public void RemoveGatewayServer(GatewayServer gateway_s)
        {
            lock (m_AuthedLock)
            {
                if (m_AuthedGatewayDict.ContainsKey(gateway_s.GatewayServerID))
                {
                    m_AuthedGatewayDict.Remove(gateway_s.GatewayServerID);
                }
            }

            lock (m_ConnectLock)
            {
                int index = m_ConnectedGatewayList.IndexOf(gateway_s);

                if (index > -1)
                {
                    m_ConnectedGatewayList.RemoveAt(index);
                }
            }

            LogManager.Debug("Remove connected gateway server. Gateway server ID " +
                             gateway_s.GatewayServerID.ToString());

            FreeGatewayServer(gateway_s);
        }


        private void RegisterGatewayServerProc()
        {
            RegisterProcImpl(Protocol.GW_ACT_AUTH, GatewayServer.PacketProcessAuth);
        }

        private void RegisterProcImpl(int protocol, PacketProcessManager.PacketProcessFunc func)
        {
            PacketProcessManager.Instance.RegisterProc(protocol, func);
        }
    }
}
