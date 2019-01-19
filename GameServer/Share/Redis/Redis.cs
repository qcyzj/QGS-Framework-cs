using System;
using System.Collections.Generic;

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


        // key operation
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


        // string operation
        public RedisValue StringGet(RedisKey key)
        {
            return m_DataBase.StringGet(key);
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

        public RedisValue[] StringMGet(RedisKey[] keys)
        {
            return m_DataBase.StringGet(keys);
        }

        public bool StringMSet(KeyValuePair<RedisKey, RedisValue>[] pairs)
        {
            return m_DataBase.StringSet(pairs);
        }

        public long StringIncr(RedisKey key, long value)
        {
            return m_DataBase.StringIncrement(key, value);
        }

        public double StringIncr(RedisKey key, double value)
        {
            return m_DataBase.StringIncrement(key, value);
        }

        public long StringDecr(RedisKey key, long value)
        {
            return m_DataBase.StringDecrement(key, value);
        }

        public double StringDecr(RedisKey key, double value)
        {
            return m_DataBase.StringDecrement(key, value);
        }

        public long StringAppend(RedisKey key, RedisValue value)
        {
            return m_DataBase.StringAppend(key, value);
        }

        public long StringLength(RedisKey key)
        {
            return m_DataBase.StringLength(key);
        }


        // bit operation
        public bool GetBit(RedisKey key, long offset)
        {
            return m_DataBase.StringGetBit(key, offset);
        }

        public bool SetBit(RedisKey key, long offset, bool bit)
        {
            return m_DataBase.StringSetBit(key, offset, bit);
        }

        public long BitCount(RedisKey key, long start = 0, long end = -1)
        {
            return m_DataBase.StringBitCount(key, start, end);
        }

        public long BitOperation(Bitwise op, RedisKey dest_key, RedisKey[] keys)
        {
            return m_DataBase.StringBitOperation(op, dest_key, keys);
        }

        public long BitPos(RedisKey key, bool bit, long start = 0, long end = -1)
        {
            return m_DataBase.StringBitPosition(key, bit, start, end);
        }


        // hash operation
        public RedisValue HashSet(RedisKey key, RedisValue field)
        {
            return m_DataBase.HashGet(key, field);
        }

        public bool HashGet(RedisKey key, RedisValue field, RedisValue value)
        {
            return m_DataBase.HashSet(key, field, value);
        }

        public void HashMSet(RedisKey key, HashEntry[] fields_and_values)
        {
            m_DataBase.HashSet(key, fields_and_values);
        }

        public RedisValue[] HashMGet(RedisKey key, RedisValue[] fields)
        {
            return m_DataBase.HashGet(key, fields);
        }

        public HashEntry[] HasGetAll(RedisKey key)
        {
            return m_DataBase.HashGetAll(key);
        }

        public bool HashFieldExists(RedisKey key, RedisValue field)
        {
            return m_DataBase.HashExists(key, field);
        }

        public long HashIncrBy(RedisKey key, RedisValue field, long value)
        {
            return m_DataBase.HashIncrement(key, field, value);
        }

        public double HashIncrBy(RedisKey key, RedisValue field, double value)
        {
            return m_DataBase.HashIncrement(key, field, value);
        }

        public long HashDecrBy(RedisKey key, RedisValue field, long value)
        {
            return m_DataBase.HashDecrement(key, field, value);
        }

        public double HashDecrBy(RedisKey key, RedisValue field, double value)
        {
            return m_DataBase.HashDecrement(key, field, value);
        }

        public bool HashDeleteField(RedisKey key, RedisValue field)
        {
            return m_DataBase.HashDelete(key, field);
        }

        public long HashDeleteFields(RedisKey key, RedisValue[] fields)
        {
            return m_DataBase.HashDelete(key, fields);
        }

        public RedisValue[] HashKeys(RedisKey key)
        {
            return m_DataBase.HashKeys(key);
        }

        public RedisValue[] HashValues(RedisKey key)
        {
            return m_DataBase.HashValues(key);
        }

        public long HashFieldCount(RedisKey key)
        {
            return m_DataBase.HashLength(key);
        }


        // lilst operaion
        public long ListLeftPush(RedisKey key, RedisValue value)
        {
            return m_DataBase.ListLeftPush(key, value);
        }

        public long ListLeftPush(RedisKey key, RedisValue[] values)
        {
            return m_DataBase.ListLeftPush(key, values);
        }

        public long ListRightPush(RedisKey key, RedisValue value)
        {
            return m_DataBase.ListRightPush(key, value);
        }

        public long ListRightPush(RedisKey key, RedisValue[] values)
        {
            return m_DataBase.ListRightPush(key, values);
        }

        public RedisValue ListLeftPop(RedisKey key)
        {
            return m_DataBase.ListLeftPop(key);
        }

        public RedisValue ListRightPop(RedisKey key)
        {
            return m_DataBase.ListRightPop(key);
        }

        public long ListLength(RedisKey key)
        {
            return m_DataBase.ListLength(key);
        }

        public RedisValue[] ListRange(RedisKey key, long start = 0, long end = -1)
        {
            return m_DataBase.ListRange(key, start, end);
        }

        public long ListRemove(RedisKey key, RedisValue value, long count = 0)
        {
            return m_DataBase.ListRemove(key, value, count);
        }

        public RedisValue ListGetByIndex(RedisKey key, long index)
        {
            return m_DataBase.ListGetByIndex(key, index);
        }

        public void ListSetByIndex(RedisKey key, long index, RedisValue value)
        {
            m_DataBase.ListSetByIndex(key, index, value);
        }

        public void ListTrim(RedisKey key, long start, long end)
        {
            m_DataBase.ListTrim(key, start, end);
        }

        public long ListInsertBefore(RedisKey key, RedisValue pivot, RedisValue value)
        {
            return m_DataBase.ListInsertBefore(key, pivot, value);
        }

        public long ListInsertAfter(RedisKey key, RedisValue pivot, RedisValue value)
        {
            return m_DataBase.ListInsertAfter(key, pivot, value);
        }

        public RedisValue ListRightPopLeftPush(RedisKey source, RedisKey dest)
        {
            return m_DataBase.ListRightPopLeftPush(source, dest);
        }


        // set operation
        public bool SetAdd(RedisKey key, RedisValue value)
        {
            return m_DataBase.SetAdd(key, value);
        }

        public long SetAdd(RedisKey key, RedisValue[] values)
        {
            return m_DataBase.SetAdd(key, values);
        }

        public bool SetRemove(RedisKey key, RedisValue value)
        {
            return m_DataBase.SetRemove(key, value);
        }

        public long SetRemove(RedisKey key, RedisValue[] values)
        {
            return m_DataBase.SetRemove(key, values);
        }

        public RedisValue[] SetMembers(RedisKey key)
        {
            return m_DataBase.SetMembers(key);
        }

        public bool SetContains(RedisKey key, RedisValue value)
        {
            return m_DataBase.SetContains(key, value);
        }

        public long SetLength(RedisKey key)
        {
            return m_DataBase.SetLength(key);
        }

        public RedisValue SetRandMember(RedisKey key)
        {
            return m_DataBase.SetRandomMember(key);
        }

        public RedisValue[] SetRandMembers(RedisKey key, int count)
        {
            return m_DataBase.SetRandomMembers(key, count);
        }

        public RedisValue SetRandPop(RedisKey key)
        {
            return m_DataBase.SetPop(key);
        }


        public RedisValue[] SetDiff(RedisKey first, RedisKey second)
        {
            return m_DataBase.SetCombine(SetOperation.Difference, first, second);
        }

        public RedisValue[] SetDiff(RedisKey[] keys)
        {
            return m_DataBase.SetCombine(SetOperation.Difference, keys);
        }

        public RedisValue[] SetInter(RedisKey first, RedisKey second)
        {
            return m_DataBase.SetCombine(SetOperation.Intersect, first, second);
        }

        public RedisValue[] SetInter(RedisKey[] keys)
        {
            return m_DataBase.SetCombine(SetOperation.Intersect, keys);
        }

        public RedisValue[] SetUnion(RedisKey first, RedisKey second)
        {
            return m_DataBase.SetCombine(SetOperation.Union, first, second);
        }

        public RedisValue[] SetUnion(RedisKey[] keys)
        {
            return m_DataBase.SetCombine(SetOperation.Union, keys);
        }

        public long SetDiffAndStore(RedisKey dest, RedisKey[] keys)
        {
            return m_DataBase.SetCombineAndStore(SetOperation.Difference, dest, keys);
        }

        public long SetInterAndStore(RedisKey dest, RedisKey[] keys)
        {
            return m_DataBase.SetCombineAndStore(SetOperation.Intersect, dest, keys);
        }

        public long SetUnionAndStore(RedisKey dest, RedisKey[] keys)
        {
            return m_DataBase.SetCombineAndStore(SetOperation.Union, dest, keys);
        }


        // zset operation
        public bool ZsetAdd(RedisKey key, RedisValue member, double score)
        {
            return m_DataBase.SortedSetAdd(key, member, score);
        }

        public long ZsetAdd(RedisKey key, SortedSetEntry[] members_and_scores)
        {
            return m_DataBase.SortedSetAdd(key, members_and_scores);
        }

        public double? ZsetGetScore(RedisKey key, RedisValue member)
        {
            return m_DataBase.SortedSetScore(key, member);
        }

        public RedisValue[] ZsetRangeByRank(RedisKey key, long start = 0, long end = -1,
                                            Order order = Order.Ascending)
        {
            return m_DataBase.SortedSetRangeByRank(key, start, end, order);
        }

        public SortedSetEntry[] ZsetRangeByRankWithScore(RedisKey key, long start = 0, long end = -1,
                                                         Order order = Order.Ascending)
        {
            return m_DataBase.SortedSetRangeByRankWithScores(key, start, end, order);
        }

        public RedisValue[] ZsetRangeByScore(RedisKey key, long min, long max,
                                             Exclude exclude = Exclude.None, Order order = Order.Ascending,
                                             long skip = 0, long take = -1)
        {
            return m_DataBase.SortedSetRangeByScore(key, min, max, exclude, order, skip, take);
        }

        public SortedSetEntry[] ZsetRangeByScoreWithScore(RedisKey key, long min, long max,
                                                      Exclude exclude = Exclude.None, Order order = Order.Ascending,
                                                      long skip = 0, long take = -1)
        {
            return m_DataBase.SortedSetRangeByScoreWithScores(key, min, max, exclude, order, skip, take);
        }

        public RedisValue ZsetIncrBy(RedisKey key, RedisValue member, double value)
        {
            return m_DataBase.SortedSetIncrement(key, member, value);
        }

        public long ZsetLength(RedisKey key)
        {
            return m_DataBase.SortedSetLength(key);
        }

        public long ZsetCount(RedisKey key, double min, double max)
        {
            return m_DataBase.SortedSetLength(key, min, max);
        }

        public bool ZsetRemove(RedisKey key, RedisValue member)
        {
            return m_DataBase.SortedSetRemove(key, member);
        }

        public long ZsetRemove(RedisKey key, RedisValue[] members)
        {
            return m_DataBase.SortedSetRemove(key, members);
        }

        public long ZsetRemoveRangeByRank(RedisKey key, long start, long end)
        {
            return m_DataBase.SortedSetRemoveRangeByRank(key, start, end);
        }

        public long ZsetRemoveRangeByScore(RedisKey key, double min, double max, 
                                           Exclude exclude = Exclude.None)
        {
            return m_DataBase.SortedSetRemoveRangeByScore(key, min, max, exclude);
        }

        public long? ZsetGetRank(RedisKey key, RedisValue member, Order order = Order.Ascending)
        {
            return m_DataBase.SortedSetRank(key, member, order);
        }

        public long ZsetInterStore(RedisKey dest, RedisKey first, RedisKey second, Aggregate aggregate)
        {
            return m_DataBase.SortedSetCombineAndStore(SetOperation.Intersect, dest, first, second, aggregate);
        }

        public long ZsetInterStore(RedisKey dest, RedisKey[] keys, double[] weights, Aggregate aggregate)
        {
            return m_DataBase.SortedSetCombineAndStore(SetOperation.Intersect, dest, keys, weights, aggregate);
        }

        public long ZsetUnionStore(RedisKey dest, RedisKey first, RedisKey second, Aggregate aggregate)
        {
            return m_DataBase.SortedSetCombineAndStore(SetOperation.Union, dest, first, second, aggregate);
        }

        public long ZsetUnionStore(RedisKey dest, RedisKey[] keys, double[] weights, Aggregate aggregate)
        {
            return m_DataBase.SortedSetCombineAndStore(SetOperation.Union, dest, keys, weights, aggregate);
        }
    }
}
