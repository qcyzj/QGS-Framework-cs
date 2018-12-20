

using System;
using System.Threading;
using System.Diagnostics;

using Share;
using Share.Net.Buffer;
using Share.Net.Packets;
using Share.Net.Sessions;

using GameServer.GameServer.User;
using GameServer.GameServer.Gateway;
using GameServer.GameServer.CenterServer;

namespace GameServer.GameServer.WinService
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
            LogManager.Debug("Game Server Start...");
            LogManager.Info("Log Manager initialized.");

            BufferManager.Instance.Initialize();
            LogManager.Info("Buffer manager initialized.");

            SocketAsyncEventArgsManager.Instance.Initialize();
            LogManager.Info("Socket aysnc event args manager initialized.");

            SessionManager.Instance.Initialize();
            LogManager.Info("Session mananger initialized.");

            PacketManager.Instance.Initialize();
            LogManager.Info("Packet manager initialized.");

            UserManager.Instance.Initialize();
            LogManager.Info("User manager initialized.");


            GatewayServerManager.Instance.Start();
            LogManager.Info("Gateway server manager start.");

            CenterServerManager.Instance.Start();
            LogManager.Info("Center server manager start.");
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
            CenterServerManager.Instance.Stop();
            LogManager.Info("Center server manager stop.");

            GatewayServerManager.Instance.Stop();
            LogManager.Info("Gateway server manager stop.");


            UserManager.Instance.Release();
            LogManager.Info("User manager released.");

            PacketManager.Instance.Release();
            LogManager.Info("Packet manager released.");

            SessionManager.Instance.Release();
            LogManager.Info("Session manager released.");

            SocketAsyncEventArgsManager.Instance.Release();
            LogManager.Info("Socket async event args mananer released.");

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
            if (sess.Object is GatewayServer)
            {
                GatewayServer gate_server = sess.Object as GatewayServer;

                if (null != gate_server)
                {
                    Debug.Assert(false);
                }
            }
        }
    }
}
