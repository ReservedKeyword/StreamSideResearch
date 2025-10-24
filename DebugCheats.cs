using BepInEx.Logging;
using NPC;
using UnityEngine;

namespace StreamSideResearch
{
    public class DebugCheats(ManualLogSource logger)
    {
        public void SpawnAgent()
        {
            var npcManager = Object.FindFirstObjectByType<NPCManager>();

            if (npcManager != null)
            {
                npcManager.SpawnNPC(NPCType.Agent, NPCBodyType.Random);
                logger.LogInfo($"Invoked SpawnNPC on Agent!");
            }
        }

        public void SpawnCustomer()
        {
            var npcManager = Object.FindFirstObjectByType<NPCManager>();

            if (npcManager != null)
            {
                npcManager.SpawnNPC(NPCType.Customer, NPCBodyType.Random);
                logger.LogInfo($"Invoked SpawnNPC on Customer!");
            }
        }
    }
}
