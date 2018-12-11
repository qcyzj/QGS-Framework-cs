
using System;
using System.Reflection;

using Share;

namespace GatewayServer.Test
{
    public class TestShare
    {
        public TestShare()
        { }

        public void RunAllTest()
        {
            Test_Share_Folder();

            Test_Share_LogManager();

            Test_Share_Singleton();

            Test_Share_ThreadManager();

            Test_Share_Time();
        }


        private void Test_Share_Folder()
        {
            string expect_dir = System.IO.Directory.GetCurrentDirectory();
            string real_dir = Folder.GetCurrentDir();

            CAssert.AreEqual(expect_dir, real_dir);
        }

        private void Test_Share_LogManager()
        {
            string log_dir = "logs";
            int append = (int)LogManager.LOG_APPENDER.COLORED_CONSOLE |
                         (int)LogManager.LOG_APPENDER.FILE |
                         (int)LogManager.LOG_APPENDER.TRACE;

            LogManager.Initialize(log_dir, "all", "utc_time", append);
            LogManager.Debug("log debug testing output.");
            LogManager.Info("log info testing output.");
            LogManager.Warn("log warn testing output.");
            LogManager.Error("log error testing output.");
            LogManager.Fatal("log fatal testing output.");
        }

        private void Test_Share_Singleton()
        {
            Type type = ThreadManager.Instance.GetType();
            CAssert.IsNotNull(ThreadManager.Instance);
        }

        private void Test_Share_ThreadManager()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                LogManager.Warn(MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private void Test_Share_Time()
        {
            DateTime now = Time.GetNow();
            DateTime utc_now = Time.GetUtcNow();
            DateTime today = Time.GetToday();
            DateTime next_day = Time.GetNextDay(now);

            CAssert.IsTrue(Time.IsInSameDay(now, today));
            CAssert.IsFalse(Time.IsInSameDay(today, next_day));

            CAssert.IsTrue(Time.IsInSameWeek(now, utc_now));
        }
    }
}
