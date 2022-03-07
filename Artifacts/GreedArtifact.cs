using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactEnsemble.Artifacts
{
    public class GreedArtifact : ArtifactTemplate
    {
        public GreedArtifact()
        {
            Init("Greed", "Removes the one-scavenger-bag-per-stage limit.", Properties.Resources.greed_on, Properties.Resources.greed_off);
            On.EntityStates.ScavMonster.Death.OnPreDestroyBodyServer += resetScavDeath;
        }
        private void resetScavDeath(On.EntityStates.ScavMonster.Death.orig_OnPreDestroyBodyServer orig, EntityStates.ScavMonster.Death self)
        {
            if (this.Enabled()) {
                self.shouldDropPack = true;
            }
            orig(self);
        }
    }
}
