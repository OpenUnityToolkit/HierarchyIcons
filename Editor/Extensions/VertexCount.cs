using UnityEngine;

using UnityEditor;

using OpenToolkit.HierarchyIcons.Settings;

namespace OpenToolkit.HierarchyIcons.Extensions
{
    public static class VertexCount
    {
        static readonly string KEY = $"{typeof(HierarchyLights).FullName}.config.";
        public static bool IsEnabled => _setting.Value;
        private static SettingBool _setting = new SettingBool(KEY + "showVerts", "Show vertex count readout for meshes")
        {
            Category = "Meshes"
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
            HierarchyIcons.OnDrawRow -= DrawVertexInfo;

            if (IsEnabled)
            {
                HierarchyIcons.OnDrawRow += DrawVertexInfo;
            }
        }

        static void DrawVertexInfo(IconData iconData, Rect rect, GUIStyle style)
        {
            if (iconData.Mesh != null)
            {
                string vertexInfo = iconData.Mesh.vertexCount.ToString("N0");
                DrawInfo(rect, vertexInfo, style);
            }
        }

        private static void DrawInfo(Rect itemRect, string text, GUIStyle style)
        {
            style.fontSize = 8;
            GUIContent verts = new GUIContent(text);
            Vector2 size = style.CalcSize(verts);

            Rect endRect = new Rect(itemRect);
            endRect.width = size.x + 4 + 4;
            endRect.x += itemRect.width - size.x - 4;

            endRect.width -= 4;
            endRect.x += 4;

            GUI.Label(endRect, verts, style);
        }
    }
}