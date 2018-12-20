using Share;
using Share.Config;
using Share.Net.Packets;

namespace GatewayServer.Gateway.AccountServer
{
    public sealed class AccountServerManager : Singleton<AccountServerManager>
    {
        private AccountServer m_AccountServer;


        public AccountServerManager()
        {
            m_AccountServer = new AccountServer();
        }


        public void Start()
        {
            RegisterAccountServerProc();

            m_AccountServer.Initialize();

            m_AccountServer.ConnectToAccountServerAsync(ConfigManager.LOCAL_IP_ADDRESS,
                                                        ConfigManager.TCP_ACCOUNT_SERVER_CONNECT_PORT);

            ValidInitializeOnce();
        }

        public void Stop()
        {
            ValidReleaseOnce();

            m_AccountServer.Release();
        }


        private void RegisterAccountServerProc()
        {
            RegisterProcImpl(Protocol.GW_ACT_AUTH, AccountServer.PacketProcessAuth);
        }

        private void RegisterProcImpl(int protocol, PacketProcessManager.PacketProcessFunc func)
        {
            PacketProcessManager.Instance.RegisterProc(protocol, func);
        }
    }
}
