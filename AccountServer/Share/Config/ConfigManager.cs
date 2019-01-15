using System.IO;

using Share.Json;

namespace Share.Config
{
    public class AccountServerConfig
    {
        public string AccountServerIPAddr;
        public int TcpGatewayServerListenPort;
        public long IDGeneratorWorkID;
        public long IDGeneratorDataCenterID;
    }

    public class ConfigManager : Singleton<ConfigManager>
    {
        private const string CONFIG_JSON_FILE = "AccountServerConfig.json";

        private AccountServerConfig m_Config;


        public string LOCAL_IP_ADDRESS { get { return m_Config.AccountServerIPAddr; } }
        public int TCP_GATEWAY_SERVER_LISTEN_PORT { get { return m_Config.TcpGatewayServerListenPort; } }
        public long ID_GENERATOR_WORKER_ID { get { return m_Config.IDGeneratorWorkID; } }
        public long ID_GENERATOR_DATACENTER_ID { get { return m_Config.IDGeneratorDataCenterID; } }


        public ConfigManager()
        {
            m_Config = null;
        }

        public void Initialize()
        {
            string json_file = Path.Combine(Folder.GetCurrentDir(), CONFIG_JSON_FILE);

            if (!File.Exists(json_file))
            {
                LogManager.Error("Account server config file " + json_file.ToString() + " not exist!");
                return;
            }

            FileStream file_stream = new FileStream(json_file, FileMode.Open);
            StreamReader stream_reader = new StreamReader(file_stream);
            stream_reader.BaseStream.Seek(0, SeekOrigin.Begin);

            string tmp_content = stream_reader.ReadToEnd();
            m_Config = JsonHelper.DeserializeJsonToObject<AccountServerConfig>(tmp_content);

            stream_reader.Close();
            file_stream.Close();
        }
    }
}
