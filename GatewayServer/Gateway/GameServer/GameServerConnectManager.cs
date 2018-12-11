using System.Threading;

using Share;

namespace GatewayServer.Gateway.GameServer
{
    public sealed class GameServerConnectManager : Singleton<GameServerConnectManager>
    {
        private GameTcpServer m_GameSocketServer;
        private Thread m_WorkingThread;
        private bool m_IsActive;


        public GameServerConnectManager()
        {
            m_GameSocketServer = new GameTcpServer();

            m_WorkingThread = new Thread(this.Run);
            m_WorkingThread.Name = this.GetType().Name + " working thread";
            m_IsActive = false;
        }


        public void Start()
        {
            m_GameSocketServer.Start();

            m_IsActive = true;
            m_WorkingThread.Start();

            ThreadManager.Instance.AddThread(m_WorkingThread);
        }

        public void Stop()
        {
            m_GameSocketServer.Stop();

            StopWorkingThread();
        }

        private void StopWorkingThread()
        {
            m_IsActive = false;

            if (null != m_WorkingThread)
            {
                m_WorkingThread.Join();
            }

            m_WorkingThread = null;
        }


        private void Run()
        {
            while (m_IsActive)
            {
                ProcessRead();

                ProcessWrite();

                ProcessPackets();

                ProcessHeartBeat();
            }
        }


        private void ProcessRead()
        { }

        private void ProcessWrite()
        { }

        private void ProcessPackets()
        { }

        private void ProcessHeartBeat()
        { }
    }
}
