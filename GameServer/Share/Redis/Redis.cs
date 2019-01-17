using System;

using StackExchange.Redis;

namespace Share.Redis
{
    //public class Redis : ICache
    //{
    //    private const int DEFAULT_TIMEOUT_SECONDS = 300;

    //    ConnectionMultiplexer m_ConnMultiplexer;
    //    IDatabase m_DataBase;


    //    public Redis(string ip_addr, int port)
    //    {
    //        string connect_str = ip_addr + ":" + port;
    //        m_ConnMultiplexer = ConnectionMultiplexer.Connect(connect_str);
    //        m_DataBase = m_ConnMultiplexer.GetDatabase();
    //    }


    //    public object Get(string key)
    //    {
    //        return Get<object>(key);
    //    }

    //    //public T Get<T>(string key)
    //    //{
    //    //    m_DataBase.StringGet(key);
           
    //    //}

    //    bool Exists(string key)
    //    {
    //        return m_DataBase.KeyExists(key);
    //    }

    //    void Remove(string key)

    //    {
    //        m_DataBase.KeyDelete(key);
    //    }

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

    //}
}
