using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Tasks;

using Share.Logs;
using Share.Config;
using Share.Net.Packets;

namespace Share.Net.WebSockets
{
    public class WebSocketServer
    {
        private TcpListener m_Listener;
        private int m_Port;
        private bool m_Stopping;


        public WebSocketServer(int port)
        {
            m_Port = port;
            m_Stopping = false;
        }

        public string Encodinng { get; private set; }

        public async Task StartAsync()
        {
            try
            {
                IPAddress address = IPAddress.Parse(ConfigManager.Instance.LOCAL_IP_ADDRESS);
                IPEndPoint end_point = new IPEndPoint(address, m_Port);

                m_Listener = new TcpListener(end_point);
                m_Listener.Start();

                while (!m_Stopping)
                {
                    TcpClient client = await m_Listener.AcceptTcpClientAsync();
                    ProcessTcpClient(client);
                }
            }
            catch (SocketException socket_ex)
            {
                LogManager.Error($"Web sokcet error listen on port { m_Port }.", socket_ex);
            }
            catch (Exception ex)
            {
                LogManager.Error($"Web socket error: ", ex);
            }
        }

        public void Stop()
        {
            m_Stopping = true;

            if (null != m_Listener)
            {
                if (null != m_Listener.Server)
                {
                    m_Listener.Server.Close();
                }

                m_Listener.Stop();
            }
        }


        private void ProcessTcpClient(TcpClient client)
        {
            Task.Run(() => ProcessTcpClientAsync(client));
        }

        private async Task ProcessTcpClientAsync(TcpClient client)
        {
            CancellationTokenSource source = new CancellationTokenSource();

            if (m_Stopping)
            {
                return;
            }

            try
            {
                NetworkStream stream = client.GetStream();
                WebSocketContextImpl context = ReadHttpHeaderFromStream(stream, source);

                if (null != context && context.IsWebSocketRequest)
                {
                    WebSocket web_socket = context.WebSocket;
                    bool ret = ProcessWebSocketHandshake(stream, context);

                    if (ret)
                    {
                        ResponseToWebSocketRequest(web_socket, source.Token);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("ProcessTcpClientAsync error: ", ex);
            }
            finally
            {
                client.Client.Close();
                client.Close();
            }
        }

        private WebSocketContextImpl ReadHttpHeaderFromStream(NetworkStream stream,
                                                              CancellationTokenSource source)
        {
            string http_header = WebSocketHttpHelper.ReadHttpHeader(stream);

            if (string.Empty != http_header)
            {
                return new WebSocketContextImpl(http_header, source);
            }
            else
            {
                return null;
            }
        }

        private bool ProcessWebSocketHandshake(NetworkStream stream, WebSocketContextImpl context)
        {
            if (string.Empty != context.SecWebSocketKey)
            {
                try
                {
                    string secWebSocketAccept = WebSocketHttpHelper.CalSecWebSocketAccept(context.SecWebSocketKey);
                    string secWebSocketProtocols = (string.Empty != context.SubProtocol) ?
                                                        $"Sec-WebSocket-Protocol: {context.SubProtocol}\r\n" : string.Empty;

                    string responsee = "HTTP/1.1 101 Switching Protocols\r\n" +
                                       "Upgrade: websocket\r\nn" +
                                       "Connection: Upgrade\r\n" +
                                       $"Sec-WebSocket-Accept: {secWebSocketAccept}\r\n" +
                                       secWebSocketProtocols +
                                       "\r\n";

                    byte[] bytes = Encoding.UTF8.GetBytes(responsee);
                    stream.Write(bytes, 0, bytes.Length);
                    return true;
                }
                catch (Exception ex)
                {
                    LogManager.Error("Process web socket handshake error:", ex);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private async Task ResponseToWebSocketRequest(WebSocket web_socket, CancellationToken token)
        {
            ArraySegment<byte> server_buf = WebSocket.CreateServerBuffer(8192);

            while (true)
            {
                WebSocketReceiveResult result = await web_socket.ReceiveAsync(server_buf, token);

                if (WebSocketMessageType.Close == result.MessageType)
                {
                    break;
                }

                if (result.Count > 8192)
                {
                    await web_socket.CloseAsync(WebSocketCloseStatus.MessageTooBig,
                                                "",
                                                token);
                    break;
                }

                //ArraySegment<byte> toSend = new ArraySegment<byte>(buffer.Array, buffer.Offset, result.Count);
                //await web_socket.SendAsync(toSend, WebSocketMessageType.Binary, true, token);

                Packet pkt = PacketManager.Instance.AllocatePacket();
                pkt.Initialize();

                server_buf.CopyTo(pkt.Buf, 0);
                pkt.SetSize();

                Debug.Assert(pkt.Valid());

                int ret = PacketProcessManager.Instance.ProcessPacket(, pkt);

                if ((int)PacketProcessManager.PACKET_PROC_ERROR.SUCCESS != ret)
                {

                }

                PacketManager.Instance.ReleasePacket(pkt);
            }
        }
    }
}
