using System;
using System.Net;
using System.Diagnostics;
using System.Net.Sockets;

using Share.Net.Buffer;
using Share.Net.Packets;

using GatewayServer.Gateway.User;

namespace Share.Net.Sessions
{
    public abstract class Session
    {
        protected enum SESSION_STATE
        {
            CLOSED = 0,
            CONNECTED,
            AUTHED,
        }

        public enum SESSION_ERROR : int
        {
            SUCCESS = 0,
            E_RECEIVE_DATA = -1,
            E_SEND_DATA = -2,
            E_DISCONNECTED = -3,
            E_PACKET_SIZE = -4,
            E_PACKET_PROC = -5,
        }

        public enum SESSION_CONNECT_ERROR : int
        {
            SUCCESS = 0,
            E_CONNECT_SOCKET = -1,
            E_CONNECT_OTHER = -2,
        }


        protected const int DEFAULT_SOCKET_BUF_SIZE = 8192;
        private const int INVALID_SESSION_INDEX = -1;


        private ReadWriteBuffer m_SendBuffer;
        private ReadWriteBuffer m_ReceiveBuffer;
        protected Socket m_Socket;
        protected SocketAsyncEventArgs m_RecvEventArgs;
        protected SocketAsyncEventArgs m_SendEventArgs;
        protected SESSION_STATE m_State;
        private int m_SessionID;
        private object m_Object;


        public SocketAsyncEventArgs RecvEventArgs { get { return m_RecvEventArgs; } }
        public SocketAsyncEventArgs SendEventArgs { get { return m_SendEventArgs; } }
        public Socket Socket { get { return m_Socket; } }


        public Session(int sess_id)
        {
            m_State = SESSION_STATE.CLOSED;
            m_SessionID = sess_id;

            m_Socket = null;
            m_SendBuffer = new ReadWriteBuffer(BufferManager.Instance.AllocateBuffer());
            m_ReceiveBuffer = new ReadWriteBuffer(BufferManager.Instance.AllocateBuffer());
        }


        public void Release()
        {
            m_State = SESSION_STATE.CLOSED;
            m_SessionID = INVALID_SESSION_INDEX;

            CloseSocket();

            m_SendBuffer.Release();
            m_ReceiveBuffer.Release();
        }


        private void CloseSocket()
        {
            Debug.Assert(null != m_RecvEventArgs);
            Debug.Assert(null != m_SendEventArgs);

            m_State = SESSION_STATE.CLOSED;

            if (null != m_Socket)
            {
                m_Socket.Shutdown(SocketShutdown.Both);
                m_Socket.Close();
                m_Socket = null;
            }

            if (null != m_RecvEventArgs)
            {
                m_RecvEventArgs.UserToken = null;
            }

            if (null != m_SendEventArgs)
            {
                m_SendEventArgs.UserToken = null;
            }

            if (null != m_Object)
            {
                m_Object = null;
            }

            m_SendBuffer.SetEmpty();
            m_ReceiveBuffer.SetEmpty();
        }


        public void SetObject(object obj)
        {
            Debug.Assert(null == m_Object);
            m_Object = obj;
        }


        protected void ProcessReceive(SocketAsyncEventArgs args)
        {
            Debug.Assert(null != args);
            Debug.Assert(m_RecvEventArgs == args);
            Debug.Assert(m_ReceiveBuffer.GetCanWriteSize() >= args.BytesTransferred);

            Array.Copy(args.Buffer, args.Offset, m_ReceiveBuffer.Buffer, m_ReceiveBuffer.WriteIndex, args.BytesTransferred);
            m_ReceiveBuffer.AddWriteSize(args.BytesTransferred);

            int ret = ProcessPackets();

            if ((int)SESSION_ERROR.SUCCESS != ret)
            { }
        }

        public bool ProcessSend(SocketAsyncEventArgs args)
        {
            Debug.Assert(null != args);
            Debug.Assert(m_SendEventArgs == args);

            if (m_SendBuffer.GetCanReadSize() <= 0)
            {
                return false;
            }

            int buf_size = m_SendBuffer.GetCanReadSize();
            Debug.Assert(buf_size >= Packet.PACKET_HEAD_LENGTH);

            Array.Copy(m_SendBuffer.Buffer, m_SendBuffer.ReadIndex, args.Buffer, 0, buf_size);
            args.SetBuffer(0, buf_size);

            m_SendBuffer.AddReadSize(buf_size);
            return true;
        }

        private int ProcessPackets()
        {
            Debug.Assert(null != m_Object);

            int ret = (int)SESSION_ERROR.SUCCESS;

            if (m_ReceiveBuffer.GetCanReadSize() < Packet.PACKET_HEAD_LENGTH)
            {
                return ret;
            }

            Packet pkt = PacketManager.Instance.AllocatePacket();

            m_ReceiveBuffer.PeekPacketHead(pkt.Buf);
            pkt.SetSize();

            if (pkt.Size < Packet.PACKET_HEAD_LENGTH)
            {
                // 消息头部未接收全

                PacketManager.Instance.ReleasePacket(pkt);
                return ret;
            }

            if (m_ReceiveBuffer.GetCanReadSize() < pkt.Size)
            {
                // 消息未接收全

                PacketManager.Instance.ReleasePacket(pkt);
                return ret;
            }

            if (pkt.Size > Packet.DEFAULT_PACKET_BUF_SIZE)
            {
                // 消息大小异常

                PacketManager.Instance.ReleasePacket(pkt);

                ret = (int)SESSION_ERROR.E_PACKET_SIZE;
                return ret;
            }

            int pkt_size = pkt.Size;

            pkt.Initialize();

            m_ReceiveBuffer.ReadBytes(pkt.Buf, pkt_size);

            pkt.SetSize();

            Debug.Assert(pkt.Size == pkt_size);
            Debug.Assert(pkt.Valid());

            ret = PacketProcessManager.Instance.ProcessPacket(m_Object, pkt);

            if ((int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS != ret)
            {
                ret = (int)SESSION_ERROR.E_PACKET_PROC;
            }

            PacketManager.Instance.ReleasePacket(pkt);
            return ret;
        }


        public void SendPacket(Packet pkt)
        {
            m_SendBuffer.WriteBytes(pkt.Buf, pkt.Size);
        }


        protected void ProcessError(SocketAsyncEventArgs args)
        {
            if (null == args.UserToken)
            {
                return;
            }

            Session sess = args.UserToken as Session;
            IPEndPoint localEp = sess.m_Socket.LocalEndPoint as IPEndPoint;

            LogManager.Error(string.Format("Socket error {0} on endpoint {1} during {2}.",
                             (Int32)args.SocketError, localEp, args.LastOperation));

            CloseSession(args);
        }

        protected void CloseSession(SocketAsyncEventArgs args)
        {
            Session sess = args.UserToken as Session;

            if (null != sess)
            {
                if (sess.m_Object is User)
                {
                    User user = sess.m_Object as User;

                    if (null != user)
                    {
                        UserManager.Instance.RemoveUser(user);
                    }
                }
                else
                {

                }

                sess.CloseSocket();
            }
        }
    }
}
