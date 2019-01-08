using System.IO;

using Newtonsoft.Json;

namespace Share.Config
{
    public class CenterServerConfig
    {
        public string CenterServerIPAddr;
        public int TcpCenterServerListenPort;
    }

    public class ConfigManager : Singleton<ConfigManager>
    {
        private const string CONFIG_JSON_FILE = "CenterServerConfig.json";

        private CenterServerConfig m_Config;


        public string LOCAL_IP_ADDRESS { get { return m_Config.CenterServerIPAddr; } }
        public int TCP_CENTER_SERVER_LISTEN_PORT { get { return m_Config.TcpCenterServerListenPort; } }


        public ConfigManager()
        {
            m_Config = null;
        }

        public void Initialize()
        {
            string json_file = Path.Combine(Folder.GetCurrentDir(), CONFIG_JSON_FILE);

            if (!File.Exists(json_file))
            {
                LogManager.Error("Center server config file " + json_file.ToString() + " not exist!");
                return;
            }

            FileStream file_stream = new FileStream(json_file, FileMode.Open);
            StreamReader stream_reader = new StreamReader(file_stream);
            stream_reader.BaseStream.Seek(0, SeekOrigin.Begin);

            string tmp_content = stream_reader.ReadToEnd();
            m_Config = JsonConvert.DeserializeObject<CenterServerConfig>(tmp_content);

            stream_reader.Close();
            file_stream.Close();
        }
    }
}
