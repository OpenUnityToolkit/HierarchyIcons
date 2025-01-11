using System.Collections.Generic;

using UnityEngine;

using UnityEditor;

using OpenToolkit.HierarchyIcons.Settings;
using OpenToolkit.HierarchyIcons.Utility;

namespace OpenToolkit.HierarchyIcons.Extensions
{
    public static class BoneOverrides
    {
        static readonly string KEY = $"{typeof(BoneOverrides).FullName}.config.";
        public static bool IsEnabled => _setting.Value;
        private static SettingBool _setting = new SettingBool(KEY + "boneIcons", "Show icon for skinned mesh bones", defaultValue: false)
        {
            Category = "Icons",
        };

        static HashSet<Transform> bones = new HashSet<Transform>();

        [InitializeOnLoadMethod]
        public static void Init()
        {
            DoSubscriptions();

            HierarchyIconsSettings.OnSettingsChange += DoSubscriptions;

            HierarchyIconsSettings.Add(_setting);
        }

        static void DoSubscriptions()
        {
            HierarchyIcons.OnClearCache -= bones.Clear;
            HierarchyIcons.OnCreateIconData -= RecordBones;

            if (IsEnabled)
            {
                HierarchyIcons.OnClearCache += bones.Clear;
                HierarchyIcons.OnCreateIconData += RecordBones;
            }
        }


        private static void RecordBones(IconData iconData)
        {
            if (!iconData.AllowOverride)
            {
                return;
            }

            var skinnedMeshRenderer = iconData.GameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                var skinnedMeshBones = skinnedMeshRenderer.bones;
                foreach (var bone in skinnedMeshBones)
                {
                    bones.Add(bone);

                    HierarchyIcons.ClearFromIconCache(bone.gameObject.GetInstanceID());
                }
            }

            // is a bone
            if (bones.Contains(iconData.GameObject.transform))
            {
                iconData.Icon = IconUtil.LoadAsset("Mesh/bone");
            }
        }
    }
}