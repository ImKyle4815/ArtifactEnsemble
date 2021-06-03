using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactEnsemble
{

    public class TradeArtifact : NewArtifact<TradeArtifact>
    {

        public override string Name => "Artifact of Trade";
        public override string NameToken => "TRADE";
        public override string Description => "Starts the run in the Space Bazaar, and spawns the blue portal every stage";
        public override Sprite IconSelectedSprite => CreateSprite(Properties.Resources.axolotl_on, Color.magenta);
        public override Sprite IconDeselectedSprite => CreateSprite(Properties.Resources.axolotl_off, Color.gray);

        protected override void InitManager()
        {
            TradeArtifactManager.Init();
        }
    }

    public static class TradeArtifactManager
    {
        private static ArtifactDef myArtifact
        {
            get { return TradeArtifact.Instance.ArtifactDef; }
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
            On.RoR2.Run.Start += StartInBazaar;
            On.RoR2.TeleporterInteraction.AttemptToSpawnAllEligiblePortals += ForceBluePortal;
            ArtifactEnsemble.Logger.LogInfo("Artifact of Trade is now enabled.");
        }

        private static void OnArtifactDisabled(RunArtifactManager man, ArtifactDef artifactDef)
        {
            if (artifactDef != myArtifact)
            {
                return;
            }

            // undo things
            On.RoR2.Run.Start -= StartInBazaar;
            On.RoR2.TeleporterInteraction.AttemptToSpawnAllEligiblePortals -= ForceBluePortal;
            ArtifactEnsemble.Logger.LogInfo("Artifact of Trade is now disabled.");
        }

        private static void StartInBazaar(On.RoR2.Run.orig_Start orig, RoR2.Run self)
        {
            orig(self);
            ArtifactEnsemble.Logger.LogInfo("Artifact of Trade is changing the first scene to the bazaar");
            if (ArtifactEnsemble.tradeArtifact.ArtifactEnabled)
            {
                ArtifactEnsemble.Logger.LogInfo("Starting in the Bazaar.");
                SceneField sceneField = new SceneField("bazaar");
                NetworkManager.singleton.ServerChangeScene(sceneField);
            }
        }

        private static void ForceBluePortal(On.RoR2.TeleporterInteraction.orig_AttemptToSpawnAllEligiblePortals orig, TeleporterInteraction self)
        {
            ArtifactEnsemble.Logger.LogInfo("Forcing the blue portal to spawn.");
            self.shouldAttemptToSpawnShopPortal = true;
            orig(self);
        }
    }
}
