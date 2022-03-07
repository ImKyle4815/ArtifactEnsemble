using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactEnsemble.Artifacts
{
    public class FortuneArtifact : ArtifactTemplate
    {
        public FortuneArtifact()
        {
            Init("Fortune", "Whenever a blue portal spawns, each player's lunar coin counts will be raised to 2-billion.", Properties.Resources.money_on, Properties.Resources.money_off);
            On.RoR2.TeleporterInteraction.AttemptToSpawnShopPortal += AddLunarCoins;
        }
        private void AddLunarCoins(On.RoR2.TeleporterInteraction.orig_AttemptToSpawnShopPortal orig, TeleporterInteraction self)
        {
            orig(self);
            ArtifactEnsemble.Logger.LogInfo("Adding lunar coins...");
            if (this.Enabled())
            {
                foreach (NetworkUser networkUser in NetworkUser.readOnlyInstancesList)
                {
                    int num = 2000000000 - (int)networkUser.netLunarCoins;
                    if (num > 0)
                    {
                        networkUser.AwardLunarCoins((uint)num);
                    }
                    else if (num < 0)
                    {
                        num *= -1;
                        networkUser.DeductLunarCoins((uint)num);
                    }
                }
            }
        }
    }
}
