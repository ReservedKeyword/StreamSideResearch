using HarmonyLib;
using Il2CppNPC;
using MelonLoader;
using StreamSideResearch.Components;
using StreamSideResearch.Managers;
using UnityEngine;

namespace StreamSideResearch.Patches
{
    [HarmonyPatch(typeof(NPCManager))]
    static class SpawnNPCPatch
    {
        private static readonly Mod mod = Mod.Instance;
        private static readonly ChatterManager chatterManager = mod.ChatterManager;
        private static readonly MelonLogger.Instance logger = mod.LoggerInstance;

        private static NPCBodyType DetermineBodyType(GameObject gameObject)
        {
            foreach (var meshRenderer in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                var meshName = meshRenderer.sharedMesh.name.ToLower() ?? "";

                if (meshName.Contains("female"))
                {
                    return NPCBodyType.Female;
                }

                if (meshName.Contains("male"))
                {
                    return NPCBodyType.Male;
                }
            }

            return NPCBodyType.Random;
        }

        [HarmonyPatch(nameof(NPCManager.SpawnNPC))]
        [HarmonyPostfix]
        static void OnNPCSpawned(StateMachine __result)
        {
            if (__result == null)
            {
                logger.Warning($"SpawnNPC returned null StateMachine");
                return;
            }

            NPCType type = __result.NPCType;
            logger.Msg($"SpawnNPC Postfix invoked with NPC type '{type}'");

            if (type != NPCType.Agent && type != NPCType.Customer)
            {
                return;
            }

            var npcType = type switch
            {
                NPCType.Agent => ChatterManager.NPCType.Agent,
                NPCType.Customer => ChatterManager.NPCType.Customer,
                _ => ChatterManager.NPCType.Unknown,
            };

            var actualBodyType = DetermineBodyType(__result.gameObject);
            logger.Msg($"Determined NPC body type as: {actualBodyType}");

            var appearancePreference = actualBodyType switch
            {
                NPCBodyType.Female => ChatterManager.AppearancePreference.Female,
                NPCBodyType.Male => ChatterManager.AppearancePreference.Male,
                _ => ChatterManager.AppearancePreference.Any,
            };

            var selectedChatter = chatterManager.GetRandomChatter(npcType, appearancePreference);

            if (selectedChatter == null)
            {
                logger.Warning("Did not receive a chatter's name, can't apply name tag");
                return;
            }

            var nameTag = __result.gameObject.AddComponent<NameTag>();
            nameTag.Color = selectedChatter.Color;
            nameTag.DisplayName = selectedChatter.DisplayName;

            logger.Msg($"Assigned chatter {selectedChatter.DisplayName} to {actualBodyType} NPC!");
        }
    }
}
