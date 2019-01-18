using System;

using StackExchange.Redis;

namespace Share.Redis
{
    public class Redis
    {
        ConnectionMultiplexer m_ConnMultiplexer;
        IDatabase m_DataBase;

        public DateTime TimeConvert { get; private set; }

        public Redis(string ip_addr, int port)
        {
            string connect_str = ip_addr + ":" + port;
            m_ConnMultiplexer = ConnectionMultiplexer.Connect(connect_str);

            m_DataBase = m_ConnMultiplexer.GetDatabase();
            TimeSpan t_span = m_DataBase.Ping();
        }


        public bool KeyExists(RedisKey key)
        {
            return m_DataBase.KeyExists(key);
        }

        public bool KeyDelete(RedisKey key)

        {
            return m_DataBase.KeyDelete(key);
        }

        public RedisType GetKeyType(RedisKey key)
        {
            return m_DataBase.KeyType(key);
        }


        public RedisValue StringGet(RedisKey key)
        {
            return m_DataBase.StringGet(key);
        }

        public RedisValue[] StringGet(RedisKey[] keys)
        {
            return m_DataBase.StringGet(keys);
        }

        public long StringIncrLong(RedisKey key, long value)
        {
            return m_DataBase.StringIncrement(key, value);
        }

        public double StringIncrDouble(RedisKey key, double value)
        {
            return m_DataBase.StringIncrement(key, value);
        }

        public bool StringSet(RedisKey key, RedisValue value)
        {
            return m_DataBase.StringSet(key, value);
        }

        public bool StringSet(RedisKey key, RedisValue value, TimeSpan expire_time)
        {
            return m_DataBase.StringSet(key, value, expire_time);
        }

        public bool StringSet(RedisKey key, RedisValue value, DateTime expire_time)
        {
            TimeSpan time_span = expire_time - Time.GetUtcNow();
            return m_DataBase.StringSet(key, value, time_span);
        }


        //    public object Get(string key)
        //    {
        //        return Get<object>(key);
        //    }

        //    //public T Get<T>(string key)
        //    //{
        //    //    m_DataBase.StringGet(key);

        //    //}





        //    void Insert(string key, object value)
        //    {
        //        m_DataBase.StringSet(key, value.ToString());
        //    }

        //    void Insert<T>(string key, T value)
        //    {

        //    }

        //    void Insert(string key, object value, int expire_time)
        //    {


        //    }

        //    void Insert<T>(string key, T value, int expire_time)
        //    { }

        //    void Insert(string key, object value, DateTime expire_time)
        //    { }

        //    void Insert<T>(string key, T value, DateTime expire_time)
        //    { }

    }
}
