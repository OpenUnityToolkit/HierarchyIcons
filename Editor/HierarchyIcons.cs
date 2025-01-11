using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.IMGUI.Controls;

using OpenToolkit.HierarchyIcons.Settings;
using OpenToolkit.HierarchyIcons.Utility;

namespace OpenToolkit.HierarchyIcons
{
    public static class HierarchyIcons
    {
        static Dictionary<int, IconData> s_iconCache = new Dictionary<int, IconData>();

        static int[] s_lastSelectionIDs = new int[0];
        static StageHandle s_lastStage;


        static Color s_editorTint = Color.white;
        static object s_treeController;

        static List<SearchableEditorWindow> s_hierarchyWindows;

        static double s_lastFrame;

        public static Action OnClearCache;
        public static Action<IconData> OnCreateIconData;
        public static Action<IconData, Rect, GUIStyle> OnDrawRow;


        [InitializeOnLoadMethod]
        public static void Init()
        {
            ReflectionHelper.InitReflection();

            ClearIconCache();

            DoSubscriptions();

            s_editorTint = UIUtil.ParsePlayModeColor();

            s_lastStage = StageUtility.GetCurrentStageHandle();

            HierarchyIconsSettings.OnSettingsChange += SettingChange;
        }

        static void SettingChange()
        {
            DoSubscriptions();

            s_hierarchyWindows = ReflectionHelper.GetAllSceneHierarchyWindows();

            ClearIconCache();
        }

        private static void DoSubscriptions()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItemCallback;
            EditorSceneManager.sceneOpened -= SceneOpened;
            Selection.selectionChanged -= OnSelectionChange;
            HierarchyIconsSettings.OnSettingsChange -= ClearIconCache;

            if (HierarchyIconsSettings.FeatureEnabled)
            {
                EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;
                EditorSceneManager.sceneOpened += SceneOpened;
                Selection.selectionChanged += OnSelectionChange;
                HierarchyIconsSettings.OnSettingsChange += ClearIconCache;
            }
        }

        static void OpenedSceneInit()
        {
            EditorApplication.update -= OpenedSceneInit;
            ClearIconCache();
        }

        static void SceneOpened(Scene scene, OpenSceneMode mode)
        {
            EditorApplication.update -= OpenedSceneInit;
            EditorApplication.update += OpenedSceneInit;
        }

        static void OnSelectionChange()
        {
            StageHandle currentStage = StageUtility.GetCurrentStageHandle();

            if (s_lastStage != currentStage)
            {
                ClearIconCache();
                s_lastStage = currentStage;
            }
            else
            {
                ClearFromIconCache(s_lastSelectionIDs);
                ClearFromIconCache(Selection.instanceIDs);
            }

            s_lastSelectionIDs = Selection.instanceIDs;
        }

        public static void ClearFromIconCache(params int[] instances)
        {
            foreach (int instanceId in instances)
            {
                s_iconCache.Remove(instanceId);
            }
        }

        static void ClearIconCache()
        {
            s_iconCache.Clear();

            OnClearCache?.Invoke();

            if (s_hierarchyWindows != null)
            {
                foreach (var window in s_hierarchyWindows)
                {
                    window?.Repaint();
                }
            }
        }

        static void HierarchyWindowItemCallback(int instanceID, Rect itemRect)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            // only get the active windows every so often
            if (EditorApplication.timeSinceStartup > s_lastFrame + 0.1)
            {
                s_lastFrame = EditorApplication.timeSinceStartup;

                s_hierarchyWindows = ReflectionHelper.GetAllSceneHierarchyWindows();
            }

            TreeViewUtil.GetItemFromWindows(instanceID, s_hierarchyWindows, out TreeViewItem treeItem, out s_treeController);

            if (treeItem == null)
            {
                return;
            }

            DrawRow(instanceID, itemRect, treeItem);
        }

        private static void DrawRow(int instanceID, Rect itemRect, TreeViewItem treeItem)
        {
            IconData iconData;

            if (s_iconCache.ContainsKey(instanceID) && s_iconCache[instanceID] != null)
            {
                iconData = s_iconCache[instanceID];
            }
            else
            {
                GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                if (gameObject == null)
                {
                    return;
                }

                iconData = ComponentUtil.IconFromGameObject(gameObject);
                s_iconCache.Add(instanceID, iconData);

                OnCreateIconData?.Invoke(iconData);
            }

            if (iconData == null || iconData.GameObject == null)
            {
                return;
            }

            bool isOdd = (int)(itemRect.y / itemRect.height) % 2 == 0;

            if (HierarchyIconsSettings.ShowRowBands)
            {
                // we do the background twice as we don't draw over the prefab 
                // open button with the current background draw, but want the
                // colours to match
                DrawNextBackground(itemRect, isOdd);
            }

            TreeViewUtil.GetHierarchyWindowStatus(out bool isWindowFocused, out bool searching);
            TreeViewUtil.GetRowStatus(treeItem, s_treeController, out bool isSelected, out bool isHovering);

            if (!searching)
            {
                HierarchyTreeLines.DrawTreeLines(itemRect, iconData.GameObject);
            }

            if (!HierarchyIconsSettings.ShowRowBands)
            {
                isOdd = false;
            }

            var backgroundColor = Colors.GetBackgroundColour(isWindowFocused, isOdd, isSelected, isHovering);
            var style = UIUtil.GetRowStyle(iconData, isSelected);

            DrawLabelAndIcon(iconData, itemRect, style, backgroundColor);
        }

        private static void DrawLabelAndIcon(IconData iconData, Rect itemRect, GUIStyle style, Color backgroundColor)
        {
            DrawColor(itemRect, backgroundColor);

            Rect labelRect = new Rect(itemRect);
            labelRect.x += 17;
            labelRect.y -= 1;

            string labelString = iconData.GameObject.name;

            GUI.Label(labelRect, labelString, style);

            bool isExpanded = ReflectionHelper.IsExpanded(iconData.GameObject.GetInstanceID(), s_treeController);

            Rect iconRect = new Rect(itemRect);
            iconRect.width = 16;

            OnDrawRow?.Invoke(iconData, itemRect, style);

            DrawIconData(iconRect, iconData, backgroundColor, isExpanded);

            if (!iconData.GameObject.activeInHierarchy)
            {
                Color halfBack = backgroundColor;
                halfBack.a *= 0.5f;

                DrawColor(iconRect, halfBack);
            }
        }

        public static void DrawIconData(Rect rect, IconData iconData, Color backgroundColor, bool isExpanded)
        {
            Color lastGUIColor = GUI.color;

            if (iconData.PrefabIcon != null)
            {
                UIUtil.DrawIcon(rect, iconData.PrefabIcon);

                if (!iconData.HideIconWhenPrefab)
                {
                    GUI.color = backgroundColor;
                    UIUtil.DrawIcon(rect, Colors.OverlayBackground);

                    rect.width /= 2;
                    rect.height /= 2;
                    rect.x += rect.width;
                    rect.y += rect.height;
                }
            }

            GUI.color = lastGUIColor;

            if (iconData.HasColorOverride)
            {
                GUI.color = iconData.ColorOverride;
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                GUI.color *= s_editorTint;
            }

            Texture2D icon = iconData.IconExpanded != null && isExpanded ? iconData.IconExpanded : iconData.Icon;

            if (iconData.PrefabIcon == null || !iconData.HideIconWhenPrefab)
            {
                UIUtil.DrawIcon(rect, icon);
            }

            GUI.color = lastGUIColor;

            if (iconData.IconUncolored != null)
            {
                UIUtil.DrawIcon(rect, iconData.IconUncolored);
            }

            if (iconData.IconOverlay != null)
            {
                UIUtil.DrawIcon(rect, iconData.IconOverlay);
            }
        }

        public static void DrawNextBackground(Rect rect, bool isOdd)
        {
            if (!isOdd)
            {
                rect.width += rect.x;
                rect.x = 32;

                rect.y += rect.height;

                DrawColor(rect, Colors.BackingAlt);
            }
        }

        public static void DrawColor(Rect rect, Color color)
        {
            Color lastCol = GUI.color;
            GUI.color = color;

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                GUI.color *= s_editorTint;
            }

            GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
            GUI.color = lastCol;
        }
    }
}