using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace Share.Net.Sessions
{
    public sealed class UdpSession : Session
    {
        public UdpSession(int sess_id)
            :base(sess_id)
        {
            m_RecvEventArgs = SocketAsyncEventArgsManager.Instance.AllocateEventArgs();
            m_RecvEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
            m_SendEventArgs = SocketAsyncEventArgsManager.Instance.AllocateEventArgs();
            m_SendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnIOCompleted);
        }


        public Socket CreateUdpSocket(int port, IPEndPoint remote_end_point)
        {
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.port)

            m_Socket.Blocking = false;
            m_Socket.ReceiveBufferSize = DEFAULT_SOCKET_BUF_SIZE;

            IPAddress address = IPAddress.Loopback;
            IPEndPoint end_point = new IPEndPoint(address, port);

            m_Socket.Bind(end_point);

            m_Socket.Connect(remote_end_point);

            if (null != m_RecvEventArgs)
            {
                m_RecvEventArgs.UserToken = this;
            }

            if (null != m_SendEventArgs)
            {
                m_SendEventArgs.UserToken = this;
            }

            return m_Socket;
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

        public void OnAsyncReceiveFrom(SocketAsyncEventArgs args)
        {
            Debug.Assert(m_RecvEventArgs == args);
            Debug.Assert(args.UserToken is UdpSession);

            if (args.BytesTransferred > 0)
            {
                if (SocketError.Success == args.SocketError)
                {
                    UdpSession sess = args.UserToken as UdpSession;

                    sess.ProcessReceive(args);

                    if (0 == sess.m_Socket.Available)
                    {
                        sess.ProcessSend(m_SendEventArgs);

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

                if (null != sess && null != sess.m_Socket)
                {
                    if (!sess.m_Socket.ReceiveFromAsync(m_RecvEventArgs))
                    {
                        sess.OnAsyncReceiveFrom(m_RecvEventArgs);
                    }
                }
            }
            else
            {
                ProcessError(args);
            }
        }
    }
}
