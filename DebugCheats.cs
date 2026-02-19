using Il2CppNPC;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace StreamSideResearch
{
    internal class DebugCheats(MelonLogger.Instance logger)
    {
        public void Spawn(NPCType npcType)
        {
            var npcManager = Object.FindObjectOfType<NPCManager>();

            if (npcManager != null)
            {
                var preSpawnedList = new List<StateMachine>();
                var networkObject = npcManager.PreSpawnNPC(preSpawnedList, npcType, NPCBodyType.Random);

                if (networkObject != null)
                {
                    npcManager.SpawnNPC(preSpawnedList);
                    logger.Msg($"Spawned {npcType} in the world!");
                }
                else
                {
                    logger.Warning($"PreSpawnNPC returned null for {npcType}");
                }
            }
        }
    }
}
