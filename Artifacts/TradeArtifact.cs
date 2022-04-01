using RoR2;

namespace ArtifactEnsemble.Artifacts
{
    class TradeArtifact : ArtifactTemplate
    {
        public TradeArtifact() {
            Init("Trade", "Spawns the blue portal every stage.", Properties.Resources.axolotl_on, Properties.Resources.axolotl_off);
            On.RoR2.TeleporterInteraction.AttemptToSpawnAllEligiblePortals += ForceBluePortal;
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
