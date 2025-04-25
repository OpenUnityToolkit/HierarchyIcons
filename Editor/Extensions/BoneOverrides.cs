using System.Collections.Generic;

using OpenToolkit.HierarchyIcons.Settings;
using OpenToolkit.HierarchyIcons.Utility;

using UnityEditor;
using UnityEngine;

namespace OpenToolkit.HierarchyIcons.Extensions
{
    public static class BoneOverrides
    {
        const string KEY = "BoneOverrides.config.";
        public static bool IsEnabled => s_setting.Value;
        private static SettingBool s_setting = new SettingBool(KEY + "boneIcons", "Show icon for skinned mesh bones", defaultValue: false)
        {
            Category = "Icons",
        };

        static HashSet<Transform> s_bones = new HashSet<Transform>();

        [InitializeOnLoadMethod]
        public static void Init()
        {
            DoSubscriptions();

            HierarchyIconsSettings.OnSettingsChange += DoSubscriptions;

            HierarchyIconsSettings.Add(s_setting);
        }

        static void DoSubscriptions()
        {
            HierarchyIcons.OnClearCache -= s_bones.Clear;
            HierarchyIcons.OnCreateIconData -= RecordBones;

            if (IsEnabled)
            {
                HierarchyIcons.OnClearCache += s_bones.Clear;
                HierarchyIcons.OnCreateIconData += RecordBones;
            }
        }

        private static void RecordBones(IconData iconData)
        {
            if (!iconData.AllowOverride)
            {
                return;
            }

            if (iconData.GameObject.TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer))
            {
                var skinnedMeshBones = skinnedMeshRenderer.bones;
                foreach (var bone in skinnedMeshBones)
                {
                    s_bones.Add(bone);

                    HierarchyIcons.ClearFromIconCache(bone.gameObject.GetInstanceID());
                }
            }

            // is a bone
            if (s_bones.Contains(iconData.GameObject.transform))
            {
                iconData.Icon = IconUtil.LoadAsset("Mesh/bone");
            }
        }
    }
}