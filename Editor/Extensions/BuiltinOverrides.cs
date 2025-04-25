using OpenToolkit.HierarchyIcons.Settings;
using OpenToolkit.HierarchyIcons.Utility;

using UnityEditor;
using UnityEngine;

namespace OpenToolkit.HierarchyIcons.Extensions
{
    public static class BuiltinOverrides
    {
        const string KEY = "BuiltinOverrides.config.";
        public static bool IsEnabled => s_setting.Value;
        private static SettingBool s_setting = new SettingBool(KEY + "alternateBuiltIn", "Show alternate builtin icons")
        {
            Category = "Icons",
            Tooltip = "Recoloured builtin icons to increase distinction, mainly affects UI components",
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
            HierarchyIcons.OnCreateIconData -= IconDataCreated;

            if (IsEnabled)
            {
                HierarchyIcons.OnCreateIconData += IconDataCreated;
            }
        }

        public static void IconDataCreated(IconData iconData)
        {
            if (!iconData.AllowOverride)
            {
                return;
            }

            if (iconData.Component == null)
            {
                return;
            }

            Texture2D customComponentIcon = IconUtil.LoadAsset("BuiltinOverrides/" + iconData.Component.GetType().Name);
            if (customComponentIcon != null)
            {
                iconData.Icon = customComponentIcon;
            }
        }
    }
}