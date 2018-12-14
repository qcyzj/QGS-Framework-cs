using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace Share.Net.Sessions
{
    public sealed class TcpSession : Session
    {
        public TcpSession(int sess_id)
            :base(sess_id)
        {
            m_RecvEventArgs = SocketAsyncEventArgsManager.Instance.AllocateEventArgs();
            m_RecvEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
            m_SendEventArgs = SocketAsyncEventArgsManager.Instance.AllocateEventArgs();
            m_SendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
        }


        private void CreateTcpSocket()
        {
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_Socket.Blocking = false;
            m_Socket.SendBufferSize = DEFAULT_SOCKET_BUF_SIZE;
            m_Socket.ReceiveBufferSize = DEFAULT_SOCKET_BUF_SIZE;

            if (null != m_RecvEventArgs)
            {
                m_RecvEventArgs.UserToken = this;
            }

            if (null != m_SendEventArgs)
            {
                m_SendEventArgs.UserToken = this;
            }
        }

        public void SetTcpSocket(Socket sock)
        {
            Debug.Assert(null != sock);

            m_Socket = sock;
            m_Socket.Blocking = false;
            m_Socket.SendBufferSize = DEFAULT_SOCKET_BUF_SIZE;
            m_Socket.ReceiveBufferSize = DEFAULT_SOCKET_BUF_SIZE;

            if (null != m_RecvEventArgs)
            {
                m_RecvEventArgs.UserToken = this;
            }

            if (null != m_SendEventArgs)
            {
                m_SendEventArgs.UserToken = this;
            }
        }


        public int Connect(object obj, string ip_address, int port)
        {
            SESSION_CONNECT_ERROR ret = SESSION_CONNECT_ERROR.SUCCESS;

            try
            {
                CreateTcpSocket();

                IPAddress address = IPAddress.Parse(ip_address);
                IPEndPoint end_point = new IPEndPoint(address, port);

                SetObject(obj);

                m_Socket.Connect(end_point);
            }
            catch (SocketException sock_ex)
            {
                ret = SESSION_CONNECT_ERROR.E_CONNECT_SOCKET;

                LogManager.Error("Socket connect error: socket exception = " + sock_ex.ErrorCode.ToString(), sock_ex);
            }
            catch (Exception ex)
            {
                ret = SESSION_CONNECT_ERROR.E_CONNECT_OTHER;

                LogManager.Error("Socket connect error: ", ex);
            }

            return (int)ret;
        }

        public void ConnectAsync(object obj, string ip_address, int port)
        {
            Debug.Assert(null != obj);

            try
            {
                CreateTcpSocket();

                IPAddress address = IPAddress.Parse(ip_address);
                IPEndPoint end_point = new IPEndPoint(address, port);

                SetObject(obj);

                m_RecvEventArgs.RemoteEndPoint = end_point;
                m_SendEventArgs.RemoteEndPoint = end_point;
                m_SendEventArgs.SetBuffer(0, 0);

                if (!m_Socket.ConnectAsync(m_SendEventArgs))
                {
                    OnAsyncConnect(m_SendEventArgs);
                }
            }
            catch (SocketException sock_ex)
            {
                LogManager.Error("Socket connect error: socket exception = " + sock_ex.ErrorCode.ToString(), sock_ex);
            }
            catch (Exception ex)
            {
                LogManager.Error("Socket connect error: ", ex);
            }
        }


        private void OnIOCompleted(object sender, SocketAsyncEventArgs args)
        {
            switch (args.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    OnAsyncConnect(args);
                    break;

                case SocketAsyncOperation.Receive:
                    OnAsyncReceive(args);
                    break;

                case SocketAsyncOperation.Send:
                    OnAsyncSend(args);
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }
        }

        private int OnAsyncConnect(SocketAsyncEventArgs args)
        {
            Debug.Assert(m_SendEventArgs == args);

            SESSION_CONNECT_ERROR ret = SESSION_CONNECT_ERROR.SUCCESS;

            if (args.SocketError == SocketError.Success)
            {
                m_State = SESSION_STATE.CONNECTED;
            }
            else
            {
                ret = SESSION_CONNECT_ERROR.E_CONNECT_OTHER;

                LogManager.Error("Socket connect error: error code = " + args.SocketError.ToString());
            }

            return (int)ret;
        }

        public void OnAsyncReceive(SocketAsyncEventArgs args)
        {
            Debug.Assert(m_RecvEventArgs == args);
            Debug.Assert(args.UserToken is TcpSession);

            if (args.BytesTransferred > 0)
            {
                if (SocketError.Success == args.SocketError)
                {
                    TcpSession sess = args.UserToken as TcpSession;

                    sess.ProcessReceive(args);

                    if (null != sess.m_Socket)
                    {
                        if (sess.ProcessSend(m_SendEventArgs))
                        {
                            if (!sess.m_Socket.SendAsync(m_SendEventArgs))
                            {
                                sess.OnAsyncSend(m_SendEventArgs);
                            }
                        }
                        else if (!sess.m_Socket.ReceiveAsync(args))
                        {
                            sess.OnAsyncReceive(args);
                        }
                    }
                }
                else
                {
                    ProcessError(args);
                }
            }
            else
            {
                CloseSession(args);
            }
        }

        private void OnAsyncSend(SocketAsyncEventArgs args)
        {
            Debug.Assert(m_SendEventArgs == args);
            Debug.Assert(args.UserToken is TcpSession);

            if (SocketError.Success == args.SocketError)
            {
                TcpSession sess = args.UserToken as TcpSession;

                if (sess.ProcessSend(args))
                {
                    if (!sess.m_Socket.SendAsync(args))
                    {
                        sess.OnAsyncSend(args);
                    }
                }
                else if (!sess.m_Socket.ReceiveAsync(m_RecvEventArgs))
                {
                    sess.OnAsyncReceive(m_RecvEventArgs);
                }
            }
            else
            {
                ProcessError(args);
            }
        }
    }
}
