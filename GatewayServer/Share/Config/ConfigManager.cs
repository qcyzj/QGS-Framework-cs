using System.IO;

using Newtonsoft.Json;

namespace Share.Config
{
    public class GatewayServerConfig
    {
        public string GatewayServerIPAddr;
        public int TcpUserListenPort;
        public int UdpUserConnectPort;
        public int TcpGameServerListenPort;
        public int TcpAccountServerConnectPort;
        public uint GatewayServerID;
    }

    public class ConfigManager : Singleton<ConfigManager>
    {
        private const string CONFIG_JSON_FILE = "GatewayServerConfig.json";

        private GatewayServerConfig m_Config;


        public string LOCAL_IP_ADDRESS { get { return m_Config.GatewayServerIPAddr; } }
        public int TCP_USER_LISTEN_PORT { get { return m_Config.TcpUserListenPort; }}
        public int UDP_USER_CONNECT_PORT { get { return m_Config.UdpUserConnectPort; } }
        public int TCP_GAME_SERVER_LISTEN_PORT { get { return m_Config.TcpGameServerListenPort; } }
        public int TCP_ACCOUNT_SERVER_CONNECT_PORT { get { return m_Config.TcpAccountServerConnectPort; } }
        public uint GATEWAY_SERVER_ID { get { return m_Config.GatewayServerID; } }


        public ConfigManager()
        {
            m_Config = null;
        }

        public void Init()
        {
            string json_file = Path.Combine(Folder.GetCurrentDir(), CONFIG_JSON_FILE);

            if (!File.Exists(json_file))
            {
                LogManager.Error("Gateway server config file " + json_file.ToString() + " not exist!");
                return;
            }

            FileStream file_stream = new FileStream(json_file, FileMode.Open);
            StreamReader stream_reader = new StreamReader(file_stream);
            stream_reader.BaseStream.Seek(0, SeekOrigin.Begin);

            string tmp_content = stream_reader.ReadToEnd();
            m_Config  = JsonConvert.DeserializeObject<GatewayServerConfig>(tmp_content);

            stream_reader.Close();
            file_stream.Close();
        }
    }
}
