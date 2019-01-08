using System;
using System.Threading;

using Share;
using Share.Config;
using Share.Net.Buffer;
using Share.Net.Server;
using Share.Net.Packets;
using Share.Net.Sessions;

using GatewayServer.Gateway.Users;
using GatewayServer.Gateway.GameServers;
using GatewayServer.Gateway.AccountServer;

namespace GatewayServer.Gateway.WinService
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
            LogManager.Debug("Gateway Server Start...");
            LogManager.Info("Log manager initialized.");

            ConfigManager.Instance.Initialize();
            LogManager.Info("Config manager initialized.");

            UdpPortManager.Instance.Initialize();
            LogManager.Info("Udp port manager initialized.");

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

            GameServerManager.Instance.Initialize();
            LogManager.Info("Game server manager initialized.");


            LogManager.Info("Game server connect manager start.");
            GameServerConnectManager.Instance.Start();

            LogManager.Info("User connect manager start.");
            UserConnectManager.Instance.Start();

            AccountServerManager.Instance.Start();
            LogManager.Info("Account server manager start.");
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
            AccountServerManager.Instance.Stop();
            LogManager.Info("Account server manager stop.");

            UserConnectManager.Instance.Stop();
            LogManager.Info("User connect manager stop.");

            GameServerConnectManager.Instance.Stop();
            LogManager.Info("Game server connect manager stop.");


            GameServerManager.Instance.Release();
            LogManager.Info("Game server manager released.");
         
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

            UdpPortManager.Instance.Release();
            LogManager.Info("Udp port manager released.");

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
            if (sess.Object is User)
            {
                User user = sess.Object as User;

                if (null != user)
                {
                    UserManager.Instance.RemoveUser(user);
                }
            }
            else if (sess.Object is GameServer)
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
