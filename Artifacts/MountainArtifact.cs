using RoR2;
using UnityEngine;
using UnityEngine.Networking;


namespace ArtifactEnsemble.Artifacts
{
    public class MountainArtifact : ArtifactTemplate
    {
        public MountainArtifact()
        {
            Init("Summit", "Automatically activates one mountain shrine every stage, plus an additional for each completed loop.", Properties.Resources.mountain_on, Properties.Resources.mountain_off);
            On.RoR2.TeleporterInteraction.OnInteractionBegin += activateMountainShrines;
        }

        private void activateMountainShrines(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, RoR2.TeleporterInteraction self, Interactor activator)
        {
            if (this.Enabled())
            {
                int numberOfMountainShrines = (Run.instance.stageClearCount / Run.stagesPerLoop) + 1;
                for (int i = 0; i < numberOfMountainShrines * ArtifactEnsembleConfig.MountainCount.Value; i++)
                {
                    self.AddShrineStack();
                }
            }
            orig(self, activator);
        }
    }
}
