using RoR2;

namespace ArtifactEnsemble.Artifacts
{
    class TradeArtifact : ArtifactTemplate
    {
        public TradeArtifact() {
            Init("Trade", "Starts in the Bazaar, and forces the blue portal to spawn every stage.", Properties.Resources.axolotl_on, Properties.Resources.axolotl_off);
            On.RoR2.Run.Start += StartInBazaar;
            On.RoR2.TeleporterInteraction.AttemptToSpawnAllEligiblePortals += ForceBluePortal;
        }
        private void StartInBazaar(On.RoR2.Run.orig_Start orig, RoR2.Run self)
        {
            orig(self);
            if (this.Enabled())
            {
                SceneField sceneField = new SceneField("bazaar");
                UnityEngine.Networking.NetworkManager.singleton.ServerChangeScene(sceneField);
            }
        }
        private void ForceBluePortal(On.RoR2.TeleporterInteraction.orig_AttemptToSpawnAllEligiblePortals orig, TeleporterInteraction self)
        {
            if (this.Enabled())
            {
                self.shouldAttemptToSpawnShopPortal = true;
            }
            orig(self);
        }
    }
}
