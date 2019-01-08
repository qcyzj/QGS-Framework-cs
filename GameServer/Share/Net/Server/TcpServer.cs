using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

using Share.Config;
using Share.Net.Sessions;

namespace Share.Net.Server
{
    public abstract class TcpServer
    {
        public enum SOCKET_BLOCKING_TYPE
        {
            BLOCKING = 0,
            NON_BLOCKING,
        }

        public enum SOCK_SERV_ERROR : int
        {
            SUCCESS = 0,
            E_USER_SESSION_IS_EMPTY = -1,
            E_SERVER_SESSION_IS_EMPTY = -2,
        }


        private const int PENDING_CONNECTION_QUEUE = 100;


        private Socket m_ListenSocket;
        private int m_Port;
        private SOCKET_BLOCKING_TYPE m_BlockingType;


        public TcpServer(TcpServer.SOCKET_BLOCKING_TYPE blocking_type, int port)
        {
            m_ListenSocket = null;

            m_Port = port;
            m_BlockingType = blocking_type;
        }


        public virtual void Start()
        {
            CreateListenSocket();

            StartAsyncAccept(null);
        }

        public virtual void Stop()
        {
            CloseListenSocket();
        }


        private void CreateListenSocket()
        {
            m_ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_ListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            IPAddress address = IPAddress.Parse(ConfigManager.Instance.LOCAL_IP_ADDRESS);
            IPEndPoint end_point = new IPEndPoint(address, m_Port);

            m_ListenSocket.Bind(end_point);
            m_ListenSocket.Listen(PENDING_CONNECTION_QUEUE);
            m_ListenSocket.Blocking = IsSocketBlocking();

            LogManager.Info("Create tcp server listen socket: " + end_point.ToString());
        }
         
        private void CloseListenSocket()
        {
            m_ListenSocket.Close();
            m_ListenSocket = null;
        }

        private bool IsSocketBlocking()
        {
            return SOCKET_BLOCKING_TYPE.BLOCKING == m_BlockingType;
        }


        protected abstract Session AllocateSession();

        protected abstract void FreeSession(Session sess);

        protected abstract SOCK_SERV_ERROR AddToRelevantManager(Session sess);


        private void StartAsyncAccept(SocketAsyncEventArgs accpet_args)
        {
            if (null == accpet_args)
            {
                accpet_args = new SocketAsyncEventArgs();
                accpet_args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAccpetCompleted);
            }
            else
            {
                accpet_args.AcceptSocket = null;
            }

            if (!m_ListenSocket.AcceptAsync(accpet_args))
            {
                ProcessAsyncAccept(accpet_args);
            }
        }

        private void OnAccpetCompleted(object sender, SocketAsyncEventArgs args)
        {
            Debug.Assert(args.LastOperation == SocketAsyncOperation.Accept);

            ProcessAsyncAccept(args);
        }

        private void ProcessAsyncAccept(SocketAsyncEventArgs accpet_args)
        {
            Debug.Assert(null != accpet_args);

            if (SocketError.Success == accpet_args.SocketError)
            {
                Socket new_socket = accpet_args.AcceptSocket;

                if (new_socket.Connected)
                {
                    try
                    {
                        TcpSession sess = (TcpSession)AllocateSession();

                        if (null != sess)
                        {
                            sess.SetTcpSocket(new_socket);

                            if (SOCK_SERV_ERROR.SUCCESS != AddToRelevantManager(sess))
                            {
                                FreeSession(sess);

                                new_socket = null;
                            }
                            else
                            {
                                if (!new_socket.ReceiveAsync(sess.RecvEventArgs))
                                {
                                    sess.OnAsyncReceive(sess.RecvEventArgs);
                                }
                            }
                        }
                        else
                        {
                            new_socket = null;

                            LogManager.Error("Socket server accept error: allocate session failed!");
                        }
                    }
                    catch (SocketException socket_ex)
                    {
                        LogManager.Error("Socket server accpet error: socket exception = " + socket_ex.ErrorCode.ToString(), socket_ex);
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error("Socket server accept error: ", ex);
                    }

                    StartAsyncAccept(accpet_args);
                }
            }
            else
            {
                // SocketError.OperationAborted   
            }
        }
    }
}
