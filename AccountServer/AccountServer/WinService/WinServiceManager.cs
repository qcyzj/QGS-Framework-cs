
using System;
using System.Threading;

using Share;
using Share.Config;
using Share.Net.Buffer;
using Share.Net.Packets;
using Share.Net.Sessions;

using AccountServer.AccountServer.Gateway;
using AccountServer.AccountServer.ID;

namespace AccountServer.AccountServer.WinService
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
            LogManager.Debug("Account Server Start...");
            LogManager.Info("Log manager initialized");

            ConfigManager.Instance.Initialize();
            LogManager.Info("Config manager initialized.");

            BufferManager.Instance.Initialize();
            LogManager.Info("Buffer manager initialized.");

            SocketAsyncEventArgsManager.Instance.Initialize();
            LogManager.Info("Socket async event args manager initialized.");

            SessionManager.Instance.Initialize();
            LogManager.Info("Session manager initialized.");

            PacketManager.Instance.Initialize();
            LogManager.Info("Packet manager initialized.");

            GatewayServerManager.Instance.Initialize();
            LogManager.Info("Game server manager initialized.");

            IDGenerator.Instance.Init(ConfigManager.Instance.ID_GENERATOR_WORKER_ID, 
                                      ConfigManager.Instance.ID_GENERATOR_DATACENTER_ID);
            LogManager.Info("ID generator initialized.");


            LogManager.Info("Gateway server connect manager start.");
            GatewayServerConnectManager.Instance.Start();
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
            GatewayServerConnectManager.Instance.Stop();
            LogManager.Info("Game server connect manager stop.");


            GatewayServerManager.Instance.Release();
            LogManager.Info("Gateway server manager released.");

            PacketManager.Instance.Release();
            LogManager.Info("Packet manager released.");

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
            if (sess.Object is GatewayServer)
            {
                GatewayServer gate_server = sess.Object as GatewayServer;

                if (null != gate_server)
                {
                    GatewayServerManager.Instance.RemoveGatewayServer(gate_server);
                }
            }
        }
    }
}
