using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Share;
using Share.Net.Packets;

namespace AccountServer.AccountServer.Gateway
{
    public sealed class GatewayServerManager : Singleton<GatewayServerManager>
    {
        private const int DEFAULT_GATEWAY_SERVER_NUM = 5;
        private const int DYNAMIC_GATEWAY_SERVER_NUM = 2;


        private Queue<GatewayServer> m_FreeGatewayQueue;

        private LightConcurrentList<GatewayServer> m_ConnectedGatewayList;
        private ConcurrentDictionary<uint, GatewayServer> m_AuthedGatewayDict;


        public GatewayServerManager()
        {
            m_FreeGatewayQueue = new Queue<GatewayServer>();

            m_ConnectedGatewayList = new LightConcurrentList<GatewayServer>();
            m_AuthedGatewayDict = new ConcurrentDictionary<uint, GatewayServer>();
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

            foreach (GatewayServer tmp_gateway_s in m_ConnectedGatewayList)
            {
                tmp_gateway_s.Release();
            }

            m_ConnectedGatewayList.Clear();
            m_ConnectedGatewayList = null;

            foreach (GatewayServer tmp_gateway_s in m_AuthedGatewayDict.Values)
            {
                tmp_gateway_s.Release();
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
            Debug.Assert(!m_ConnectedGatewayList.Contains(gateway_s));

            m_ConnectedGatewayList.TryAdd(gateway_s);

            LogManager.Debug("Add connected gateway server. Gateway server ID = " +
                             gateway_s.GatewayServerID.ToString());
        }

        public void AddAuthedGatewayServer(GatewayServer gateway_s)
        {
            Debug.Assert(m_ConnectedGatewayList.IndexOf(gateway_s) > -1);

            if (m_ConnectedGatewayList.TryRemove(gateway_s))
            {}
            else
            {
                Debug.Assert(false);
            }

            if (m_AuthedGatewayDict.TryRemove(gateway_s.GatewayServerID, out GatewayServer tmp_gateway_s))
            {
                Debug.Assert(false);
                FreeGatewayServer(tmp_gateway_s);
            }

            m_AuthedGatewayDict.TryAdd(gateway_s.GatewayServerID, gateway_s);

            LogManager.Debug("Add authed gateway server. Gateway server ID = " +
                             gateway_s.GatewayServerID.ToString());
        }

        public void RemoveGatewayServer(GatewayServer gateway_s)
        {
            m_AuthedGatewayDict.TryRemove(gateway_s.GatewayServerID, out GatewayServer tmp_gate_s);

            m_ConnectedGatewayList.TryRemove(gateway_s);

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
