using RoR2;
using UnityEngine;
using static ArtifactEnsemble.ArtifactEnsemble;

namespace ArtifactEnsemble.Artifacts
{

    public class BazaarArtifact : ArtifactTemplate
    {
        public BazaarArtifact()
        {
            Init("Bazaar", "Adds a cleansing pool, scrapper, and a void chest to the space bazaar.", Properties.Resources.bazaar_on, Properties.Resources.bazaar_off);
            On.RoR2.BazaarController.SetUpSeerStations += SpawnBazaarExtras;
        }

        private void SpawnBazaarExtras(On.RoR2.BazaarController.orig_SetUpSeerStations orig, BazaarController self)
        {
            orig(self);
            
            if (!this.Enabled()) return;
            
            ArtifactEnsemble.Logger.LogInfo("Spawning bazaar extras.");
            
            // Cleansing Pool
            if(ArtifactEnsembleConfig.SpawnBazaarCleansingPool.Value)
                TrySpawn("SpawnCards/InteractableSpawnCard/iscShrineCleanse", new Vector3(-65.7f, -23.5f, -18.9f), Vector3.zero);
            
            // Scrapper
            if (ArtifactEnsembleConfig.SpawnBazaarScrapper.Value)
                TrySpawn("SpawnCards/InteractableSpawnCard/iscScrapper", new Vector3(-82.1f, -23.7f, -5.2f),
                    new Vector3(0f, 72.6f, 0f));
            
            // Void Chest
            if (ArtifactEnsembleConfig.SpawnBazaarVoidChest.Value)
                TrySpawn("SpawnCards/InteractableSpawnCard/iscVoidChest",new Vector3(-69.7f, -23.5f, -12.9f),Vector3.zero);
        }
    }
}
