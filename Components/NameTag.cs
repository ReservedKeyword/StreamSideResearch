using Il2CppTMPro;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

namespace StreamSideResearch.Components
{
    public class NameTag(System.IntPtr ptr) : MonoBehaviour(ptr)
    {
        private static readonly Mod mod = Mod.Instance;
        private static readonly ModConfig modConfig = mod.ModConfig;

        public Color Color { get; set; }
        public string DisplayName { get; set; }

        private Transform headBone;
        private GameObject nameTagObject;
        private NavMeshAgent navMeshAgent;

        public void Start()
        {
            CreateNameTag();
            FindHeadBone();
        }

        private void CreateNameTag()
        {
            nameTagObject = new($"NameTag_{DisplayName}");

            var canvas = nameTagObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var textObject = new GameObject("Text");
            textObject.transform.SetParent(nameTagObject.transform);
            textObject.transform.localPosition = Vector3.zero;
            textObject.transform.localScale = Vector3.one;

            var textRect = textObject.AddComponent<RectTransform>();
            textRect.sizeDelta = new(200, 500);

            var text = textObject.AddComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.Center;
            text.color = modConfig.UseTwitchColors ? Color : Color.white;
            text.fontSize = modConfig.TextFontSize;
            text.fontStyle = FontStyles.Bold;
            text.outlineColor = Color.black;
            text.outlineWidth = modConfig.TextOutlineWidth;
            text.text = DisplayName;

            nameTagObject.transform.localScale = Vector3.one * .005f;
        }

        private void FindHeadBone()
        {
            var animator = GetComponentInChildren<Animator>();

            if (animator != null)
            {
                headBone = animator.GetBoneTransform(HumanBodyBones.Head);
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

                var heightOffset = modConfig.HeighOffset;

                if (headBone != null)
                {
                    nameTagObject.transform.position = headBone.position + Vector3.up * heightOffset;
                }
                else
                {
                    nameTagObject.transform.position = npcPosition + Vector3.up * heightOffset;
                }

                var mainCamera = Camera.main;

                if (mainCamera != null)
                {
                    nameTagObject.transform.rotation = mainCamera.transform.rotation;
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
