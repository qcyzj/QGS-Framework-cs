using System;

namespace Share
{
    public static class Time
    {
        private static DateTime min_time = new DateTime(1970, 1, 1);
        private static DateTime max_time = new DateTime(2030, 1, 1);


        public static DateTime MinValue { get { return min_time; } }
        public static DateTime MaxValue { get { return max_time; } }


        public static DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }

        public static DateTime GetNow()
        {
            return DateTime.Now;
        }

        public static DateTime GetToday()
        {
            return DateTime.Today;
        }

        public static DateTime GetNextDay(DateTime cur)
        {
            return cur.Date.AddDays(1);
        }


        public static bool IsInSameDay(DateTime left, DateTime right)
        {
            return left.Date == right.Date; 
        }

        public static bool IsInSameWeek(DateTime left, DateTime right)
        {
            return IsInSameDay(left.AddDays(-(int)left.DayOfWeek), right.AddDays(-(int)right.DayOfWeek));
        }


    }
}
