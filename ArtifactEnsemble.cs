using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using R2API.Networking;
using System.Linq;
using System.Reflection;
using UnityEngine;
using RoR2;
using ArtifactEnsemble.Artifacts;

namespace ArtifactEnsemble
{
    //Lists Plugin MetaData
    [BepInPlugin("ImKyle4815.ArtifactEnsemble", "ArtifactEnsemble", "2.0.1")]
    //Declare submodule dependencies
    [BepInDependency("com.bepis.r2api")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(NetworkingAPI), nameof(LanguageAPI), nameof(CommandHelper))]

    public class ArtifactEnsemble : BaseUnityPlugin
	{
        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        public void Awake()
        {
            //Init the logger for the mod
            ArtifactEnsemble.Logger = base.Logger;

            //Load the config
            ArtifactEnsembleConfig.Init(base.Config);

            //Tells R2API to scan the mod for console commands
            //CommandHelper.AddToConsoleWhenReady();

            //Initialize artifacts
            if (ArtifactEnsembleConfig.UseBazaarArtifact.Value) new BazaarArtifact();
            if (ArtifactEnsembleConfig.UseFortuneArtifact.Value) new FortuneArtifact();
            if (ArtifactEnsembleConfig.UseGreedArtifact.Value) new GreedArtifact();
            //if (ArtifactEnsembleConfig.UseBazaarArtifact.Value) new HasteArtifact();
            if (ArtifactEnsembleConfig.UseMountainArtifact.Value) new MountainArtifact();
            if (ArtifactEnsembleConfig.UseReanimationArtifact.Value) new ReanimationArtifact();
            if (ArtifactEnsembleConfig.UseTradeArtifact.Value) new TradeArtifact();

            //Disables the space bazaar's kickout feature
            On.EntityStates.NewtMonster.KickFromShop.OnEnter += (orig, self) => { };
        }

        private void ConfigInit()
        {

        }
    }
}
