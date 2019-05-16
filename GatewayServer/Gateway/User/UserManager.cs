using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Share;
using Share.Net.Packets;
using Share.Net.Sessions;

namespace GatewayServer.Gateway.Users
{
    public class UserManager : Singleton<UserManager>
    {
        private Queue<User> m_FreeUserQueue;

        private LightConcurrentList<User> m_ConnectedUserList;
        private ConcurrentDictionary<uint, User> m_AuthedUserDict;

        private ConcurrentDictionary<uint, User> m_ConnectlessUserDict;


        public UserManager()
        {
            m_FreeUserQueue = new Queue<User>();

            m_ConnectedUserList = new LightConcurrentList<User>();
            m_AuthedUserDict = new ConcurrentDictionary<uint, User>();

            m_ConnectlessUserDict = new ConcurrentDictionary<uint, User>();
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

            foreach (User tmp_user in m_ConnectedUserList)
            {
                tmp_user.Release();
            }

            m_ConnectedUserList.Clear();
            m_ConnectedUserList = null;

            foreach (User tmp_user in m_AuthedUserDict.Values)
            {
                tmp_user.Release();
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
            Debug.Assert(!m_ConnectedUserList.Contains(user));

            m_ConnectedUserList.TryAdd(user);
            
            LogManager.Debug("Add connected user. UserID = " + user.UserID);
        }

        public void AddAuthedUser(User user)
        {
            Debug.Assert(m_ConnectedUserList.IndexOf(user) > -1);

            if (m_ConnectedUserList.TryRemove(user))
            {}
            else
            {
                Debug.Assert(false);
            }

            if (m_AuthedUserDict.TryRemove(user.UserID, out User tmp_user))
            {
                Debug.Assert(false);
                FreeUser(tmp_user);
            }

            m_AuthedUserDict.TryAdd(user.UserID, user);

            LogManager.Debug("Add authed user. UserID = " + user.UserID);
        }

        public void RemoveUser(User user)
        {
            //Debug.Assert(User.INVALID_USER_ID != user.UserID);

            m_AuthedUserDict.TryRemove(user.UserID, out User tmp_user);

            m_ConnectedUserList.TryRemove(user);

            LogManager.Debug("Remove connected user. UserID = " + user.UserID);

            FreeUser(user);
        }


        public void AddConnectlessUser(User user)
        {
            if (m_ConnectlessUserDict.TryRemove(user.UserID, out User tmp_user))
            {
                FreeUser(tmp_user);
            }

            m_ConnectlessUserDict.TryAdd(user.UserID, user);

            LogManager.Debug("Add connectless user. UserID = " + user.UserID);
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
            return m_ConnectedUserList.Count();
        }
    }
}