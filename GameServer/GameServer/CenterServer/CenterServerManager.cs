using Share;
using Share.Config;
using Share.Net.Packets;

namespace GameServer.GameServer.CenterServer
{
    public sealed class CenterServerManager : Singleton<CenterServerManager>
    {
        private CenterServer m_CenterServer;

        
        public CenterServerManager()
        {
            m_CenterServer = new CenterServer();
        }


        public void Start()
        {
            RegisterCenterServerProc();

            m_CenterServer.Initialize();

            m_CenterServer.ConnectToCenterServerAsync(ConfigManager.LOCAL_IP_ADDRESS,
                                                      ConfigManager.TCP_CENTER_SERVER_CONNECT_PORT);

            ValidInitializeOnce();
        }

        public void Stop()
        {
            ValidReleaseOnce();

            m_CenterServer.Release();
        }


        private void RegisterCenterServerProc()
        {
            RegisterProcImpl(Protocol.GS_CNT_AUTH, CenterServer.PacketProcessAuth);
        }

        private void RegisterProcImpl(int protocol, PacketProcessManager.PacketProcessFunc func)
        {
            PacketProcessManager.Instance.RegisterProc(protocol, func);
        }
    }
}
