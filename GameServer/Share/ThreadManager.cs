using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace Share
{
    public sealed class ThreadManager : Singleton<ThreadManager>
    {
        private List<Thread> m_ThreadList;
        

        public ThreadManager()
        {
            m_ThreadList = new List<Thread>();
        }


        public void AddThread(Thread thread)
        {
            Debug.Assert(null != thread);
            m_ThreadList.Add(thread);
        }
    }
}
