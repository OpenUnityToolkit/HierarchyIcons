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
        private static SettingBool _setting = new SettingBool(KEY + "managerOverrides", "Show managers icons")
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
            if (!iconData.AllowOverride)
            {
                return;
            }

            // this means we have a gameObject icon, which we won't want to override
            if (iconData.Icon != null && iconData.Component == null)
            {
                return;
            }

            // don't affect the default classes containing our keywords
            if (iconData.Component != null && (
                iconData.Component is ParticleSystem ||
                iconData.Component is ParticleSystemForceField ||
                iconData.Component is UnityEngine.EventSystems.EventSystem ||
                iconData.Component is CharacterController ||
                iconData.Component is StreamingController))
            {
                return;
            }

            Texture2D managerIcon = GetManagerIcon(iconData.GameObject, iconData.Component);
            if (managerIcon != null)
            {
                iconData.Icon = managerIcon;
            }
        }

        private static Texture2D GetManagerIcon(GameObject gameObject, Component component)
        {
            Texture2D stringIcon = FindStringMatchIcons(gameObject.name);
            if (stringIcon != null)
            {
                return stringIcon;
            }

            if (component == null)
            {
                return null;
            }

            stringIcon = FindStringMatchIcons(component.GetType().Name);
            if (stringIcon != null)
            {
                return stringIcon;
            }

            return null;
        }

        static Texture2D FindStringMatchIcons(string componentName)
        {
            if (IsManager(componentName))
            {
                return IconUtil.LoadAsset("Managers/cog");
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