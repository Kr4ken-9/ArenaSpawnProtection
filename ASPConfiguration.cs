using Rocket.API;
using UnityEngine;

namespace ArenaSpawnProtection
{
    public class ASPConfiguration : IRocketPluginConfiguration, IDefaultable
    {
        public void LoadDefaults()
        {
            PlayerProtectionTime = 10f;
            ProtectionStartMessage = "Players protected!";
            ProtectionEndMessage = "Player protection ended!";
            MessageColor = Color.cyan;
        }

        public float PlayerProtectionTime;
        public string ProtectionStartMessage;
        public string ProtectionEndMessage;
        public Color MessageColor;
    }
}
