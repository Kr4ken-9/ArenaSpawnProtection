using System;
using Rocket.Unturned;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using System.Collections;

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
            StartCoroutine(CheckArenaState());
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= ProtectPlayerConnected;
            StopAllCoroutines();
        }

        private void ProtectPlayerConnected(UnturnedPlayer player)
        {
            if (LevelManager.arenaState == EArenaState.WARMUP)
            {
                if (!player.Features.VanishMode)
                    player.Features.VanishMode = true;
            }
        }

        public IEnumerator CheckArenaState()
        {
            while (true)
            {
                if (Provider.isServer)
                {
                    switch (LevelManager.arenaState)
                    {
                        case EArenaState.WARMUP:
                            {
                                if (!ProtectedB)
                                {
                                    ProtectedB = true;
                                    if (PTCoroutine != null)
                                        try { StopCoroutine(PTCoroutine); } catch { }
                                    Rocket.Core.Logging.Logger.Log("Protecting players!", ConsoleColor.Cyan);
                                    PTCoroutine = StartCoroutine(ProtectPlayers());
                                }
                                break;
                            }
                        case EArenaState.INTERMISSION:
                            {
                                ProtectedB = false;
                                break;
                            }
                    }
                }
                yield return new WaitForSeconds(1f);
            }
        }

        public IEnumerator ProtectPlayers()
        {
            for (int i = 0; i < Provider.clients.Count; i++)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(Provider.clients[i].playerID.steamID);
                if (Vanish && !uPlayer.Features.VanishMode)
                    uPlayer.Features.VanishMode = true;
                if (God && !uPlayer.Features.GodMode)
                    uPlayer.Features.GodMode = true;
                Rocket.Core.Logging.Logger.Log("Protected " + uPlayer.DisplayName, ConsoleColor.Cyan);
            }
            ChatManager.say(Instance.Configuration.Instance.ProtectionStartMessage, Instance.Configuration.Instance.MessageColor);
            yield return new WaitForSeconds(Instance.Configuration.Instance.PlayerProtectionTime + 5f);
            for (int i = 0; i < Provider.clients.Count; i++)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromCSteamID(Provider.clients[i].playerID.steamID);
                if (Vanish && uPlayer.Features.VanishMode)
                    uPlayer.Features.VanishMode = false;
                if (God && uPlayer.Features.GodMode)
                    uPlayer.Features.GodMode = false;
                Rocket.Core.Logging.Logger.Log("Ended protection for " + uPlayer.DisplayName, ConsoleColor.Cyan);
            }
            ChatManager.say(Instance.Configuration.Instance.ProtectionEndMessage, Instance.Configuration.Instance.MessageColor);

            yield return new WaitForEndOfFrame();
        }

        private bool God;

        private bool Vanish;

        private Coroutine PTCoroutine;

        public static SpawnProtection Instance;

        private bool ProtectedB = false;
    }
}