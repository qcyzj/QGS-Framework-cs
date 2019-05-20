using System;

namespace Share
{
    public static class Time
    {
        private static DateTimeOffset min_time = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        private static DateTimeOffset max_time = new DateTimeOffset(2099, 1, 1, 0, 0, 0, TimeSpan.Zero);


        public static DateTimeOffset MinValue { get { return min_time; } }
        public static DateTimeOffset MaxValue { get { return max_time; } }


        public static DateTimeOffset GetUtcNow()
        {
            return DateTimeOffset.UtcNow;
        }

        public static DateTimeOffset GetNow()
        {
            return DateTimeOffset.Now;
        }

        public static DateTimeOffset GetToday()
        {
            return DateTimeOffset.UtcNow.Date;
        }

        public static DateTimeOffset GetNextDay(DateTimeOffset cur)
        {
            return cur.Date.AddDays(1);
        }

        public static long GetCurMilliseconds()
        {
            return (long)(DateTimeOffset.UtcNow - min_time).TotalMilliseconds;
        }


        public static bool IsInSameDay(DateTimeOffset left, DateTimeOffset right)
        {
            return left.Date == right.Date; 
        }

        public static bool IsInSameWeek(DateTimeOffset left, DateTimeOffset right)
        {
            return IsInSameDay(left.AddDays(-(int)left.DayOfWeek), right.AddDays(-(int)right.DayOfWeek));
        }
    }
}
