
using System.Diagnostics;

using Share.Net.Packets;
using Share.Net.Sessions;

namespace Client.Users
{
    public partial class User
    {
        public const uint INVALID_USER_ID = 0;


        private Session m_UserSession;
        private uint m_UserID;


        public uint UserID { get { return m_UserID; } }


        public User()
        {
            m_UserSession = null;
            m_UserID = INVALID_USER_ID;
        }


        public void Release()
        {
            if (null != m_UserSession)
            {
                m_UserSession.Release();
            }

            m_UserSession = null;
            m_UserID = INVALID_USER_ID;
        }


        public void SetUserSession(Session user_sess)
        {
            Debug.Assert(null != user_sess);
            m_UserSession = user_sess;
        }


        public void SetUserID(uint user_id)
        {
            Debug.Assert(INVALID_USER_ID == m_UserID);
            Debug.Assert(INVALID_USER_ID != user_id);

            m_UserID = user_id;
        }


        public int ConnectToGateway(string ip_addr, int port)
        {
            Debug.Assert(m_UserSession is TcpSession);
            TcpSession tcp_session = m_UserSession as TcpSession;

            int ret = tcp_session.Connect(this, ip_addr, port);

            if ((int)Session.SESSION_CONNECT_ERROR.SUCCESS == ret)
            {}

            return ret;
        }

        public void ConnectToGatewayAsync(string ip_addr, int port)
        {
            Debug.Assert(m_UserSession is TcpSession);
            TcpSession tcp_session = m_UserSession as TcpSession;

            tcp_session.ConnectAsync(this, ip_addr, port);
        }

        public static void OnAsyncConnected(object obj)
        {
            Debug.Assert(null != obj);
            Debug.Assert(obj is User);

            User user = obj as User;
            UserManager.Instance.AddConnectedUser(user);

            user.SendAuthPacket();
            user.StartSendAsync();
        }

        public void StartSendAsync()
        {
            Debug.Assert(m_UserSession is TcpSession);
            TcpSession tcp_session = m_UserSession as TcpSession;

            tcp_session.StartSendAsync();        
        }


        public void StartSendToAsync(string ip_addr, int port)
        {
            Debug.Assert(m_UserSession is UdpSession);
            UdpSession udp_session = m_UserSession as UdpSession;

            udp_session.CreateUdpSocket(this, ip_addr, port);
            udp_session.StartSendToAsync();
        }

        public void SetRemotePort(int port)
        {
            Debug.Assert(m_UserSession is UdpSession);
            UdpSession udp_sess = m_UserSession as UdpSession;

            udp_sess.SetRemotePort(port);
        }


        private void SendPacket(Packet pkt)
        {
            if (null != m_UserSession)
            {
                m_UserSession.SendPacket(pkt);
            }

            PacketManager.Instance.ReleasePacket(pkt);
        }
    }
}
