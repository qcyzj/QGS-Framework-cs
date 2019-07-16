using System.Collections;
using System.Collections.Generic;

namespace Share.Collections
{
    public class LightConcurrentList<T> : IEnumerable<T>, IEnumerable
    {
        private List<T> m_List;
        private object m_Lock;

        
        public LightConcurrentList()
        {
            m_List = new List<T>();
            m_Lock = new object();
        }


        public bool Contains(T item)
        {
            lock (this.m_Lock)
            {
                return this.m_List.Contains(item);
            }
        }

        public bool TryAdd(T item)
        {
            lock (this.m_Lock)
            {
                if (!this.m_List.Contains(item))
                {
                    this.m_List.Add(item);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int IndexOf(T item)
        {
            lock (this.m_Lock)
            {
                return this.m_List.IndexOf(item);
            }
        }

        public bool TryRemove(T item)
        {
            lock (this.m_Lock)
            {
                int index = this.m_List.IndexOf(item);
                bool ret = index > -1;

                if (ret)
                {
                    this.m_List.RemoveAt(index);
                }

                return ret;
            }
        }

        public void Clear()
        {
            lock (this.m_Lock)
            {
                this.m_List.Clear();
            }
        }

        public int Count()
        {
            lock (this.m_Lock)
            {
                return this.m_List.Count;
            }
        }


        public IEnumerator<T> GetEnumerator()
        {
            lock (this.m_Lock)
            {
                return this.m_List.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}