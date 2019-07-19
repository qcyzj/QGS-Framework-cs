using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using Share.Logs;
using Share.Config;

namespace Share.Net.WebSocket
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
            if (m_Stopping)
            {
                return;
            }


            NetworkStream stream = client.GetStream();

            while (true)
            {

            }
        }
    }
}
