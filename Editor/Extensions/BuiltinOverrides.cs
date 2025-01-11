using UnityEngine;

using UnityEditor;

using OpenToolkit.HierarchyIcons.Settings;
using OpenToolkit.HierarchyIcons.Utility;

namespace OpenToolkit.HierarchyIcons.Extensions
{
    public static class BuiltinOverrides
    {
        static readonly string KEY = $"{typeof(BuiltinOverrides).FullName}.config.";
        public static bool IsEnabled => _setting.Value;
        private static SettingBool _setting = new SettingBool(KEY + "alternateBuiltIn", "Show alternate builtin icons")
        {
            Category = "Icons",
            Tooltip = "Recoloured builtin icons to increase distinction, mainly affects UI components",
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