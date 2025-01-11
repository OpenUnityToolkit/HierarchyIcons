using UnityEngine;

using UnityEditor;

using OpenToolkit.HierarchyIcons.Settings;
using OpenToolkit.HierarchyIcons.Utility;

namespace OpenToolkit.HierarchyIcons
{
    public static class HierarchyTreeLines
    {
        static Color s_editorTintColor = Color.white;

        [InitializeOnLoadMethod]
        public static void Init()
        {
            s_editorTintColor = UIUtil.ParsePlayModeColor();
        }

        public static void DrawTreeLines(Rect rowRect, GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            if (gameObject.transform.parent == null)
            {
                return;
            }

            if (!HierarchyIconsSettings.ShowTreeLines)
            {
                return;
            }

            Color cachedGuiColor = GUI.color;
            GUI.color = Colors.TreeActive;

            DrawJunctions(rowRect, gameObject);

            DrawExpandedLines(rowRect, gameObject);

            GUI.color = cachedGuiColor;
        }

        private static void DrawExpandedLines(Rect rowRect, GameObject gameObject)
        {
            Texture2D lineTex;

            if (HierarchyIconsSettings.ShowExpandedTreeLinesDotted)
            {
                lineTex = IconUtil.LoadAsset("TreeLines/subHierarchyDotted3");
            }
            else
            {
                lineTex = IconUtil.LoadAsset("TreeLines/subHierarchyLine");
            }


            Rect linedRect = new Rect(rowRect);
            linedRect.x -= 22;
            linedRect.width = 16;
            Transform nextRoot = gameObject.transform.parent;

            bool skip = true;
            while (nextRoot != null)
            {
                if (!skip)
                {
                    GUI.color = GetLineColor(nextRoot.gameObject);

                    GUI.DrawTexture(linedRect, lineTex, ScaleMode.StretchToFill, true);
                }

                skip = !(nextRoot.parent != null && nextRoot.transform.GetSiblingIndex() != nextRoot.parent.childCount - 1);

                nextRoot = nextRoot.parent;
                linedRect.x -= 14;
            }
        }

        private static Color GetLineColor(GameObject gameObject)
        {
            Color color = gameObject.activeInHierarchy ? Colors.TreeActive : Colors.TreeInactive;

            if (PrefabUtility.IsPartOfAnyPrefab(gameObject))
            {
                color = gameObject.activeInHierarchy ? Colors.PrefabTreeActive : Colors.PrefabTreeInactive;
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                color *= s_editorTintColor;
            }

            return color;
        }

        private static void DrawJunctions(Rect rowRect, GameObject gameObject)
        {
            Texture2D tex;

            Rect rect = new Rect(rowRect);
            rect.width = 16;
            rect.x -= 22;

            if (gameObject.transform.parent != null)
            {
                GUI.color = GetLineColor(gameObject.transform.parent.gameObject);
            }

            tex = GetJunctionTexture(gameObject);

            if (tex != null)
            {
                GUI.DrawTexture(rect, tex, ScaleMode.StretchToFill, true);
            }
        }

        private static Texture2D GetJunctionTexture(GameObject gameObject)
        {
            Texture2D tex;
            if (gameObject.transform.GetSiblingIndex() == gameObject.transform.parent.childCount - 1)
            {
                if (gameObject.transform.childCount > 0)
                {
                    tex = IconUtil.LoadAsset("TreeLines/subHierarchyEndShort");
                }
                else
                {
                    tex = IconUtil.LoadAsset("TreeLines/subHierarchyEnd");
                }
            }
            else
            {
                if (gameObject.transform.childCount > 0)
                {
                    tex = IconUtil.LoadAsset("TreeLines/subHierarchyJunctionShort");
                }
                else
                {
                    tex = IconUtil.LoadAsset("TreeLines/subHierarchyJunction");
                }
            }

            return tex;
        }
    }
}