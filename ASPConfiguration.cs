using System;
using Rocket.API;
using UnityEngine;

namespace ArenaSpawnProtection
{
    public class ASPConfiguration : IRocketPluginConfiguration, IDefaultable
    {
        public void LoadDefaults()
        {
            this.PlayerHideTime = 10f;
            this.ProtectionStartMessage = "Players protected!";
            this.ProtectionEndMessage = "Player protection ended!";
            this.MessageColor = Color.cyan;
        }

        public float PlayerHideTime;
        public string ProtectionStartMessage;
        public string ProtectionEndMessage;
        public Color MessageColor;
    }
}
