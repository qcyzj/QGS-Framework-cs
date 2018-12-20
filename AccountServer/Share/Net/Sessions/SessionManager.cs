using System.Diagnostics;
using System.Collections.Generic;

namespace Share.Net.Sessions
{
    public sealed class SessionManager : Singleton<SessionManager>
    {
        public const int USER_TCP_SESSION_MAX_NUM = 100;
        public const int USER_UDP_SESSION_MAX_NUM = 100;
        public const int SERVER_TCP_SESSION_MAX_NUM = 10;


        private Queue<Session> m_FreeTcpUserSessionQueue;
        private Queue<Session> m_FreeUdpUserSessionQueue;
        private Queue<Session> m_FreeServerSessionQueue;


        public SessionManager()
        {
            m_FreeTcpUserSessionQueue = new Queue<Session>();
            m_FreeUdpUserSessionQueue = new Queue<Session>();
            m_FreeServerSessionQueue = new Queue<Session>();
        }


        public void Initialize()
        {
            Session sess = null;

            for (int i = 0; i < USER_TCP_SESSION_MAX_NUM; ++i)
            {
                sess = new TcpSession(i + 1);
                m_FreeTcpUserSessionQueue.Enqueue(sess);
            }

            for (int i = 0; i < USER_UDP_SESSION_MAX_NUM; ++i)
            {
                sess = new UdpSession(i + 1);
                m_FreeUdpUserSessionQueue.Enqueue(sess);
            }

            for (int i = 0; i < SERVER_TCP_SESSION_MAX_NUM; ++i)
            {
                sess = new TcpSession(i + 1);
                m_FreeServerSessionQueue.Enqueue(sess);
            }

            ValidInitializeOnce();      
        }

        public void Release()
        {
            ValidReleaseOnce();

            foreach (Session sess in m_FreeTcpUserSessionQueue)
            {
                sess.Release();
            }

            m_FreeTcpUserSessionQueue.Clear();
            m_FreeTcpUserSessionQueue = null;

            foreach (Session sess in m_FreeUdpUserSessionQueue)
            {
                sess.Release();
            }

            m_FreeUdpUserSessionQueue.Clear();
            m_FreeUdpUserSessionQueue = null;

            foreach (Session sess in m_FreeServerSessionQueue)
            {
                sess.Release();
            }

            m_FreeServerSessionQueue.Clear();
            m_FreeServerSessionQueue = null;
        }


        public Session AllocateTcpUserSession()
        {
            Session sess = null;

            if (m_FreeTcpUserSessionQueue.Count > 0)
            {
                sess = m_FreeTcpUserSessionQueue.Dequeue();
            }
            
            return sess;
        }

        public void FreeTcpUserSession(Session sess)
        {
            if (null == sess)
            {
                return;
            }

            Debug.Assert(sess is TcpSession);

            m_FreeTcpUserSessionQueue.Enqueue(sess);
        }


        public Session AllocateUdpUserSession()
        {
            Session sess = null;

            if (m_FreeUdpUserSessionQueue.Count > 0)
            {
                sess = m_FreeUdpUserSessionQueue.Dequeue();
            }

            return sess;
        }

        public void FreeUdpUserSession(Session sess)
        {
            if (null == sess)
            {
                return;
            }

            Debug.Assert(sess is UdpSession);

            m_FreeUdpUserSessionQueue.Enqueue(sess);
        }


        public Session AllocateServerSession()
        {
            Session sess = null;

            if (m_FreeServerSessionQueue.Count > 0)
            {
                sess = m_FreeServerSessionQueue.Dequeue();
            }

            return sess;
        }

        public void FreeServerSession(Session sess)
        {
            if (null == sess)
            {
                return;
            }

            Debug.Assert(sess is TcpSession);

            m_FreeServerSessionQueue.Enqueue(sess);
        }



        public int GetTcpUserSessionCount()
        {
            return m_FreeTcpUserSessionQueue.Count;
        }

        public int GetUdpUserSessionCount()
        {
            return m_FreeUdpUserSessionQueue.Count;
        }

        public int GetServerSessionCount()
        {
            return m_FreeServerSessionQueue.Count;
        }
    }
}
