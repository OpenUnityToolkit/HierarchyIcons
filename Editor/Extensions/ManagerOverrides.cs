using UnityEngine;

using UnityEditor;

using OpenToolkit.HierarchyIcons.Settings;
using OpenToolkit.HierarchyIcons.Utility;

namespace OpenToolkit.HierarchyIcons.Extensions
{
    public static class ManagerOverrides
    {
        static readonly string KEY = $"{typeof(ManagerOverrides).FullName}.config.";
        public static bool IsEnabled => _setting.Value;
        private static SettingBool _setting = new SettingBool(KEY + "managerOverrides", "Show managers as cogs")
        {
            Category = "Icons",
            Tooltip = "Component types or Game Object names containing 'manager', 'system', or 'controller' are shown as cogs",
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
            HierarchyIcons.OnCreateIconData -= IconDataCreated;

            if (IsEnabled)
            {
                HierarchyIcons.OnCreateIconData += IconDataCreated;
            }
        }

        public static void IconDataCreated(IconData iconData)
        {
            Texture2D stringIcon = FindStringMatchIcons(iconData.GameObject.name);
            if (stringIcon != null)
            {
                iconData.Icon = stringIcon;
                return;
            }

            if (iconData.Component == null)
            {
                return;
            }

            stringIcon = FindStringMatchIcons(iconData.Component.GetType().Name);
            if (stringIcon != null)
            {
                iconData.Icon = stringIcon;
                return;
            }
        }

        static Texture2D FindStringMatchIcons(string componentName)
        {
            if (IsManager(componentName))
            {
                return IconUtil.LoadAsset("hierarchy/cog");
            }

            return null;
        }

        public static bool IsManager(string name)
        {
            if (name.ToLower().Contains("manager"))
            {
                return true;
            }

            if (name.ToLower().Contains("controller"))
            {
                return true;
            }

            if (name.ToLower().Contains("system"))
            {
                return true;
            }

            return false;
        }
    }
}