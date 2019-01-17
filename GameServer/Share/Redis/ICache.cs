using System;

namespace Share.Redis
{
    public interface ICache
    {
        object Get(string key);

        T Get<T>(string key);

        bool Exists(string key);

        void Remove(string key);


        void Insert(string key, object value);

        void Insert<T>(string key, T value);

        void Insert(string key, object value, int expire_time);

        void Insert<T>(string key, T value, int expire_time);

        void Insert(string key, object value, DateTime expire_time);

        void Insert<T>(string key, T value, DateTime expire_time);
    }
}
