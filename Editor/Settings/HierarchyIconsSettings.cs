using System;
using System.Collections.Generic;

using UnityEditor;

namespace OpenToolkit.HierarchyIcons.Settings
{
    static class HierarchyIconsSettings
    {
        private const string GROUP_KEY = "OpenToolkit.HierarchyIcons";
        private const string CONFIG_KEY = GROUP_KEY + ".config.";

        public static bool OVerallFeatureEnabled => s_featureEnabled.Value;
        private static SettingBool s_featureEnabled = new SettingBool(CONFIG_KEY + "featureEnabled", "Enabled");

        public static bool ShowRowBands => s_showRowBands.Value;
        private static SettingBool s_showRowBands = new SettingBool(CONFIG_KEY + "showRowBands", "Show banding on hierarchy rows")
        {
            Category = "View"
        };

        public static bool ShowTreeLines => s_showTreeLines.Value;
        private static SettingBool s_showTreeLines = new SettingBool(CONFIG_KEY + "showTreeLines", "Show hierarchy tree lines")
        {
            Category = "View"
        };
        public static bool ShowExpandedTreeLinesDotted => s_showExpandedTreeLinesDotted.Value;
        private static SettingBool s_showExpandedTreeLinesDotted = new SettingBool(CONFIG_KEY + "showExpandedTreeLinesDotted", "Dotted lines on expanded trees")
        {
            Category = "View"
        };

        static List<Setting> s_settings = new List<Setting>
        {
            s_showRowBands,
            s_showTreeLines,
            s_showExpandedTreeLinesDotted,
        };

        public static event Action OnSettingsChange;
        static SimpleSettingsProvider s_provider = new SimpleSettingsProvider("Open Toolkit/Hierarchy Icons", s_settings, s_featureEnabled);
        public static void Add(Setting setting)
        {
            s_provider.Add(setting);
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            s_provider.label = "Hierarchy Icons";

            s_provider.OnSettingsChange += () => OnSettingsChange?.Invoke();

            return s_provider;
        }
    }
}