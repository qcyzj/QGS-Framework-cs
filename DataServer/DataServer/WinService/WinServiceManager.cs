using System;
using System.Threading;

using Share;
using Share.Config;

namespace DataServer.DataServer.WinService
{
    public class WinServiceManager : Singleton<WinServiceManager>
    {
        private bool m_IsActive;


        public WinServiceManager()
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
            LogManager.Debug("Data Server Start...");
            LogManager.Info("Log manager initialized.");

            ConfigManager.Instance.Initialize();
            LogManager.Info("Config manager initialized.");
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
