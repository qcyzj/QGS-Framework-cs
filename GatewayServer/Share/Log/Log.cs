using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Share.Logs
{
    public sealed class Log
    {
        private const string DEFAULT_LOG_PATH = "logs";
        private const string DEFAULT_LOG_FILE_PREFIX = "gw_";
        private const string DEFAULT_LOG_FILE_SUFFIXES = ".log";
        private const string XML_LOG_FILE_SUFFIXES = ".xml";
        private const string DEFAULT_LOG_FILE_DATE_FORMAT = "yyyy-MM-dd";

        private const LogManager.LOG_LEVEL DEFAULT_LOG_LEVEL = LogManager.LOG_LEVEL.ALL;
        private const LogManager.LOG_LAYOUT DEFAULT_LOG_LAYOUT = LogManager.LOG_LAYOUT.UTC_TIMESTAMP;

        private const int DEBUG_COLOR = (int)ConsoleColor.Green;
        private const int INFO_COLOR = (int)(ConsoleColor.Green | ConsoleColor.Blue | ConsoleColor.Red);
        private const int WARN_COLOR = (int)ConsoleColor.Cyan;
        private const int ERROR_COLOR = (int)ConsoleColor.Red;
        private const int FATAL_COLOR = (int)ConsoleColor.Yellow;

        private const string STR_LV_DEBUG = "debug";
        private const string STR_LV_INFO = "info";
        private const string STR_LV_WARN = "warn";
        private const string STR_LV_ERROR = "error";
        private const string STR_LV_FATAL = "fatal";
        private const string STR_LV_OFF = "off";

        private const string STR_LO_UTC_TIME = "utc_time";
        private const string STR_LO_RAW_TIME = "raw_time";
        private const string STR_LO_SIMPLE = "simple";
        private const string STR_LO_XML = "xml";

        private const int NORMAL_LOG_WAIT_TIME = 1;
        private const int IDLE_LOG_WAIT_TIME = 10;


        private LogManager.LOG_LEVEL m_Level;
        private LogManager.LOG_LAYOUT m_Layout;
        private int m_LogAppender;
        private string m_LogPath;

        private Thread m_LogThread;
        private LogDBuffer m_LogBuffer;
        private bool m_IsActive;


        public Log(string log_path, string level, string lay_out, int log_appender)
        {
            if (null == log_path || string.Empty == log_path)
            {
                log_path = DEFAULT_LOG_PATH;
            }

            log_path = Path.Combine(Folder.GetCurrentDir(), log_path);

            if (!Directory.Exists(log_path))
            {
                Directory.CreateDirectory(log_path);
            }

            m_Level = ParseLogLevel(level);
            m_Layout = ParseLogLayout(lay_out);
            m_LogAppender = log_appender;
            m_LogPath = Path.Combine(log_path, DEFAULT_LOG_FILE_PREFIX);

            m_LogThread = new Thread(this.Run);
            m_LogThread.Name = this.GetType().Name + " work thread";
            m_LogBuffer = new LogDBuffer();
            m_IsActive = true;

            m_LogThread.Start();

            ThreadManager.Instance.AddThread(m_LogThread);
        }


        public LogManager.LOG_LEVEL GetLevel()
        {
            return m_Level;
        }

        private LogManager.LOG_LEVEL ParseLogLevel(string level)
        {
            LogManager.LOG_LEVEL log_lv = LogManager.LOG_LEVEL.ALL;

            switch (level.ToLower())
            {
                case STR_LV_DEBUG:
                    log_lv = LogManager.LOG_LEVEL.DEBUG;
                    break;

                case STR_LV_INFO:
                    log_lv = LogManager.LOG_LEVEL.INFO;
                    break;

                case STR_LV_WARN:
                    log_lv = LogManager.LOG_LEVEL.WARN;
                    break;

                case STR_LV_ERROR:
                    log_lv = LogManager.LOG_LEVEL.ERROR;
                    break;

                case STR_LV_FATAL:
                    log_lv = LogManager.LOG_LEVEL.FATAL;
                    break;

                case STR_LV_OFF:
                    log_lv = LogManager.LOG_LEVEL.OFF;
                    break;

                default:
                    break;
            }

            return log_lv;
        }

        private LogManager.LOG_LAYOUT ParseLogLayout(string lay_out)
        {
            LogManager.LOG_LAYOUT log_lay = LogManager.LOG_LAYOUT.UTC_TIMESTAMP;

            switch (lay_out.ToLower())
            {
                case STR_LO_UTC_TIME:
                    log_lay = LogManager.LOG_LAYOUT.UTC_TIMESTAMP;
                    break;

                case STR_LO_RAW_TIME:
                    log_lay = LogManager.LOG_LAYOUT.RAW_TIMESTAMP;
                    break;

                case STR_LO_SIMPLE:
                    log_lay = LogManager.LOG_LAYOUT.SIMPLE_FORMAT;
                    break;

                case STR_LO_XML:
                    log_lay = LogManager.LOG_LAYOUT.XML_FORMAT;
                    break;

                default:
                    break;

            }
            return log_lay;
        }


        public void Write(LogManager.LOG_LEVEL level, string log, Exception ex = null)
        {
            if (null != ex)
            {
                log = log +
                      "\r\n Error: " + ex.Message +
                      "\r\n InnerException: " + ex.InnerException +
                      "\r\n StackTrace: " + ex.StackTrace;
            }

            string str_now = string.Empty;

            if (LogManager.LOG_LAYOUT.UTC_TIMESTAMP == m_Layout)
            {
                str_now = Time.GetUtcNow().ToString("G");
            }
            else if (LogManager.LOG_LAYOUT.RAW_TIMESTAMP == m_Layout)
            {
                str_now = Time.GetNow().ToString("G");
            }
            else if (LogManager.LOG_LAYOUT.SIMPLE_FORMAT == m_Layout)
            { }
            else if (LogManager.LOG_LAYOUT.XML_FORMAT == m_Layout)
            { }

            log = str_now + " [" + GetLevelName(level) + "] " + log;

            if ((m_LogAppender | (int)LogManager.LOG_APPENDER.FILE) > 0)
            {
                WriteBuffer(log);
            }

            if ((m_LogAppender | (int)LogManager.LOG_APPENDER.CONSOLE) > 0 ||
                (m_LogAppender | (int)LogManager.LOG_APPENDER.COLORED_CONSOLE) > 0)
            {
                WriteConsole(level, log);
            }

            if ((m_LogAppender | (int)LogManager.LOG_APPENDER.TRACE) > 0)
            {
                WriteTrace(log);
            }
        }

        private void WriteBuffer(string log)
        {
            log = log + "\r\n";
            m_LogBuffer.WriteLog(log);
        }

        private void WriteFile(DateTimeOffset today, byte[] log_array)
        {
            string file_suffixes = string.Empty;

            if (LogManager.LOG_LAYOUT.XML_FORMAT == m_Layout)
            {
                file_suffixes = XML_LOG_FILE_SUFFIXES;
            }
            else
            {
                file_suffixes = DEFAULT_LOG_FILE_SUFFIXES;
            }

            string file_name = m_LogPath + today.ToString(DEFAULT_LOG_FILE_DATE_FORMAT) + file_suffixes;

            using (FileStream stream = new FileStream(file_name, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                stream.Write(log_array, 0, log_array.Length);
            }
        }

        private void WriteConsole(LogManager.LOG_LEVEL level, string log)
        {
            if ((m_LogAppender | (int)LogManager.LOG_APPENDER.COLORED_CONSOLE) > 0)
            {
                SetConsoleFrontColor(level);
            }

            Console.Out.WriteLine(log);
        }

        private void WriteTrace(string log)
        {
            Trace.WriteLine(log);
        }


        private void SetConsoleFrontColor(LogManager.LOG_LEVEL level)
        {
            int color = -1;

            switch (level)
            {
                case LogManager.LOG_LEVEL.DEBUG:
                    color = DEBUG_COLOR;
                    break;

                case LogManager.LOG_LEVEL.INFO:
                    color = INFO_COLOR;
                    break;

                case LogManager.LOG_LEVEL.WARN:
                    color = WARN_COLOR;
                    break;

                case LogManager.LOG_LEVEL.ERROR:
                    color = ERROR_COLOR;
                    break;

                case LogManager.LOG_LEVEL.FATAL:
                    color = FATAL_COLOR;
                    break;

                default:
                    break;
            }

            if (-1 != color)
            {
                Console.ForegroundColor = (ConsoleColor)color;
            }
        }

        private string GetLevelName(LogManager.LOG_LEVEL level)
        {
            string str_lv = string.Empty;

            switch (level)
            {
                case LogManager.LOG_LEVEL.DEBUG:
                    str_lv = STR_LV_DEBUG;
                    break;

                case LogManager.LOG_LEVEL.INFO:
                    str_lv = STR_LV_INFO;
                    break;

                case LogManager.LOG_LEVEL.WARN:
                    str_lv = STR_LV_WARN;
                    break;

                case LogManager.LOG_LEVEL.ERROR:
                    str_lv = STR_LV_ERROR;
                    break;

                case LogManager.LOG_LEVEL.FATAL:
                    str_lv = STR_LV_FATAL;
                    break;

                default:
                    break;
            }

            return str_lv;
        }


        public void Release()
        {
            m_LogBuffer.SetServiceStop();

            while (m_LogBuffer.HasLog())
            {
                Thread.Sleep(1);
            }

            m_IsActive = false;

            if (null != m_LogThread)
            {
                m_LogThread.Join();
            }

            m_LogThread = null;
        }

        private void Run()
        {
            bool has_log = false;
            byte[] log_array = null;
            int sleep_seconds = 0;
            StringBuilder temp_sb = new StringBuilder();

            while (m_IsActive)
            {
                log_array = m_LogBuffer.ReadLog(temp_sb, ref has_log);

                if (has_log)
                {
                    DateTimeOffset today = Time.GetToday();
                    WriteFile(today, log_array);

                    sleep_seconds = NORMAL_LOG_WAIT_TIME;
                }
                else
                {
                    sleep_seconds = IDLE_LOG_WAIT_TIME;
                }

                Thread.Sleep(sleep_seconds);
            }
        }
    }
}
