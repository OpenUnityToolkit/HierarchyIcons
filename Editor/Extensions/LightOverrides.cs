using OpenToolkit.HierarchyIcons.Settings;
using OpenToolkit.HierarchyIcons.Utility;

using UnityEditor;
using UnityEngine;

namespace OpenToolkit.HierarchyIcons.Extensions
{
    public static class LightOverrides
    {
        const string KEY = "LightOverrides.config.";
        public static bool IsEnabled => s_setting.Value;
        private static SettingBool s_setting = new SettingBool(KEY + "showLightColors", "Show light colours")
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
            HierarchyIcons.OnCreateIconData -= SetLightIcons;
            HierarchyIcons.OnDrawRow -= UpdateLightColors;

            if (IsEnabled)
            {
                HierarchyIcons.OnCreateIconData += SetLightIcons;
                HierarchyIcons.OnDrawRow += UpdateLightColors;
            }
        }

        static void UpdateLightColors(IconData iconData, Rect rect, GUIStyle style)
        {
            if (iconData.Component is Light light)
            {
                iconData.ColorOverride = light.color;
            }
        }

        static void SetLightIcons(IconData iconData)
        {
            if (!iconData.AllowOverride)
            {
                return;
            }

            if (iconData.Component is Light light)
            {
                if (light.type == LightType.Spot)
                {
                    SetLight(iconData, "Lights/spot_light", "Lights/spot_fixture");
                }
                else if (light.type == LightType.Directional)
                {
                    SetLight(iconData, "Lights/directional_light", "Lights/directional_fixture");
                }
                else if (light.type == LightType.Rectangle)
                {
                    SetLight(iconData, "Lights/area_light", "Lights/area_fixture");
                }
                else
                {
                    SetLight(iconData, "Lights/light_light", "Lights/light_fixture");
                }
            }
        }

        static void SetLight(IconData iconData, string lightPath, string pathFixture)
        {
            Texture2D lightIcon = IconUtil.LoadAsset(lightPath);
            Texture2D fixtureIcon = IconUtil.LoadAsset(pathFixture);

            iconData.IconUncolored = fixtureIcon;
            iconData.Icon = lightIcon;
            iconData.HasColorOverride = true;
        }
    }
}