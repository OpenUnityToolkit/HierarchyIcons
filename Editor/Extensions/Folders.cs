using UnityEngine;

using UnityEditor;

using OpenToolkit.HierarchyIcons.Settings;
using OpenToolkit.HierarchyIcons.Utility;

namespace OpenToolkit.HierarchyIcons.Extensions
{
    public static class HierarchyFolder
    {
        static readonly string KEY = $"{typeof(HierarchyFolder).FullName}.config.";
        public static bool IsEnabled => _setting.Value;
        private static SettingBool _setting = new SettingBool(KEY + "emptyFolderParents", "Show empty parents as folders")
        {
            Category = "Icons"
        };

        [InitializeOnLoadMethod]
        public static void Init()
        {
            DoSubscriptions();

            HierarchyIconsSettings.OnSettingsChange += DoSubscriptions;

            HierarchyIconsSettings.Add(_setting);
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