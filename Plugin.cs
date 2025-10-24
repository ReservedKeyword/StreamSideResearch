using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using StreamSideResearch.TwitchIntegration;
using UnityEngine;

namespace StreamSideResearch
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance { get; private set; }

        internal ChatterManager ChatterManager { get; private set; }
        internal new ManualLogSource Logger { get; private set; }
        internal PluginConfig PluginConfig { get; private set; }

        private DebugCheats debugCheats;
        private Harmony harmony;

        public void Awake()
        {
            Instance = this;
            Logger = base.Logger;
            PluginConfig = new(this);

            ChatterManager = new(PluginConfig, Logger);
            ChatterManager.Connect();

            debugCheats = new(Logger);

            harmony = new Harmony("com.reservedkeyword.StreamSideResearch");
            harmony.PatchAll();

            Logger.LogInfo($"Plugin has fully loaded!");
        }

        public void Update()
        {
            if (!PluginConfig.EnableCheats.Value)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                debugCheats.SpawnAgent();
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                debugCheats.SpawnCustomer();
            }
        }
    }
}
