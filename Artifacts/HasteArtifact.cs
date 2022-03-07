using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactEnsemble.Artifacts
{
    public class HasteArtifact : ArtifactTemplate
    {
        public HasteArtifact()
        {
            Init("Haste", "Makes scrapping/printing faster.", Properties.Resources.haste_on, Properties.Resources.haste_off);
            On.EntityStates.Duplicator.Duplicating.DropDroplet += fasterDropDroplet;
            On.EntityStates.Duplicator.Duplicating.BeginCooking += fasterBeginCooking;
        }

        private void fasterDropDroplet(On.EntityStates.Duplicator.Duplicating.orig_DropDroplet orig, EntityStates.Duplicator.Duplicating self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                self.outer.GetComponent<PurchaseInteraction>().Networkavailable = true;
            }
        }
        private void fasterBeginCooking(On.EntityStates.Duplicator.Duplicating.orig_BeginCooking orig, EntityStates.Duplicator.Duplicating self)
        {
            if (!NetworkServer.active)
            {
                orig(self);
            }
        }
    }
    /*
    public static class HasteArtifactManager
    {
        private static ArtifactDef myArtifact
        {
            get { return HasteArtifact.Instance.ArtifactDef; }
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
            EntityStates.Duplicator.Duplicating.initialDelayDuration = 0f;
            EntityStates.Duplicator.Duplicating.timeBetweenStartAndDropDroplet = 0f;
            EntityStates.Scrapper.WaitToBeginScrapping.duration = 0f;
            EntityStates.Scrapper.Scrapping.duration = 0f;
            EntityStates.Scrapper.ScrappingToIdle.duration = 0f;
            On.EntityStates.Duplicator.Duplicating.DropDroplet += fasterDropDroplet;
            On.EntityStates.Duplicator.Duplicating.BeginCooking += fasterBeginCooking;
            ArtifactEnsemble.Logger.LogInfo("Artifact of Haste is now enabled.");
            
        }

        private static void OnArtifactDisabled(RunArtifactManager man, ArtifactDef artifactDef)
        {
            if (artifactDef != myArtifact)
            {
                return;
            }

            // undo things
            EntityStates.Duplicator.Duplicating.initialDelayDuration = 1.5f;
            EntityStates.Duplicator.Duplicating.timeBetweenStartAndDropDroplet = 1.333f;
            EntityStates.Scrapper.WaitToBeginScrapping.duration = 1.5f;
            EntityStates.Scrapper.Scrapping.duration = 1.5f;
            EntityStates.Scrapper.ScrappingToIdle.duration = 1f;
            On.EntityStates.Duplicator.Duplicating.DropDroplet -= fasterDropDroplet;
            On.EntityStates.Duplicator.Duplicating.BeginCooking -= fasterBeginCooking;
            ArtifactEnsemble.Logger.LogInfo("Artifact of Haste is now disabled.");
        }
    }*/
}