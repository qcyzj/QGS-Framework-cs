using System;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;

using Share.Logs;
using Share.Net.Sessions;

using GatewayServer.Gateway.Users;

namespace GatewayServer.Test
{
    public class TestGatewayUser
    {
        public TestGatewayUser()
        { }

        public void RunAllTest()
        {
            Test_Gateway_User_UserSocketServer();

            Test_Gateway_User_UserConnectManager();

            Test_Gateway_User_User();

            Test_Gateway_User_UserManager();

            Test_Gateway_User_UserPacketProcess();
        }


        private void Test_Gateway_User_UserSocketServer()
        {
            // test in UserConnectManager
        }

        private void Test_Gateway_User_User()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                LogManager.Warn(MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private void Test_Gateway_User_UserConnectManager()
        {
            UserConnectManager.Instance.Start();
            LogManager.Info("testing user connect manager start.");

            while (true)
            {
                Thread.Sleep(5 * 1000);
                break;
            }

            UserConnectManager.Instance.Stop();
            LogManager.Info("testing user connect manager stop.");
        }

        private void Test_Gateway_User_UserManager()
        {
            CAssert.AreEqual(SessionManager.USER_TCP_SESSION_MAX_NUM + SessionManager.USER_UDP_SESSION_MAX_NUM, 
                             UserManager.Instance.GetFreeUserCount());

            //Session sess = SessionManager.Instance.AllocateUserSession();


            //UserManager.Instance.AddConnectedUser(user);

            //UserManager.Instance.AddLoginedUser(user);

            //UserManager.Instance.ProcessReceive();



            //CAssert.AreEqual(SessionManager.USER_SESSION_MAX_NUM,
            //                 UserManager.Instance.GetFreeUserCount());




            User user = null;
            List<User> user_list = new List<User>();

            for (int i = 0; i < SessionManager.USER_TCP_SESSION_MAX_NUM; ++i)
            {
                user = UserManager.Instance.AllocateUser();
                CAssert.IsNotNull(user);

                user_list.Add(user);
                user = null;
            }

            for (int i = 0; i < SessionManager.USER_UDP_SESSION_MAX_NUM; ++i)
            {
                user = UserManager.Instance.AllocateUser();
                CAssert.IsNotNull(user);

                user_list.Add(user);
                user = null;
            }

            CAssert.AreEqual(0, UserManager.Instance.GetFreeUserCount());
        }

        private void Test_Gateway_User_UserPacketProcess()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                LogManager.Warn(MethodBase.GetCurrentMethod().Name, ex);
            }
        }
    }
}
