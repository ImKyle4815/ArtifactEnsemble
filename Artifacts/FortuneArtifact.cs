using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactEnsemble
{

    public class FortuneArtifact : NewArtifact<FortuneArtifact>
    {

        public override string Name => "Artifact of Fortune";
        public override string NameToken => "FORTUNE";
        public override string Description => "Whenever a blue portal spawns, players' lunar coin counts will be raised to 2-billion";
        public override Sprite IconSelectedSprite => CreateSprite(Properties.Resources.money_on, Color.magenta);
        public override Sprite IconDeselectedSprite => CreateSprite(Properties.Resources.money_off, Color.gray);

        protected override void InitManager()
        {
            FortuneArtifactManager.Init();
        }
    }

    public static class FortuneArtifactManager
    {
        private static ArtifactDef myArtifact
        {
            get { return FortuneArtifact.Instance.ArtifactDef; }
        }

        public static void Init()
        {
            RunArtifactManager.onArtifactEnabledGlobal += OnArtifactEnabled;
            RunArtifactManager.onArtifactDisabledGlobal += OnArtifactDisabled;
        }

        private static void OnArtifactEnabled(RunArtifactManager man, ArtifactDef artifactDef)
        {
            if (!NetworkServer.active || artifactDef != myArtifact)
            {
                return;
            }

            // do things
            ArtifactEnsemble.Logger.LogInfo("Artifact of Fortune is now enabled.");
            On.RoR2.TeleporterInteraction.AttemptToSpawnShopPortal += AddLunarCoins;
        }

        private static void OnArtifactDisabled(RunArtifactManager man, ArtifactDef artifactDef)
        {
            if (artifactDef != myArtifact)
            {
                return;
            }

            // undo things
            ArtifactEnsemble.Logger.LogInfo("Artifact of Fortune is now disabled.");
            On.RoR2.TeleporterInteraction.AttemptToSpawnShopPortal -= AddLunarCoins;
        }

        private static void AddLunarCoins(On.RoR2.TeleporterInteraction.orig_AttemptToSpawnShopPortal orig, TeleporterInteraction self)
        {
            orig(self);
            ArtifactEnsemble.Logger.LogInfo("Adding lunar coins...");
            if (ArtifactEnsemble.fortuneArtifact.ArtifactEnabled)
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
