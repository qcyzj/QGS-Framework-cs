﻿using GameServer.GameServer.WinService;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WinServiceManager.Instance.Init();

            WinServiceManager.Instance.Loop();

            WinServiceManager.Instance.Exit();
        }
    }
}
