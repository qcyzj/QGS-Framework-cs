using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

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
        private LogBuffer m_LogBuffer;
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
            m_LogBuffer = new LogBuffer();
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
            DateTime today = Time.GetToday();

            if (LogManager.LOG_LAYOUT.UTC_TIMESTAMP == m_Layout)
            {
                str_now = today.ToShortDateString() + " " + Time.GetUtcNow().ToLongTimeString();
            }
            else if (LogManager.LOG_LAYOUT.RAW_TIMESTAMP == m_Layout)
            {
                str_now = today.ToShortDateString() + " " + Time.GetNow().ToLongTimeString();
            }
            else if (LogManager.LOG_LAYOUT.SIMPLE_FORMAT == m_Layout)
            {}
            else if (LogManager.LOG_LAYOUT.XML_FORMAT == m_Layout)
            {}

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

        private void WriteFile(DateTime today, byte[] log_array)
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
                    DateTime today = Time.GetToday();
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

    public sealed class LogBuffer
    {
        private enum BUFFER_ID
        {
            INVALID = 0,
            BUF_FIRST,
            BUF_SECOND,
        }


        private const int LOG_BUF_SIZE_MAX = 16;
        private const int LOG_BUF_CAN_READ_SIZE = 12;


        private List<string> m_BufferFirst;
        private List<string> m_BufferSecond;
        private BUFFER_ID m_WriteBufID;
        private object m_WriteLockObj;
        private bool m_BufferCanRead;
        private bool m_ServiceWillStop;


        public LogBuffer()
        {
            m_BufferFirst = new List<string>(LOG_BUF_SIZE_MAX);
            m_BufferSecond = new List<string>(LOG_BUF_SIZE_MAX);

            m_WriteBufID = BUFFER_ID.BUF_FIRST;
            m_WriteLockObj = new object();
            m_BufferCanRead = false;
            m_ServiceWillStop = false;
        }


        public void WriteLog(string log)
        {
            lock (m_WriteLockObj)
            {
                List<string> write_buf = BUFFER_ID.BUF_FIRST == m_WriteBufID ? m_BufferFirst : m_BufferSecond;
                write_buf.Add(log);

                if (!m_BufferCanRead && write_buf.Count >= LOG_BUF_CAN_READ_SIZE)
                {
                    m_BufferCanRead = true;
                }
            }
        }

        public byte[] ReadLog(StringBuilder temp_sb, ref bool has_log)
        {
            temp_sb.Clear();

            List<string> write_buf = BUFFER_ID.BUF_FIRST == m_WriteBufID ? m_BufferFirst : m_BufferSecond;
            List<string> read_buf = BUFFER_ID.BUF_FIRST == m_WriteBufID ? m_BufferSecond : m_BufferFirst;
            byte[] log_array = null;

            if (0 == read_buf.Count)
            {
                lock (m_WriteLockObj)
                {
                    if (m_BufferCanRead || m_ServiceWillStop)
                    {
                        SwapBuffer(ref write_buf, ref read_buf);

                        m_BufferCanRead = false;

                        Debug.Assert(0 == write_buf.Count);
                    }
                }

                if (read_buf.Count > 0)
                {
                    foreach (string log in read_buf)
                    {
                        temp_sb.Append(log);
                    }

                    read_buf.Clear();

                    log_array = Encoding.Default.GetBytes(temp_sb.ToString());
                    has_log = true;
                }
                else
                {
                    has_log = false;
                }
            }

            return log_array;
        }

        private void SwapBuffer(ref List<string> write_buf, ref List<string> read_buf)
        {
            List<string> temp = write_buf;
            write_buf = read_buf;
            read_buf = temp;

            if (BUFFER_ID.BUF_FIRST == m_WriteBufID)
            {
                m_WriteBufID = BUFFER_ID.BUF_SECOND;
            }
            else
            {
                m_WriteBufID = BUFFER_ID.BUF_FIRST;
            }
        }


        public bool HasLog()
        {
            int write_num = 0;
            int read_num = 0;

            lock(m_WriteLockObj)
            {
                List<string> write_buf = BUFFER_ID.BUF_FIRST == m_WriteBufID ? m_BufferFirst : m_BufferSecond;
                List<string> read_buf = BUFFER_ID.BUF_FIRST == m_WriteBufID ? m_BufferSecond : m_BufferFirst;

                write_num = write_buf.Count;
                read_num = read_buf.Count;
            }

            return write_num > 0 || read_num > 0;
        }

        public void SetServiceStop()
        {
            m_ServiceWillStop = true;
        }
    }
}
