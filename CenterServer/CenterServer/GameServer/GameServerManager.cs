using System.Diagnostics;
using System.Collections.Generic;

using Share;
using Share.Net.Packets;

namespace CenterServer.CenterServer.GameServers
{
    public sealed class GameServerManager : Singleton<GameServerManager>
    {
        private const int DEFAULT_GAME_SERVER_NUM = 5;
        private const int DYNAMIC_GAME_SERVER_NUM = 2;


        private Queue<GameServer> m_FreeGameServerQueue;

        private List<GameServer> m_ConnectedGameServerList;
        private Dictionary<uint, GameServer> m_AuthedGameServerDict;
        private object m_AuthedLock;
        private object m_ConnectLock;


        public GameServerManager()
        {
            m_FreeGameServerQueue = new Queue<GameServer>();

            m_ConnectedGameServerList = new List<GameServer>();
            m_AuthedGameServerDict = new Dictionary<uint, GameServer>();

            m_AuthedLock = new object();
            m_ConnectLock = new object();
        }


        public void Initialize()
        {
            DynamicAllocateGameServer(DEFAULT_GAME_SERVER_NUM);

            RegisterGameServerProc();

            ValidInitializeOnce();
        }

        public void Release()
        {
            ValidReleaseOnce();

            GameServer game_s = null;

            while (m_FreeGameServerQueue.Count > 0)
            {
                game_s = m_FreeGameServerQueue.Dequeue();
                game_s.Release();
                game_s = null;
            }

            m_FreeGameServerQueue.Clear();

            lock (m_ConnectLock)
            {
                foreach (GameServer tmp_game_s in m_ConnectedGameServerList)
                {
                    tmp_game_s.Release();
                }
            }

            m_ConnectedGameServerList.Clear();
            m_ConnectedGameServerList = null;

            lock (m_AuthedLock)
            {
                foreach (GameServer tmp_game_s in m_AuthedGameServerDict.Values)
                {
                    tmp_game_s.Release();
                }
            }

            m_AuthedGameServerDict.Clear();
            m_AuthedGameServerDict = null;
        }

        private void DynamicAllocateGameServer(int game_server_num)
        {
            GameServer game_s = null;

            for (int i = 0; i < game_server_num; ++i)
            {
                game_s = new GameServer();
                m_FreeGameServerQueue.Enqueue(game_s);
                game_s = null;
            }
        }


        public GameServer AllocateGameServer()
        {
            if (0 == m_FreeGameServerQueue.Count)
            {
                DynamicAllocateGameServer(DYNAMIC_GAME_SERVER_NUM);
            }

            GameServer game_s = m_FreeGameServerQueue.Dequeue();
            Debug.Assert(null != game_s);
            return game_s;
        }

        private void FreeGameServer(GameServer game_s)
        {
            if (null != game_s)
            {
                return;
            }

            m_FreeGameServerQueue.Enqueue(game_s);
        }


        public void AddConnectedGameServer(GameServer game_s)
        {
            lock (m_ConnectLock)
            {
                Debug.Assert(!m_ConnectedGameServerList.Contains(game_s));

                if (!m_ConnectedGameServerList.Contains(game_s))
                {
                    m_ConnectedGameServerList.Add(game_s);
                }
            }

            LogManager.Debug("Add connected game server. Game Server ID = " +
                             game_s.GameServerID.ToString());
        }

        public void AddAuthedGameServer(GameServer game_s)
        {
            lock (m_ConnectLock)
            {
                int index = m_ConnectedGameServerList.IndexOf(game_s);

                if (index > -1)
                {
                    m_ConnectedGameServerList.RemoveAt(index);
                }
                else
                {
                    Debug.Assert(false);
                }
            }

            GameServer tmp_game_s = null;

            lock (m_AuthedLock)
            {
                if (m_AuthedGameServerDict.TryGetValue(game_s.GameServerID, out tmp_game_s))
                {
                    Debug.Assert(false);
                    m_AuthedGameServerDict.Remove(game_s.GameServerID);

                    FreeGameServer(tmp_game_s);
                }

                m_AuthedGameServerDict.Add(game_s.GameServerID, game_s);
            }

            LogManager.Debug("Add authed game server. Game server ID = " +
                             game_s.GameServerID.ToString());
        }

        public void RemoveGameServer(GameServer game_s)
        {
            lock (m_AuthedLock)
            {
                if (m_AuthedGameServerDict.ContainsKey(game_s.GameServerID))
                {
                    m_AuthedGameServerDict.Remove(game_s.GameServerID);
                }
            }

            lock (m_ConnectLock)
            {
                int index = m_ConnectedGameServerList.IndexOf(game_s);

                if (index > -1)
                {
                    m_ConnectedGameServerList.RemoveAt(index);
                }
            }

            LogManager.Debug("Remove connected game server. Game server ID = " +
                             game_s.GameServerID.ToString());

            FreeGameServer(game_s);
        }


        private void RegisterGameServerProc()
        {
            RegisterProcImpl(Protocol.GS_CNT_AUTH, GameServer.PacketProcessAuth);
        }

        private void RegisterProcImpl(int protocol, PacketProcessManager.PacketProcessFunc func)
        {
            PacketProcessManager.Instance.RegisterProc(protocol, func);
        }
    }
}
