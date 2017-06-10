using System;
using Rocket.Unturned;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using System.Collections;
using System.Threading;

namespace ArenaSpawnProtection
{
    public class SpawnProtection : RocketPlugin<ASPConfiguration>
    {
        protected override void Load()
        {
            Instance = this;
            Vanish = Instance.Configuration.Instance.EnableVanish;
            God = Instance.Configuration.Instance.EnableGod;
            Rocket.Core.Logging.Logger.Log("\n\n\rArenaSpawnProtection made by ic3w0lf", ConsoleColor.Green);
            Rocket.Core.Logging.Logger.Log(string.Format("\r\rPlayer Protection Time: {0} seconds\n\n", Instance.Configuration.Instance.PlayerProtectionTime), ConsoleColor.Green);
            Rocket.Core.Logging.Logger.Log(string.Format("Protect with godmode {0}, protect with vanish {1}", God ? "on" : "off", Vanish ? "on" : "off"), ConsoleColor.Green);
            U.Events.OnPlayerConnected += ProtectPlayerConnected;
            LevelManager.onArenaMessageUpdated += ArenaMessageUpdated;
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= ProtectPlayerConnected;
            LevelManager.onArenaMessageUpdated -= ArenaMessageUpdated;
        }

        private void ProtectPlayerConnected(UnturnedPlayer player)
        {
            if (LevelManager.arenaState == EArenaState.WARMUP)
                player.Features.VanishMode = true;
        }

        public void ArenaMessageUpdated(EArenaMessage Message)
        {
            switch (Message)
            {
                case EArenaMessage.WARMUP:
                    break;
            }
        }

        public void ProtectPlayers()
        {
            new Thread(() =>
            {
                for (int i = 0; i < Provider.clients.Count; i++)
                {
                    UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(Provider.clients[i].playerID.steamID);
                    uPlayer.Features.VanishMode = true;
                    uPlayer.Features.GodMode = true;
                    Rocket.Core.Logging.Logger.Log("Protected " + uPlayer.DisplayName, ConsoleColor.Cyan);
                }
                ChatManager.say(Instance.Configuration.Instance.ProtectionStartMessage, Instance.Configuration.Instance.MessageColor);
                Thread.Sleep(5000);
                for (int i = 0; i < Provider.clients.Count; i++)
                {
                    UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(Provider.clients[i].playerID.steamID);
                    uPlayer.Features.VanishMode = false;
                    uPlayer.Features.GodMode = false;
                    Rocket.Core.Logging.Logger.Log("Ended protection for " + uPlayer.DisplayName, ConsoleColor.Cyan);
                }
                ChatManager.say(Instance.Configuration.Instance.ProtectionEndMessage, Instance.Configuration.Instance.MessageColor);
            }).Start();
        }

        private bool God;

        private bool Vanish;

        public static SpawnProtection Instance;
    }
}