using System.Net;
using System.Diagnostics;
using System.Net.Sockets;

using Share.Net.Sessions;

namespace Share.Net.Server
{
    public abstract class UdpServer
    {
        public const int PORT = 12001;

        public enum SOCK_SERV_ERROR : int
        {
            SUCCESS = 0,
            E_USER_SESSION_IS_EMPTY = -1,
            E_SERVER_SESSION_IS_EMPTY = -2,
        }


        private Socket m_ListenSocket;
        private int m_Port;


        public UdpServer(int port)
        {
            m_ListenSocket = null;
            m_Port = port;
        }


        public virtual void Start()
        {
            CreateSocket();

            StartAsyncReceive(null);
        }

        public virtual void Stop()
        {
            CloseSocket();
        }


        private void CreateSocket()
        {
            m_ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_ListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint end_point = new IPEndPoint(address, m_Port);

            m_ListenSocket.Bind(end_point);

            LogManager.Info("create udp sokcet, port = " + m_Port.ToString());
        }

        private void CloseSocket()
        {
            m_ListenSocket.Shutdown(SocketShutdown.Both);
            m_ListenSocket.Close();
            m_ListenSocket = null;
        }


        protected abstract Session AllocateSession();

        protected abstract void FreeSession(Session sess);

        protected abstract SOCK_SERV_ERROR AddToRelevantManager(Session sess);


        private void StartAsyncReceive(SocketAsyncEventArgs args)
        {
            if (null == args)
            {
                args = new SocketAsyncEventArgs();
                args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                args.Completed += new System.EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
            }
            else
            {
                args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            }

            if (!m_ListenSocket.ReceiveFromAsync(args))
            {
                ProcessAsyncReceiveFrom(args);
            }
        }

        private void OnReceiveCompleted(object send, SocketAsyncEventArgs args)
        {
            Debug.Assert(args.LastOperation == SocketAsyncOperation.ReceiveFrom);

            ProcessAsyncReceiveFrom(args);
        }

        private void ProcessAsyncReceiveFrom(SocketAsyncEventArgs args)
        {
            Debug.Assert(null != args);
            Debug.Assert(SocketError.Success == args.SocketError);

            if (SocketError.Success == args.SocketError)
            {
                IPEndPoint remote_end_point = (IPEndPoint)args.RemoteEndPoint;

                UdpSession sess = (UdpSession)AllocateSession();

                if (null != sess)
                {
                    Socket socket = sess.CreateUdpSocket(PORT, remote_end_point);

                    if (SOCK_SERV_ERROR.SUCCESS != AddToRelevantManager(sess))
                    {
                        FreeSession(sess);
                    }
                    else
                    {
                        if (!socket.ReceiveFromAsync(sess.RecvEventArgs))
                        {
                            sess.OnAsyncReceiveFrom(sess.RecvEventArgs);
                        }
                    }
                }
                else
                {
                    LogManager.Error("Socket server receive error: allocate session failed!");
                }
            }
            
            StartAsyncReceive(args);
        }
    }
}
