using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using R2API.Networking;

namespace ArtifactEnsemble
{
    //Lists Plugin MetaData
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    //Declare submodule dependencies
    [BepInDependency("com.bepis.r2api")]
    [R2APISubmoduleDependency(new string[] { nameof(NetworkingAPI), nameof(ArtifactAPI), nameof(LanguageAPI), nameof(CommandHelper) })]
    public class ArtifactEnsemble : BaseUnityPlugin
	{
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "ImKyle4815";
        public const string PluginName = "ArtifactEnsemble";
        public const string PluginVersion = "1.2.0";

        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        //public static TestArtifact testArtifact;
        public static ReanimationArtifact reanimationArtifact;
        public static TradeArtifact tradeArtifact;
        public static FortuneArtifact fortuneArtifact;
        public static BazaarArtifact bazaarArtifact;
        public static HasteArtifact hasteArtifact;
        public static GreedArtifact greedArtifact;
        public static MountainArtifact mountainArtifact;
        //public static RandomArtifact randomArtifact;

        public void Awake()
        {
            //Init the logger for the mod
            ArtifactEnsemble.Logger = base.Logger;

            //Load the config file (nothing, for now)
            ArtifactEnsembleConfig.Init(base.Config);

            //Tells R2API to scan the mod for console commands
            CommandHelper.AddToConsoleWhenReady();

            //Initialize artifacts
            ArtifactEnsemble.reanimationArtifact = new ReanimationArtifact();
            ArtifactEnsemble.tradeArtifact = new TradeArtifact();
            ArtifactEnsemble.fortuneArtifact = new FortuneArtifact();
            ArtifactEnsemble.bazaarArtifact = new BazaarArtifact();
            ArtifactEnsemble.hasteArtifact = new HasteArtifact();
            ArtifactEnsemble.greedArtifact = new GreedArtifact();
            ArtifactEnsemble.mountainArtifact = new MountainArtifact();
            //ArtifactEnsemble.randomArtifact = new RandomArtifact();

            //Disables the space bazaar's kickout feature
            On.EntityStates.NewtMonster.KickFromShop.OnEnter += (orig, self) => { };
        }
    }
}
