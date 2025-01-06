using BepInEx;
using Eremite;
using Eremite.Controller;
using Eremite.Model;
using Eremite.Model.Meta;
using HarmonyLib;
using UnityEngine.Assertions.Must;

namespace GetSharkCat
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        private Harmony harmony;

        private void Awake()
        {
            Instance = this;
            harmony = Harmony.CreateAndPatchAll(typeof(Plugin));
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Configs.EffectEnabled = Configs.EffectEnabled; // I have no idea how to do this sensibly. Sorry!
        }

        [HarmonyPatch(typeof(MainController), nameof(MainController.OnServicesReady))]
        [HarmonyPostfix]
        private static void HookMainControllerSetup()
        {
            // This method will run after game load (Roughly on entering the main menu)
            // At this point a lot of the game's data will be available.
            // Your main entry point to access this data will be `Serviceable.Settings` or `MainController.Instance.Settings`
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
                GameNpcModel cat = SO.Settings.GetNpc("Cat");
                SO.NpcsService.RemoveNpc(cat); // To make sure no doubles are present when someone has the cycle reward.
                Instance.Logger.LogInfo("Cat NPC removed.");

                SO.NpcsService.SpawnNewNpc(cat, SO.BuildingsService.GetMainHearth().Entrance);
                Instance.Logger.LogInfo("Cat NPC added.");

                if (Configs.EffectEnabled && !SO.PerksService.HasPerk("[WE] First Elder Bonus"))
                {
                    EffectModel catEffectModel = MB.Settings.GetEffect("[WE] First Elder Bonus");
                    catEffectModel.Apply();
                    Instance.Logger.LogInfo("Cat effect cycle perk added.");

                    SO.NpcsService.RemoveNpc(cat); // Doing this because applying the perk spawns the cat.
                    Instance.Logger.LogInfo("Cat NPC removed.");

                }


            }
        }
    }
}
