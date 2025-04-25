using OpenToolkit.HierarchyIcons.Settings;
using OpenToolkit.HierarchyIcons.Utility;

using UnityEditor;
using UnityEngine;

namespace OpenToolkit.HierarchyIcons.Extensions
{
    public static class FolderOverrides
    {
        const string KEY = "FolderOverrides.config.";
        public static bool IsEnabled => s_setting.Value;
        private static SettingBool s_setting = new SettingBool(KEY + "emptyFolderParents", "Show empty parents as folders")
        {
            Category = "Icons"
        };

        [InitializeOnLoadMethod]
        public static void Init()
        {
            DoSubscriptions();

            HierarchyIconsSettings.OnSettingsChange += DoSubscriptions;

            HierarchyIconsSettings.Add(s_setting);
        }

        static void DoSubscriptions()
        {
            HierarchyIcons.OnCreateIconData -= FindFolderIcons;

            if (IsEnabled)
            {
                HierarchyIcons.OnCreateIconData += FindFolderIcons;
            }
        }

        public static void FindFolderIcons(IconData iconData)
        {
            if (!iconData.AllowOverride)
            {
                return;
            }

            if (iconData.Component is Transform transform)
            {
                if (transform.childCount == 0)
                {
                    return;
                }

                iconData.IconExpanded = IconUtil.LoadBuiltinIcon("FolderOpened");
                iconData.Icon = IconUtil.LoadBuiltinIcon("Folder");
                iconData.HideIconWhenPrefab = true;
            }
        }
    }
}