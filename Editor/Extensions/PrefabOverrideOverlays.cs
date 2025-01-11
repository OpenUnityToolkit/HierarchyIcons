using UnityEngine;

using UnityEditor;

using OpenToolkit.HierarchyIcons.Settings;
using OpenToolkit.HierarchyIcons.Utility;

namespace OpenToolkit.HierarchyIcons.Extensions
{
    public static class PrefabOverrideOverlays
    {
        static readonly string KEY = $"{typeof(ManagerOverrides).FullName}.config.";
        public static bool IsEnabled => _setting.Value;
        private static SettingBool _setting = new SettingBool(KEY + "prefabOverlays", "Show prefab override overlays")
        {
            Category = "Overlays",
            Tooltip = "Shows a '+' for gameObjects added to a prefab and an 'o' for gameObjects with overrides",
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
            Texture2D overlay = GetOverrideOverlay(iconData.GameObject, iconData.Components);
            if (overlay != null)
            {
                iconData.IconOverlay = overlay;
            }
        }

        private static Texture2D GetOverrideOverlay(GameObject gameObject, Component[] components)
        {
            if (gameObject == null)
            {
                return null;
            }

            if (components == null)
            {
                return null;
            }

            Texture2D texture = null;

            if (PrefabUtility.IsAddedGameObjectOverride(gameObject))
            {
                texture = Colors.PrefabAddedOverlay;
            }
            else if (!PrefabUtility.IsAnyPrefabInstanceRoot(gameObject) && PrefabUtility.IsPartOfAnyPrefab(gameObject))
            {
                var overrides = PrefabUtility.GetObjectOverrides(gameObject);

                foreach (var objOverride in overrides)
                {
                    for (int i = 0; i < components.Length; i++)
                    {
                        if (components[i] == objOverride.instanceObject)
                        {
                            texture = Colors.PrefabModifiedOverlay;
                            break;
                        }
                    }

                    if (texture != null)
                    {
                        break;
                    }
                }

            }

            return texture;
        }

    }
}