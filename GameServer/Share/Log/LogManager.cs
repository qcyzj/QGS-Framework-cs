using System;

namespace Share
{
    public static class LogManager
    {
        public enum LOG_LEVEL
        {
            ALL = 0,
            DEBUG,
            INFO,
            WARN,
            ERROR,
            FATAL,
            OFF,
        }

        public enum LOG_APPENDER
        {
            COLORED_CONSOLE = 0x01,
            CONSOLE = 0x02,
            FILE = 0x04,
            TRACE = 0x08,
        }

        public enum LOG_LAYOUT
        {
            UTC_TIMESTAMP = 0,
            RAW_TIMESTAMP,
            SIMPLE_FORMAT,
            XML_FORMAT,
        }


        private static Log m_Logger;


        public static void Initialize(string log_path, string level, string lay_out, int log_appender)
        {
            m_Logger = new Log(log_path, level, lay_out, log_appender);
        }

        public static void Release()
        {
            if (null != m_Logger)
            {
                m_Logger.Release();
            }
        }


        public static void Debug(string log)
        {
            Debug(log, null);
        }

        public static void Info(string log)
        {
            Info(log, null);
        }

        public static void Warn(string log)
        {
            Warn(log, null);
        }

        public static void Error(string log)
        {
            Error(log, null);
        }

        public static void Fatal(string log)
        {
            Fatal(log, null);
        }

        public static void Debug(string log, Exception ex)
        {
            if (m_Logger.GetLevel() <= LOG_LEVEL.DEBUG)
            {
                m_Logger.Write(LOG_LEVEL.DEBUG, log, ex);
            }
        }

        public static void Info(string log, Exception ex)
        {
            if (m_Logger.GetLevel() <= LOG_LEVEL.INFO)
            {
                m_Logger.Write(LOG_LEVEL.INFO, log, ex);
            }
        }

        public static void Warn(string log, Exception ex)
        {
            if (m_Logger.GetLevel() <= LOG_LEVEL.WARN)
            {
                m_Logger.Write(LOG_LEVEL.WARN, log, ex);
            }
        }

        public static void Error(string log, Exception ex)
        {
            if (m_Logger.GetLevel() <= LOG_LEVEL.ERROR)
            {
                m_Logger.Write(LOG_LEVEL.ERROR, log, ex);
            }
        }

        public static void Fatal(string log, Exception ex)
        {
            if (m_Logger.GetLevel() <= LOG_LEVEL.FATAL)
            {
                m_Logger.Write(LOG_LEVEL.FATAL, log, ex);
            }
        }
    }
}
