using System.Globalization;

using UnityEngine;

using UnityEditor;

namespace OpenToolkit.HierarchyIcons.Utility
{
    public static class UIUtil
    {
        static string PLAY_MODE_DARKEN_KEY = "Playmode tint";

        public static void DrawIcon(Rect rect, Texture texture)
        {
            if (texture == null)
            {
                return;
            }

            if (texture.width > texture.height)
            {
                float ratio = (float)texture.height / texture.width;
                rect.y += (rect.height - (rect.height * ratio)) * 0.5f;
                rect.height *= ratio;
            }
            if (texture.height > texture.width)
            {

                float ratio = (float)texture.width / texture.height;
                rect.x += (rect.width - (rect.width * ratio)) * 0.5f;
                rect.width *= ratio;
            }

            GUI.DrawTexture(rect, texture);
        }

        public static GUIStyle GetRowStyle(IconData iconData, bool isSelected)
        {
            if (PrefabUtility.IsPartOfAnyPrefab(iconData.GameObject))
            {
                return GetPrefabStyle(iconData, isSelected);

            }
            else
            {
                return GetNormalStyle(iconData, isSelected);
            }
        }

        public static GUIStyle GetNormalStyle(IconData iconData, bool isSelected)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            if (iconData.GameObject.activeInHierarchy)
            {
                if (isSelected)
                {
                    style.normal.textColor = Color.white;
                    style.hover.textColor = Color.white;
                }
            }
            else
            {
                style.normal.textColor = Colors.TreeActive;
                style.hover.textColor = Colors.TreeActive;
            }

            return style;
        }

        public static GUIStyle GetPrefabStyle(IconData iconData, bool isSelected)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            if (!iconData.GameObject.activeInHierarchy)
            {
                if (isSelected)
                {
                    style.normal.textColor = Colors.Prefab;
                    style.hover.textColor = Colors.Prefab;
                }
                else
                {
                    style.normal.textColor = Colors.PrefabInactive;
                    style.hover.textColor = Colors.PrefabInactive;
                }
            }
            else
            {
                if (isSelected)
                {
                    style.normal.textColor = Color.white;
                    style.hover.textColor = Color.white;
                }
                else
                {
                    style.normal.textColor = Colors.Prefab;
                    style.hover.textColor = Colors.Prefab;
                }
            }

            return style;
        }

        public static Color ParsePlayModeColor()
        {
            return ParsePrefColor(PLAY_MODE_DARKEN_KEY);
        }

        public static Color ParsePrefColor(string key)
        {
            string s = EditorPrefs.GetString(key);
            string[] split = s.Split(';');

            split[1] = split[1].Replace(',', '.');
            split[2] = split[2].Replace(',', '.');
            split[3] = split[3].Replace(',', '.');
            split[4] = split[4].Replace(',', '.');

            bool success = float.TryParse(split[1], NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out var r);
            success &= float.TryParse(split[2], NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out var g);
            success &= float.TryParse(split[3], NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out var b);
            success &= float.TryParse(split[4], NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out var a);

            if (success)
            {
                return new Color(r, g, b, a);
            }
            else
            {
                return Color.white;
            }
        }
    }
}