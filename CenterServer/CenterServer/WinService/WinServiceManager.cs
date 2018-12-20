using System;

using Share;
using System.Threading;

using Share.Net.Buffer;
using Share.Net.Packets;
using Share.Net.Sessions;

using CenterServer.CenterServer.GameServers;

namespace CenterServer.CenterServer.WinService
{
    public class WinServiceManager : Singleton<WinServiceManager>
    {
        private bool m_IsActive;


        public WinServiceManager()
        {
            m_IsActive = true;

            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelHandle);

            Session.SetManagerRemoveFunc(ManagerRemoveObjectFunc);
        }


        public void Init()
        {
            string log_dir = "logs";
            string log_level = "all";
            string log_lay_out = "utc_time";
            int log_append = (int)LogManager.LOG_APPENDER.COLORED_CONSOLE |
                             (int)LogManager.LOG_APPENDER.FILE |
                             (int)LogManager.LOG_APPENDER.TRACE;

            LogManager.Initialize(log_dir, log_level, log_lay_out, log_append);
            LogManager.Debug("Center Server Start...");
            LogManager.Info("Log manager initialized.");

            BufferManager.Instance.Initialize();
            LogManager.Info("Buffer manager initialized.");

            SocketAsyncEventArgsManager.Instance.Initialize();
            LogManager.Info("Socket async event args manager initialized.");

            SessionManager.Instance.Initialize();
            LogManager.Info("Session manager initialized.");

            PacketManager.Instance.Initialize();
            LogManager.Info("Packet mananger initialized.");

            GameServerManager.Instance.Initialize();
            LogManager.Info("Game server manager initialized.");


            LogManager.Info("Game server connect manager start.");
            GameServerConnectManager.Instance.Start();
        }

        public void Loop()
        {
            while (m_IsActive)
            {
                Thread.Sleep(100);
            }
        }

        public void Exit()
        {
            GameServerConnectManager.Instance.Stop();
            LogManager.Info("Game server connect manager stop.");


            GameServerManager.Instance.Release();
            LogManager.Info("Game server manager released.");

            PacketManager.Instance.Release();
            LogManager.Info("Packet mananger released.");

            SessionManager.Instance.Release();
            LogManager.Info("Session manager released.");

            SocketAsyncEventArgsManager.Instance.Release();
            LogManager.Info("Socket async event args manager released.");

            BufferManager.Instance.Release();
            LogManager.Info("Buffer manager released.");

            LogManager.Release();
        }

        
        private void CancelHandle(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            SetInactive();
        }

        private void SetInactive()
        {
            m_IsActive = false;
        }


        private void ManagerRemoveObjectFunc(Session sess)
        {
            if (sess.Object is GameServer)
            {
                GameServer game_server = sess.Object as GameServer;

                if (null != game_server)
                {
                    GameServerManager.Instance.RemoveGameServer(game_server);
                }

            }
        }
    }
}
