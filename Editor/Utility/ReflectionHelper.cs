using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace OpenToolkit.HierarchyIcons
{
    public static class ReflectionHelper
    {
        static BindingFlags FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        static Type SceneHierarchyWindowType = typeof(Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
        static Type TreeViewControllerType = typeof(Editor).Assembly.GetType("UnityEditor.IMGUI.Controls.TreeViewController");
        static Type ITreeViewDataSourceType = typeof(Editor).Assembly.GetType("UnityEditor.IMGUI.Controls.ITreeViewDataSource");
        static Type TreeViewStateType = typeof(Editor).Assembly.GetType("UnityEditor.IMGUI.Controls.TreeViewState");
        static Type RenameOverlayType = typeof(Editor).Assembly.GetType("UnityEditor.RenameOverlay");

        static MethodInfo s_isRenamingMethod;
        static PropertyInfo s_stateRenameOverlayProperty;
        static PropertyInfo s_stateProperty;
        public static bool IsTreeRenaming(object treeController)
        {
            if (s_isRenamingMethod == null)
            {
                s_isRenamingMethod = RenameOverlayType.GetMethod("IsRenaming");
            }

            if (s_stateRenameOverlayProperty == null)
            {
                s_stateRenameOverlayProperty = TreeViewStateType.GetProperty("renameOverlay", FLAGS);
            }

            if (s_stateProperty == null)
            {
                s_stateProperty = TreeViewControllerType.GetProperty("state", FLAGS);
            }

            var state = s_stateProperty.GetValue(treeController);
            var renameOverlay = s_stateRenameOverlayProperty.GetValue(state);
            bool? isRenaming = s_isRenamingMethod.Invoke(renameOverlay, null) as bool?;

            return isRenaming == true;
        }

        public static object GetTreeController(EditorWindow hierarchyWindow)
        {
            if (hierarchyWindow == null)
            {
                return null;
            }

            if (hierarchyWindow.GetType().Name != "SceneHierarchyWindow")
            {
                return null;
            }

            var prop = hierarchyWindow.GetType().GetProperty("sceneHierarchy");
            var sceneHierarchy = prop.GetValue(hierarchyWindow);

            var treeViewMethod = sceneHierarchy.GetType().GetProperty("treeView", FLAGS);
            var controller = treeViewMethod.GetValue(sceneHierarchy);

            return controller;
        }

        static PropertyInfo s_dataProperty;
        static MethodInfo s_getItemMethod;

        public static TreeViewItem GetItem(int row, object treeController)
        {
            if (s_dataProperty == null)
            {
                s_dataProperty = TreeViewControllerType.GetProperty("data", FLAGS);
            }

            if (s_getItemMethod == null)
            {
                s_getItemMethod = ITreeViewDataSourceType.GetMethod("GetItem", FLAGS);
            }

            var data = s_dataProperty.GetValue(treeController);
            if (GetRowCount(treeController) <= row)
            {
                return null;
            }

            var item = s_getItemMethod.Invoke(data, new object[] { row }) as TreeViewItem;
            return item;
        }

        static MethodInfo s_getRowMethod;
        public static int GetRow(int id, object treeController)
        {
            if (s_dataProperty == null)
            {
                s_dataProperty = TreeViewControllerType.GetProperty("data", FLAGS);
            }

            if (s_getRowMethod == null)
            {
                s_getRowMethod = ITreeViewDataSourceType.GetMethod("GetRow", FLAGS);
            }

            var data = s_dataProperty.GetValue(treeController);
            return (int)s_getRowMethod.Invoke(data, new object[] { id });
        }

        static PropertyInfo s_getRowCountProperty;
        public static int GetRowCount(object treeController)
        {
            if (s_dataProperty == null)
            {
                s_dataProperty = TreeViewControllerType.GetProperty("data", FLAGS);
            }

            if (s_getRowCountProperty == null)
            {
                s_getRowCountProperty = ITreeViewDataSourceType.GetProperty("rowCount", FLAGS);
            }

            var data = s_dataProperty.GetValue(treeController);
            return (int)s_getRowCountProperty.GetValue(data, new object[] { });
        }

        static MethodInfo s_isExpandedMethod;
        public static bool IsExpanded(int id, object treeController)
        {
            if (s_dataProperty == null)
            {
                s_dataProperty = TreeViewControllerType.GetProperty("data", FLAGS);
            }

            if (s_isExpandedMethod == null)
            {
                s_isExpandedMethod = ITreeViewDataSourceType.GetMethod("IsExpanded", new Type[] { typeof(int) });
            }

            var data = s_dataProperty.GetValue(treeController);
            return (bool)s_isExpandedMethod.Invoke(data, new object[] { id });
        }

        static MethodInfo s_isItemDragSelectedOrSelectedMethod;
        public static bool IsItemDragSelectedOrSelected(TreeViewItem item, object treeController)
        {
            if (s_isItemDragSelectedOrSelectedMethod == null)
            {
                s_isItemDragSelectedOrSelectedMethod = TreeViewControllerType.GetMethod("IsItemDragSelectedOrSelected", FLAGS);
            }
            return (bool)s_isItemDragSelectedOrSelectedMethod.Invoke(treeController, new object[] { item });
        }

        static PropertyInfo s_hoverItemProperty;
        public static TreeViewItem GetHoverItem(object treeController)
        {
            if (s_hoverItemProperty == null)
            {
                s_hoverItemProperty = TreeViewControllerType.GetProperty("hoveredItem", FLAGS);
            }
            return s_hoverItemProperty.GetValue(treeController, new object[] { }) as TreeViewItem;
        }

        static PropertyInfo s_isDraggingProperty;
        static MethodInfo s_getDropTargetControlIDMethod;
        static PropertyInfo s_draggingProperty;
        public static bool IsDragging(object treeController)
        {
            if (s_isDraggingProperty == null)
            {
                s_isDraggingProperty = TreeViewControllerType.GetProperty("isDragging", FLAGS);
            }
            if (s_draggingProperty == null)
            {
                s_draggingProperty = TreeViewControllerType.GetProperty("dragging", FLAGS);
            }
            if (s_getDropTargetControlIDMethod == null)
            {
                var tempDragging = s_draggingProperty.GetValue(treeController);
                if (tempDragging != null)
                {
                    s_getDropTargetControlIDMethod = tempDragging.GetType().GetMethod("GetDropTargetControlID", FLAGS);
                }
            }

            bool isDragging = (bool)s_isDraggingProperty.GetValue(treeController);
            var dragging = s_draggingProperty.GetValue(treeController);

            if (dragging == null)
            {
                return false;
            }

            int dropTargetId = (int)s_getDropTargetControlIDMethod.Invoke(dragging, new object[] { });

            return isDragging && dragging != null && dropTargetId == 0;
        }

        static MethodInfo s_getSceneHierarchiesMethod;

        public static List<SearchableEditorWindow> GetAllSceneHierarchyWindows()
        {
            if (s_getSceneHierarchiesMethod == null)
            {
                s_getSceneHierarchiesMethod = SceneHierarchyWindowType.GetMethod("GetAllSceneHierarchyWindows", BindingFlags.Public | BindingFlags.Static);
            }

            var list = (IList)s_getSceneHierarchiesMethod.Invoke(null, null);

            List<SearchableEditorWindow> windows = new List<SearchableEditorWindow>();

            foreach (var window in list)
            {
                windows.Add(window as SearchableEditorWindow);
            }

            return windows;
        }

        static PropertyInfo s_hasSearchFilterInfo;

        public static bool HasSearchFilter(SearchableEditorWindow searchableEditorWindow)
        {
            if (s_hasSearchFilterInfo == null)
            {
                s_hasSearchFilterInfo = typeof(SearchableEditorWindow).GetProperty("hasSearchFilter", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            bool? hasSearchFilter = s_hasSearchFilterInfo.GetValue(searchableEditorWindow) as bool?;

            return hasSearchFilter == true;
        }

        static PropertyInfo s_hasSearchFilterFocusInfo;
        public static bool HasSearchFilterFocus(SearchableEditorWindow searchableEditorWindow)
        {
            if (s_hasSearchFilterFocusInfo == null)
            {
                s_hasSearchFilterFocusInfo = typeof(SearchableEditorWindow).GetProperty("hasSearchFilterFocus", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            bool? hasSearchFilterFocus = s_hasSearchFilterFocusInfo.GetValue(searchableEditorWindow) as bool?;

            return hasSearchFilterFocus == true;
        }
    }
}