﻿using BepInEx;
using BepInEx.Logging;
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
        internal static ManualLogSource myLog;

        private void Awake()
        {
            Instance = this;
            harmony = Harmony.CreateAndPatchAll(typeof(Plugin));
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Configs.EffectGrantingEnabled = Configs.EffectGrantingEnabled; // I have no idea how to do this sensibly. Sorry!
            myLog = Instance.Logger;
            myLog.LogInfo("OKOŃb");
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
                if (Configs.CheckForDuplicates)
                {
                    CatGranter.SafeGrant();
                }
                else
                {
                    CatGranter.UnsafeGrant();
                }
            }
        }
    }
}
