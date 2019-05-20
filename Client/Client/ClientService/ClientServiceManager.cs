using System;
using System.Threading;

using Share;
using Share.Net.Buffer;
using Share.Net.Packets;
using Share.Net.Sessions;

using Client.Users;

namespace Client.ClientService
{
    public class ClientServiceManager : Singleton<ClientServiceManager>
    {
        private bool m_IsActive;

        public ClientServiceManager()
        {
            m_IsActive = true;

            Console.CancelKeyPress += new ConsoleCancelEventHandler(CancelHandle);
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
            LogManager.Info("Log manager initialized");

            BufferManager.Instance.Initialize();
            LogManager.Info("Buffer manager initialized.");

            SocketAsyncEventArgsManager.Instance.Initialize();
            LogManager.Info("Socket async event args manager initialized.");

            SessionManager.Instance.Initialize();
            LogManager.Info("Session manager initialized.");

            PacketManager.Instance.Initialize();
            LogManager.Info("Packet manager initialized.");

            UserManager.Instance.Initialize();
            LogManager.Info("User manager initialized.");

            LogManager.Info("User connect manager start.");
            UserConnectManager.Instance.Start();
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
            UserConnectManager.Instance.Stop();
            LogManager.Info("User connect manager stop.");
         
            UserManager.Instance.Release();
            LogManager.Info("User manager released.");

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
    }
}
