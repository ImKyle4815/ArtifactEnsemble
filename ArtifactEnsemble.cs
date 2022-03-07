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
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    //Declare submodule dependencies
    [BepInDependency("com.bepis.r2api")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(NetworkingAPI), nameof(LanguageAPI), nameof(CommandHelper))]

    public class ArtifactEnsemble : BaseUnityPlugin
	{
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "ImKyle4815";
        public const string PluginName = "ArtifactEnsemble";
        public const string PluginVersion = "2.0.0";

        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        public void Awake()
        {
            //Init the logger for the mod
            ArtifactEnsemble.Logger = base.Logger;

            //Load the config file
            //ArtifactEnsembleConfig.Init(base.Config);

            //Tells R2API to scan the mod for console commands
            //CommandHelper.AddToConsoleWhenReady();

            //Initialize artifacts
            new BazaarArtifact();
            new FortuneArtifact();
            new GreedArtifact();
            //new HasteArtifact();
            new MountainArtifact();
            new ReanimationArtifact();
            new TradeArtifact();

            //Disables the space bazaar's kickout feature
            On.EntityStates.NewtMonster.KickFromShop.OnEnter += (orig, self) => { };
        }
    }
}
