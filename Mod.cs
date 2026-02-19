using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using StreamSideResearch.Components;
using StreamSideResearch.Managers;
#if IN_TEST_MODE
using Il2CppNPC;
using UnityEngine;
#endif

[assembly: HarmonyDontPatchAll]
[assembly: MelonColor(1, 160, 32, 240)]
[assembly: MelonGame("CyberneticWalrus", "Roadside Research")]
[assembly: MelonInfo(typeof(StreamSideResearch.Mod), "StreamSideResearch", "1.0.0", "ReservedKeyword")]

namespace StreamSideResearch
{
    public class Mod : MelonMod
    {
        internal static Mod Instance { get; private set; }

        internal ChatterManager ChatterManager { get; private set; }
        internal ModConfig ModConfig { get; private set; }

#if IN_TEST_MODE
        private DebugCheats debugCheats;
#endif
        private HarmonyLib.Harmony harmony;

        public override void OnInitializeMelon()
        {
            Instance = this;
            ModConfig = new();
            ChatterManager = new(this, ModConfig);

            ClassInjector.RegisterTypeInIl2Cpp<NameTag>();
            ChatterManager.Connect();

#if IN_TEST_MODE
            debugCheats = new(LoggerInstance);
#endif

            harmony = new("com.reservedkeyword.StreamSideResearch");
            harmony.PatchAll();

            LoggerInstance.Msg("Mod has finished initialization process!");
        }

#if IN_TEST_MODE
        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                debugCheats.Spawn(NPCType.Agent);
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                debugCheats.Spawn(NPCType.Customer);
            }
        }
#endif
    }
}
