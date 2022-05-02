using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace ArtifactEnsemble.Artifacts
{
    public class NoBossNoWaitArtifact : ArtifactTemplate
    {
        public NoBossNoWaitArtifact()
        {
            Init("Passage", "Charges the teleporter upon killing the boss. Still progresses time appropriately.", Properties.Resources.nobossnowait_on, Properties.Resources.nobossnowait_off);
            On.RoR2.TeleporterInteraction.UpdateMonstersClear += skipTeleporterCharging;
        }
        private void skipTeleporterCharging(On.RoR2.TeleporterInteraction.orig_UpdateMonstersClear orig, RoR2.TeleporterInteraction self)
        {
            orig(self);
            if (this.Enabled())
            {
                if (self.monstersCleared && self.holdoutZoneController && self.activationState == TeleporterInteraction.ActivationState.Charging && self.chargeFraction > 0.02f)
                {
                    int displayChargePercent = TeleporterInteraction.instance.holdoutZoneController.displayChargePercent;
                    float runStopwatch = Run.instance.GetRunStopwatch();
                    int num = Math.Min(Util.GetItemCountForTeam(self.holdoutZoneController.chargingTeam, RoR2Content.Items.FocusConvergence.itemIndex, true, true), 3);
                    float num2 = (100f - (float)displayChargePercent) / 100f * (TeleporterInteraction.instance.holdoutZoneController.baseChargeDuration / (1f + 0.3f * (float)num));
                    num2 = (float)Math.Round((double)num2, 2);
                    float runStopwatch2 = runStopwatch + (float)Math.Round((double)num2, 2);
                    Run.instance.SetRunStopwatch(runStopwatch2);
                    TeleporterInteraction.instance.holdoutZoneController.FullyChargeHoldoutZone();
                }
            }
        }
    }
}
