using System.Diagnostics;
using System.Collections.Generic;

using Share;
using Share.Net.Packets;
using Share.Net.Sessions;

namespace GatewayServer.Gateway.User
{
    public class UserManager : Singleton<UserManager>
    {
        private Queue<User> m_FreeUserQueue;

        private List<User> m_ConnectedUserList;
        private Dictionary<uint, User> m_AuthedUserDict;
        private object m_AuthedLock;
        private object m_ConnectLock;

        private Dictionary<uint, User> m_ConnectlessUserDict;
        private object m_LessLock;


        public UserManager()
        {
            m_FreeUserQueue = new Queue<User>();

            m_ConnectedUserList = new List<User>();
            m_AuthedUserDict = new Dictionary<uint, User>();

            m_AuthedLock = new object();
            m_ConnectLock = new object();

            m_ConnectlessUserDict = new Dictionary<uint, User>();
            m_LessLock = new object();
        }


        public void Initialize()
        {
            User user = null;
            int user_count = SessionManager.USER_TCP_SESSION_MAX_NUM + SessionManager.USER_UDP_SESSION_MAX_NUM;

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

            lock (m_ConnectLock)
            {
                foreach (User tmp_user in m_ConnectedUserList)
                {
                    tmp_user.Release();
                }
            }

            m_ConnectedUserList.Clear();
            m_ConnectedUserList = null;

            lock (m_AuthedLock)
            {
                foreach (User tmp_user in m_AuthedUserDict.Values)
                {
                    tmp_user.Release();
                }
            }

            m_AuthedUserDict.Clear();
            m_AuthedUserDict = null;
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

        private void FreeUser(User user)
        {
            if (null == user)
            {
                return;
            }

            m_FreeUserQueue.Enqueue(user);
        }


        public void AddConnectedUser(User user)
        {
            lock (m_ConnectLock)
            {
                Debug.Assert(!m_ConnectedUserList.Contains(user));

                if (!m_ConnectedUserList.Contains(user))
                {
                    m_ConnectedUserList.Add(user);
                }
                else
                {
                    Debug.Assert(false);
                }
            }
            
            LogManager.Debug("Add connected user. UserID = " + user.UserID);
        }

        public void AddAuthedUser(User user)
        {
            lock (m_ConnectLock)
            {
                int index = m_ConnectedUserList.IndexOf(user);

                if (index > -1)
                {
                    m_ConnectedUserList.RemoveAt(index);
                }
                else
                {
                    Debug.Assert(false);
                }
            }

            User tmp_user = null;

            lock (m_AuthedLock)
            {
                if (m_AuthedUserDict.TryGetValue(user.UserID, out tmp_user))
                {
                    Debug.Assert(false);
                    m_AuthedUserDict.Remove(user.UserID);

                    FreeUser(tmp_user);
                }

                m_AuthedUserDict.Add(user.UserID, user);
            }


            LogManager.Debug("Add authed user. UserID = " + user.UserID);
        }

        public void RemoveUser(User user)
        {
            //Debug.Assert(User.INVALID_USER_ID != user.UserID);

            lock (m_AuthedLock)
            {
                if (m_AuthedUserDict.ContainsKey(user.UserID))
                {
                    m_AuthedUserDict.Remove(user.UserID);
                }
            }

            lock (m_ConnectLock)
            {
                int index = m_ConnectedUserList.IndexOf(user);

                if (index > -1)
                {
                    m_ConnectedUserList.RemoveAt(index);
                }
            }

            LogManager.Debug("Remove connected user. UserID = " + user.UserID);

            FreeUser(user);
        }


        public void AddConnectlessUser(User user)
        {
            User tmp_user = null;

            lock (m_LessLock)
            {
                if (m_ConnectlessUserDict.TryGetValue(user.UserID, out tmp_user))
                {
                    Debug.Assert(false);
                    m_ConnectlessUserDict.Remove(user.UserID);

                    FreeUser(tmp_user);
                }

                m_ConnectlessUserDict.Add(user.UserID, user);
            }
        }


        private void RegisterUserProc()
        {
            RegisterProcImpl(Protocol.CLI_GW_ENTER_AUTH, User.PacketProcessAuth);
            RegisterProcImpl(Protocol.CLI_GW_ENTER_TEST, User.PacketProcessTest);
            RegisterProcImpl(Protocol.CLI_GW_ENTER_TEST_2, User.PacketProcessTest_2);

            RegisterProcImpl(Protocol.CLI_GW_ENTER_REGISTER, User.PacketProcessEnterRegister);
            RegisterProcImpl(Protocol.CLI_GW_ENTER_LOGIN, User.PacketProcessEnterLogin);


            RegisterProcImpl(Protocol.UDP_CLI_GW_CONNECT, User.PaacketProcessUdpConnect);
            RegisterProcImpl(Protocol.UDP_CLI_GW_AUTH, User.PacketProcessUdpAuth);
            RegisterProcImpl(Protocol.UDP_CLI_GW_TEST, User.PacketProcessUdpTest);
            RegisterProcImpl(Protocol.UDP_CLI_GW_TEST_2, User.PacketProcessUdpTest_2);
        }

        private void RegisterProcImpl(int protocol, PacketProcessManager.PacketProcessFunc func)
        {
            PacketProcessManager.Instance.RegisterProc(protocol, func);
        }


        public int GetFreeUserCount()
        {
            return m_FreeUserQueue.Count;
        }

        public int GetAuthedUserCount()
        {
            return m_AuthedUserDict.Count;
        }

        public int GetConnectedUserCount()
        {
            return m_ConnectedUserList.Count;
        }
    }
}