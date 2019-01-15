using System.IO;

using Share.Json;

namespace Share.Config
{
    public class GameServerConfig
    {
        public string GameServerIPAddr;
        public int TcpGatewayServerConnectPort;
        public int TcpCenterServerConnectPort;
        public uint GameServerID;
    }

    public class ConfigManager : Singleton<ConfigManager>
    {
        private const string CONFIG_JSON_FILE = "GameServerConfig.json";

        private GameServerConfig m_Config;


        public string LOCAL_IP_ADDRESS { get { return m_Config.GameServerIPAddr; } }
        public int TCP_GATEWAY_SERVER_CONNECT_PORT { get { return m_Config.TcpGatewayServerConnectPort; } }
        public int TCP_CENTER_SERVER_CONNECT_PORT { get { return m_Config.TcpCenterServerConnectPort; } }
        public uint GAME_SERVER_ID { get { return m_Config.GameServerID; } }


        public ConfigManager()
        {
            m_Config = null;
        }

        public void Initialize()
        {
            string json_file = Path.Combine(Folder.GetCurrentDir(), CONFIG_JSON_FILE);

            if (!File.Exists(json_file))
            {
                LogManager.Error("Game server config file " + json_file.ToString() + " not exist!");
                return;
            }

            FileStream file_stream = new FileStream(json_file, FileMode.Open);
            StreamReader stream_reader = new StreamReader(file_stream);
            stream_reader.BaseStream.Seek(0, SeekOrigin.Begin);

            string tmp_content = stream_reader.ReadToEnd();
            m_Config = JsonHelper.DeserializeJsonToObject<GameServerConfig>(tmp_content);

            stream_reader.Close();
            file_stream.Close();
        }
    }
}
