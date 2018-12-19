using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

using Share.Config;

namespace Share.Net.Sessions
{
    public sealed class UdpSession : Session
    {
        public UdpSession(int sess_id)
            : base(sess_id)
        {
            m_RecvEventArgs = SocketAsyncEventArgsManager.Instance.AllocateEventArgs();
            m_RecvEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
            m_SendEventArgs = SocketAsyncEventArgsManager.Instance.AllocateEventArgs();
            m_SendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
        }


        public Socket CreateUdpSocket(int local_port, IPEndPoint remote_end_point)
        {
            try
            {
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
 
                m_Socket.Blocking = false;
                m_Socket.ReceiveBufferSize = DEFAULT_SOCKET_BUF_SIZE;

                IPAddress address = IPAddress.Parse(ConfigManager.LOCAL_IP_ADDRESS);
                IPEndPoint end_point = new IPEndPoint(address, local_port);

                m_Socket.Bind(end_point);

                m_Socket.Connect(remote_end_point);

                if (null != m_RecvEventArgs)
                {
                    m_RecvEventArgs.UserToken = this;
                    m_RecvEventArgs.RemoteEndPoint = remote_end_point;
                }

                if (null != m_SendEventArgs)
                {
                    m_SendEventArgs.UserToken = this;
                    m_SendEventArgs.RemoteEndPoint = remote_end_point;
                }
            }
            catch (SocketException sock_ex)
            {
                LogManager.Error("Create udp socket connect error: socket exception = " + sock_ex.ErrorCode.ToString(), sock_ex);
            }
            catch (Exception ex)
            {
                LogManager.Error("Create udp socket connect error: ", ex);
            }

            return m_Socket;
        }

        public void CreateUdpSocket(object obj, string ip_addr, int port)
        {
            try
            {
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                m_Socket.Blocking = false;
                m_Socket.ReceiveBufferSize = DEFAULT_SOCKET_BUF_SIZE;

                SetObject(obj);

                IPAddress local_address = IPAddress.Parse(ConfigManager.LOCAL_IP_ADDRESS);
                IPEndPoint local_end_point = new IPEndPoint(local_address, 0);

                m_Socket.Bind(local_end_point);

                IPAddress remote_address = IPAddress.Parse(ip_addr);
                IPEndPoint remote_end_point = new IPEndPoint(remote_address, port);

                if (null != m_RecvEventArgs)
                {
                    m_RecvEventArgs.UserToken = this;
                    m_RecvEventArgs.RemoteEndPoint = remote_end_point;
                }

                if (null != m_SendEventArgs)
                {
                    m_SendEventArgs.UserToken = this;
                    m_SendEventArgs.RemoteEndPoint = remote_end_point;
                }
            }
            catch (SocketException sock_ex)
            {
                LogManager.Error("Create udp socket connect error: socket exception = " + sock_ex.ErrorCode.ToString(), sock_ex);
            }
            catch (Exception ex)
            {
                LogManager.Error("Create udp socket connect error: ", ex);
            }
        }

        public void SetRemotePort(int remote_port)
        {
            Debug.Assert(null != this.Object);

            try
            {
                IPAddress address = IPAddress.Parse(ConfigManager.LOCAL_IP_ADDRESS);
                IPEndPoint end_point = new IPEndPoint(address, remote_port);

                m_Socket.Connect(end_point);

                if (null != m_RecvEventArgs)
                {
                    m_RecvEventArgs.RemoteEndPoint = end_point;
                }

                if (null != m_SendEventArgs)
                {
                    m_SendEventArgs.RemoteEndPoint = end_point;
                }
            }
            catch (SocketException sock_ex)
            {
                LogManager.Error("Set udp socket connect error: socket exception = " + sock_ex.ErrorCode.ToString(), sock_ex);
            }
            catch (Exception ex)
            {
                LogManager.Error("Set udp socket connect error: ", ex);
            }
        }


        private void OnIOCompleted(object sender, SocketAsyncEventArgs args)
        {
            switch (args.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    OnAsyncReceiveFrom(args);
                    break;

                case SocketAsyncOperation.SendTo:
                    OnAsyncSendTo(args);
                    break;

                default:
                    Debug.Assert(false);
                    break;
            }
        }

        private void OnAsyncReceiveFrom(SocketAsyncEventArgs args)
        {
            Debug.Assert(m_RecvEventArgs == args);
            Debug.Assert(args.UserToken is UdpSession);

            if (args.BytesTransferred > 0)
            {
                if (SocketError.Success == args.SocketError)
                {
                    UdpSession sess = args.UserToken as UdpSession;

                    //DebugUdpRecvLog(sess.m_Socket, args);

                    sess.ProcessReceive(args);

                    if (sess.ProcessSend(m_SendEventArgs))
                    {
                        if (!sess.m_Socket.SendToAsync(m_SendEventArgs))
                        {
                            sess.OnAsyncSendTo(m_SendEventArgs);
                        }
                    }
                    else if (!sess.m_Socket.ReceiveFromAsync(args))
                    {
                        sess.OnAsyncReceiveFrom(args);
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

        private void OnAsyncSendTo(SocketAsyncEventArgs args)
        {
            Debug.Assert(m_SendEventArgs == args);
            Debug.Assert(args.UserToken is UdpSession);

            if (SocketError.Success == args.SocketError)
            {
                UdpSession sess = args.UserToken as UdpSession;

                //if (null != sess && null != sess.m_Socket)
                //{
                    //DebugUdpSendLog(sess.m_Socket, args);

                    if (sess.ProcessSend(args))
                    {
                        if (!sess.m_Socket.SendToAsync(args))
                        {
                            sess.OnAsyncSendTo(args);
                        }
                    }
                    else if (!sess.m_Socket.ReceiveFromAsync(m_RecvEventArgs))
                    {
                        sess.OnAsyncReceiveFrom(m_RecvEventArgs);
                    }
                //}
            }
            else
            {
                ProcessError(args);
            }
        }


        public void StartSendToAsync()
        {
            ProcessSend(m_SendEventArgs);

            if (!m_Socket.SendToAsync(m_SendEventArgs))
            {
                OnAsyncSendTo(m_SendEventArgs);
            }
        }


        private void DebugUdpSendLog(Socket socket, SocketAsyncEventArgs send_args)
        {
            Debug.Assert(null != socket);
            Debug.Assert(null != send_args);

            LogManager.Error("Send Udp. socket local end point = " + socket.LocalEndPoint.ToString());
            LogManager.Error("Send Udp. args remote end point " + send_args.RemoteEndPoint.ToString() +
                             " bytes transferred = " + send_args.BytesTransferred);
        }

        private void DebugUdpRecvLog(Socket socket, SocketAsyncEventArgs recv_args)
        {
            Debug.Assert(null != socket);
            Debug.Assert(null != recv_args);

            LogManager.Error("Recv Udp. socket local end point = " + socket.LocalEndPoint.ToString());
            LogManager.Error("Recv Udp. args remote end point " + recv_args.RemoteEndPoint.ToString() +
                             " bytes transferred = " + recv_args.BytesTransferred);
        }
    }
}
