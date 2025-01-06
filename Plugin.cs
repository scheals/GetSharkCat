using BepInEx;
using BepInEx.Logging;
using Eremite;
using Eremite.Controller;
using HarmonyLib;

namespace GetSharkCat
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        private Harmony harmony;
        internal static ManualLogSource myLog;

        private void Awake()
        {
            Instance = this;
            harmony = Harmony.CreateAndPatchAll(typeof(Plugin));
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Configs.EffectGrantingEnabled = Configs.EffectGrantingEnabled; // I have no idea how to do this sensibly. Sorry!
            myLog = Instance.Logger;
        }

        [HarmonyPatch(typeof(MainController), nameof(MainController.OnServicesReady))]
        [HarmonyPostfix]
        private static void HookMainControllerSetup()
        {
            Instance.Logger.LogInfo($"Performing game initialization on behalf of {PluginInfo.PLUGIN_GUID}.");
            Instance.Logger.LogInfo($"The game has loaded {MainController.Instance.Settings.effects.Length} effects.");
        }

        [HarmonyPatch(typeof(GameController), nameof(GameController.StartGame))]
        [HarmonyPostfix]
        private static void HookEveryGameStart()
        {
            // Too difficult to predict when GameController will exist and I can hook observers to it
            // So just use Harmony and save us all some time. This method will run after every game start
            var isNewGame = MB.GameSaveService.IsNewGame();
            Instance.Logger.LogInfo($"Entered a game. Is this a new game: {isNewGame}.");

            if (isNewGame)
            {
                if (Configs.CheckForDuplicates)
                {
                    Instance.Logger.LogInfo("Safe and new games config chosen.");
                    CatGranter.SafeGrant();
                }
                else
                {
                    Instance.Logger.LogInfo("Unsafe and new games config chosen.");
                    CatGranter.UnsafeGrant();
                }
            }
            else if (Configs.AddToOngoingGames) // This has to be tidied up, I need Resharper or something for stuff like this.
            {
                if (Configs.CheckForDuplicates)
                {
                    Instance.Logger.LogInfo("Unsafe and new games config chosen.");
                    CatGranter.SafeGrant();
                }
                else
                {
                    Instance.Logger.LogInfo("Unsafe and all games config chosen.");
                    CatGranter.UnsafeGrant();
                }
            }
        }
    }
}
