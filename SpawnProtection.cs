using System;
using Rocket.Unturned;
using Rocket.Core.Plugins;
using Rocket.Core.Logging;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Rocket.Core.Extensions;
using System.Collections;
using System.Diagnostics;

namespace ArenaSpawnProtection
{
    public class SpawnProtection : RocketPlugin<ASPConfiguration>
    {
        protected override void Load()
        {
            Instance = this;
            Rocket.Core.Logging.Logger.Log("\n\n\rArenaSpawnProtection made by ic3w0lf", ConsoleColor.Green);
            Rocket.Core.Logging.Logger.Log(string.Format("\r\rPlayer Hide Time: {0} seconds\n\n", base.Configuration.Instance.PlayerHideTime), ConsoleColor.Green);
            U.Events.OnPlayerConnected += new UnturnedEvents.PlayerConnected(ProtectPlayerConnected);
            StartCoroutine(CheckArenaState());
        }
        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= new UnturnedEvents.PlayerConnected(ProtectPlayerConnected);
            StopAllCoroutines();
        }

        private void ProtectPlayerConnected(UnturnedPlayer player)
        {
            if (LevelManager.arenaState == EArenaState.WARMUP)
            {
                player.Features.VanishMode = true;
            }
        }

        [DebuggerHidden()]
        public IEnumerator CheckArenaState()
        {
            while (true)
            {
                if (Provider.isServer)
                {
                    switch(LevelManager.arenaState)
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
                yield return new WaitForSeconds(.05f);
            }
        }

        [DebuggerHidden()]
        public IEnumerator ProtectPlayers()
        {
            for (int i = 0; i < Provider.clients.Count; i++)
            {
                SteamPlayer sPlayer = Provider.clients[i];
                UnturnedPlayer uPlayer = (UnturnedPlayer)UnturnedPlayer.FromCSteamID(sPlayer.playerID.steamID);
                uPlayer.Features.VanishMode = true;
                Rocket.Core.Logging.Logger.Log("Protected " + uPlayer.DisplayName, ConsoleColor.Cyan);
            }
            ChatManager.say(base.Configuration.Instance.ProtectionStartMessage, base.Configuration.Instance.MessageColor);
            yield return new WaitForSeconds(base.Configuration.Instance.PlayerHideTime + 5f);
            for (int i = 0; i < Provider.clients.Count; i++)
            {
                SteamPlayer sPlayer = Provider.clients[i];
                UnturnedPlayer uPlayer = (UnturnedPlayer)UnturnedPlayer.FromCSteamID(sPlayer.playerID.steamID);
                uPlayer.Features.VanishMode = false;
                Rocket.Core.Logging.Logger.Log("Ended protection for " + uPlayer.DisplayName, ConsoleColor.Cyan);
            }
            ChatManager.say(base.Configuration.Instance.ProtectionEndMessage, base.Configuration.Instance.MessageColor);

            yield return new WaitForEndOfFrame();
        }

        private Coroutine PTCoroutine;
        public static SpawnProtection Instance;

        private bool ProtectedB = false;
    }
}