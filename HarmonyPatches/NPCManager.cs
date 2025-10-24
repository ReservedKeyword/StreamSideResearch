using BepInEx.Logging;
using Fusion;
using HarmonyLib;
using NPC;
using StreamSideResearch.TwitchIntegration;
using StreamSideResearch.UI;
using UnityEngine;

namespace StreamSideResearch.HarmonyPatches
{
    [HarmonyPatch(typeof(NPCManager))]
    static class NPCManagerPatches
    {
        private static readonly Plugin plugin = Plugin.Instance;
        private static readonly ChatterManager chatterManager = plugin.ChatterManager;
        private static readonly ManualLogSource logger = plugin.Logger;

        private static NPCBodyType DetermineBodyType(GameObject gameObject)
        {
            foreach (var meshRenderer in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                var meshName = meshRenderer.sharedMesh?.name.ToLower() ?? "";

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
        static void SpawnNPC_Postfix(NetworkObject __result, NPCType type, NPCBodyType bodyType)
        {
            logger.LogInfo($"SpawnNPC Postfix invoked with NPC type '{type}' and a body type '{bodyType}'.");

            if (__result == null || (type != NPCType.Agent && type != NPCType.Customer))
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
            logger.LogInfo($"Determined NPC body as: {actualBodyType}.");

            var bodyPreference = actualBodyType switch
            {
                NPCBodyType.Female => ChatterManager.BodyPreference.Female,
                NPCBodyType.Male => ChatterManager.BodyPreference.Male,
                _ => ChatterManager.BodyPreference.Any,
            };

            string chatterName = chatterManager.GetRandomChatter(npcType, bodyPreference);

            if (chatterName == null)
            {
                logger.LogWarning("Did not receive a chatter's name, cannot apply nametag.");
                return;
            }

            var nameTag = __result?.gameObject.AddComponent<NPCNameTag>();
            nameTag.ChatterName = chatterName;
            logger.LogInfo($"Assigning chatter {chatterName} to {bodyType} NPC!");
        }
    }
}
