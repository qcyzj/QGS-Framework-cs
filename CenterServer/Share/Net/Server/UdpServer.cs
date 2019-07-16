using System.Net;
using System.Diagnostics;
using System.Net.Sockets;

using Share.Logs;
using Share.Config;
using Share.Net.Packets;
using Share.Net.Sessions;

namespace Share.Net.Server
{
    public abstract class UdpServer
    {
        public enum SOCK_SERV_ERROR : int
        {
            SUCCESS = 0,
            E_USER_SESSION_IS_EMPTY = -1,
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
            CreateListenSocket();

            StartAsyncReceive(null);
        }

        public virtual void Stop()
        {
            CloseSocket();
        }


        private void CreateListenSocket()
        {
            m_ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_ListenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            IPAddress address = IPAddress.Parse(ConfigManager.Instance.LOCAL_IP_ADDRESS);
            IPEndPoint end_point = new IPEndPoint(address, m_Port);

            m_ListenSocket.Bind(end_point);

            LogManager.Info("Create udp server sokcet: " + end_point.ToString());
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
                args.SetBuffer(new byte[Packet.PACKET_HEAD_LENGTH], 0, Packet.PACKET_HEAD_LENGTH);
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
                //DebugUdpRecvLog(m_ListenSocket, args);

                IPEndPoint remote_end_point = (IPEndPoint)args.RemoteEndPoint;

                UdpSession sess = (UdpSession)AllocateSession();

                if (null != sess)
                {
                    int udp_port = UdpPortManager.Instance.AllocatePort();

                    Socket socket = sess.CreateUdpSocket(udp_port, remote_end_point);

                    if (SOCK_SERV_ERROR.SUCCESS != AddToRelevantManager(sess))
                    {
                        FreeSession(sess);
                    }
                    else
                    {
                        sess.ProcessSend(sess.SendEventArgs);
                        
                        if (!socket.SendToAsync(sess.SendEventArgs))
                        {
                            sess.OnAsyncSendTo(sess.SendEventArgs);
                        }
                    }
                }
                else
                {
                    LogManager.Error("Socket server receive from error: allocate session failed!");
                }

                StartAsyncReceive(args);
            }
            else
            {   
                // SocketError.OperationAborted   
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

            LogManager.Error("Acpt Udp. socket local end point = " + socket.LocalEndPoint.ToString());
            LogManager.Error("Acpt Udp. args remote end point " + recv_args.RemoteEndPoint.ToString() +
                             " bytes transferred = " + recv_args.BytesTransferred);
        }
    }
}
