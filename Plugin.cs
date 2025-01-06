﻿using BepInEx;
using Eremite;
using Eremite.Controller;
using Eremite.Model;
using HarmonyLib;

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
                // add the shark cat npc (if not present - to figure out, now you'll get doubles)
                // depending on config add the effect
                GameNpcModel cat = SO.Settings.GetNpc("Cat");
                SO.NpcsService.SpawnNewNpc(cat, SO.BuildingsService.GetMainHearth().Entrance);
                SO.NpcsService.Register(cat.prefab);
                Instance.Logger.LogInfo("Cat NPC added.");

                EffectModel catEffect = MB.Settings.GetEffect("[WE] First Elder Cat Resolve");
                catEffect.Apply();
                Instance.Logger.LogInfo("Cat effect perk added.");
            }
        }
    }
}
