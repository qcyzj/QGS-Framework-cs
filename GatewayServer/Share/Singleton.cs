using System.Diagnostics;

namespace Share
{
    public class Singleton<T> where T : class, new()
    {
#if DEBUG
        private static int m_InitIndex = 0;

        protected void ValidInitializeOnce() { Debug.Assert(1 == ++m_InitIndex); }
        protected void ValidReleaseOnce() { Debug.Assert(0 == --m_InitIndex); }
#endif

        private static readonly T m_Instance = new T();

        protected Singleton() { Debug.Assert(null == m_Instance); }

        public static T Instance { get { return m_Instance; } }
    }
}
