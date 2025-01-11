using System;
using System.Collections.Generic;

using UnityEditor;

namespace OpenToolkit.HierarchyIcons.Settings
{
    static class HierarchyIconsSettings
    {
        private const string GROUP_KEY = "OpenToolkit.HierarchyIcons";
        private const string CONFIG_KEY = GROUP_KEY + ".config.";

        // OVerall Feature Enabled
        public static bool FeatureEnabled => s_featureEnabled.Value;
        private static SettingBool s_featureEnabled = new SettingBool(CONFIG_KEY + "featureEnabled", "Enabled");


        public static bool ShowRowBands => _showRowBands.Value;
        private static SettingBool _showRowBands = new SettingBool(CONFIG_KEY + "showRowBands", "Show banding on hierarchy rows")
        {
            Category = "View"
        };

        public static bool ShowTreeLines => _showTreeLines.Value;
        private static SettingBool _showTreeLines = new SettingBool(CONFIG_KEY + "showTreeLines", "Show hierarchy tree lines")
        {
            Category = "View"
        };
        public static bool ShowExpandedTreeLinesDotted => _showExpandedTreeLinesDotted.Value;
        private static SettingBool _showExpandedTreeLinesDotted = new SettingBool(CONFIG_KEY + "showExpandedTreeLinesDotted", "Dotted lines on expanded trees")
        {
            Category = "View"
        };


        static List<Setting> Settings = new List<Setting>
        {
            _showRowBands,
            _showTreeLines,
            _showExpandedTreeLinesDotted,
        };


        public static Action OnSettingsChange;
        public static void Add(Setting setting) => _provider.Add(setting);
        static SimpleSettingsProvider _provider = new SimpleSettingsProvider("Open Toolkit/Hierarchy Icons", Settings, s_featureEnabled);


        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            _provider.label = "Hierarchy Icons";

            _provider.OnSettingsChange += () => OnSettingsChange?.Invoke();

            return _provider;
        }
    }
}