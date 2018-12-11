
using System.Diagnostics;
using System.Collections.Generic;

using Share;

namespace GatewayServer.Gateway.GameServer
{
    public sealed class GameServerManager : Singleton<GameServerManager>
    {
        private const int DEFAULT_GAME_SERVER_NUM = 5;
        private const int DYNAMIC_GAME_SERVER_NUM = 2;


        private Queue<GameServer> m_FreeGameServerQueue;
        private Dictionary<uint, GameServer> m_ConnectedGameServerDict;


        public GameServerManager()
        {
            m_FreeGameServerQueue = new Queue<GameServer>();
            m_ConnectedGameServerDict = new Dictionary<uint, GameServer>();
        }


        public void Initialize()
        {
            DynamicAllocateGameServer(DEFAULT_GAME_SERVER_NUM);
        }

        public void Release()
        {
            GameServer game_s = null;

            while (m_FreeGameServerQueue.Count > 0)
            {
                game_s = m_FreeGameServerQueue.Dequeue();
                game_s.Release();
                game_s = null;
            }

            m_FreeGameServerQueue = null;

            foreach (GameServer tmp_game_s in m_ConnectedGameServerDict.Values)
            {
                tmp_game_s.Release();
            }

            m_ConnectedGameServerDict.Clear();
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

        public void AddGameServer(GameServer game_s)
        {
            Debug.Assert(!m_ConnectedGameServerDict.ContainsKey(game_s.GameServerID));

            m_ConnectedGameServerDict.Add(game_s.GameServerID, game_s);
        }
    }
}
