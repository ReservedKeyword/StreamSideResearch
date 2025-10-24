using BepInEx.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace StreamSideResearch.UI
{
    public class NPCNameTag : MonoBehaviour
    {
        private static readonly Plugin plugin = Plugin.Instance;
        private static readonly PluginConfig config = plugin.PluginConfig;
        private static readonly ManualLogSource logger = plugin.Logger;

        public string ChatterName { get; set; }

        private Transform headBone;
        private GameObject nameTagObject;
        private NavMeshAgent navMeshAgent;

        public void Start()
        {
            if (string.IsNullOrEmpty(ChatterName))
            {
                // This is here as a failsafe, but it should never get called. This component
                // _should_ not get added if there is no viewer name to assign to it.
                ChatterName = "Viewer";
            }

            CreateNameTag();
            FindHeadBone();
        }

        private void CreateNameTag()
        {
            nameTagObject = new GameObject("NameTag");

            var canvas = nameTagObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var textObject = new GameObject("Text");
            textObject.transform.SetParent(nameTagObject.transform);
            textObject.transform.localPosition = Vector3.zero;
            textObject.transform.localScale = Vector3.one;

            var textRect = textObject.AddComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(200, 50);

            var text = textObject.AddComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.fontSize = config.TextFontSize.Value;
            text.outlineColor = Color.black;
            text.outlineWidth = config.TextOutlineWidth.Value;
            text.text = ChatterName;

            nameTagObject.transform.localScale = Vector3.one * 0.005f;
        }

        private void FindHeadBone()
        {
            var animator = GetComponentInChildren<Animator>();

            if (animator != null)
            {
                headBone = animator.GetBoneTransform(HumanBodyBones.Head);
                logger.LogInfo($"Setting head bone for NPC to: {headBone}");
            }
        }

        public void LateUpdate()
        {
            if (navMeshAgent == null)
            {
                navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            }

            if (nameTagObject != null)
            {
                Vector3 npcPosition;

                if (navMeshAgent != null && navMeshAgent.enabled)
                {
                    npcPosition = navMeshAgent.transform.position;
                }
                else
                {
                    npcPosition = transform.position;
                }

                var heightOffset = config.HeightOffset.Value;

                if (headBone != null)
                {
                    nameTagObject.transform.position = headBone.position + Vector3.up * heightOffset;
                }
                else
                {
                    nameTagObject.transform.position = npcPosition + Vector3.up * heightOffset;
                }

                if (Camera.main != null)
                {
                    nameTagObject.transform.rotation = Camera.main.transform.rotation;
                }
            }
        }

        public void OnDestroy()
        {
            if (nameTagObject != null)
            {
                Destroy(nameTagObject);
            }
        }
    }
}
