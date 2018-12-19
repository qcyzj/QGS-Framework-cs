using System.Collections.Generic;

using Share;
using Share.Net.Packets;
using Share.Net.Sessions;

namespace GameServer.GameServer.User
{
    public class UserManager : Singleton<UserManager>
    {
        private Queue<User> m_FreeUserQueue;

        private Dictionary<uint, User> m_UserDict;



        public UserManager()
        {
            m_FreeUserQueue = new Queue<User>();
            m_UserDict = new Dictionary<uint, User>();
        }


        public void Initialize()
        {
            User user = null;
            int user_count = SessionManager.USER_TCP_SESSION_MAX_NUM + 
                                SessionManager.USER_UDP_SESSION_MAX_NUM;

            for (int i = 0; i < user_count; ++i)
            {
                user = new User();
                m_FreeUserQueue.Enqueue(user);
                user = null;
            }

            RegisterUserProc();

            ValidInitializeOnce();
        }

        public void Release()
        {
            ValidReleaseOnce();

            User user = null;

            while (m_FreeUserQueue.Count > 0)
            {
                user = m_FreeUserQueue.Dequeue();
                user.Release();
            }

            m_FreeUserQueue = null;

            foreach (User tmp_user in m_UserDict.Values)
            {
                tmp_user.Release();
            }

            m_UserDict.Clear();
            m_UserDict = null;
        }


        public User AllocateUser()
        {
            User user = null;

            if (m_FreeUserQueue.Count > 0)
            {
                user = m_FreeUserQueue.Dequeue();
            }

            return user;
        }

        public void FreeUser(User user)
        {
            if (null == user)
            {
                return;
            }

            m_FreeUserQueue.Enqueue(user);
        }


        private void RegisterUserProc()
        {
            //RegisterProcImpl();
            //RegisterProcImpl();
        }

        private void RegisterProcImpl(int protocol, PacketProcessManager.PacketProcessFunc func)
        {
            PacketProcessManager.Instance.RegisterProc(protocol, func);
        }
    }
}
