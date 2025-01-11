using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace OpenToolkit.HierarchyIcons.Utility
{
    public static class TreeViewUtil
    {
        public static void GetHierarchyWindowStatus(out bool isWindowFocused, out bool searching)
        {
            searching = false;
            bool searchFocus = false;

            if (EditorWindow.focusedWindow is SearchableEditorWindow searchable)
            {
                if (ReflectionHelper.HasSearchFilter(searchable))
                {
                    searching = true;
                }

                if (ReflectionHelper.HasSearchFilterFocus(searchable))
                {
                    searchFocus = true;
                }
            }

            isWindowFocused = EditorWindow.focusedWindow?.GetType().Name == "SceneHierarchyWindow";

            if (searchFocus)
            {
                isWindowFocused = false;
            }
        }

        public static void GetRowStatus(TreeViewItem treeItem, object treeController, out bool isSelected, out bool isHovering)
        {
            isSelected = ReflectionHelper.IsItemDragSelectedOrSelected(treeItem, treeController);
            isHovering = ReflectionHelper.GetHoverItem(treeController) == treeItem;
            if (ReflectionHelper.IsDragging(treeController))
            {
                isHovering = false;
            }

            bool isRenaming = ReflectionHelper.IsTreeRenaming(treeController);
            if (isRenaming)
            {
                isSelected = false;
            }
        }

        public static void GetItemFromWindows(int instanceID, List<SearchableEditorWindow> hierarchyWindows, out TreeViewItem treeViewItem, out object treeController)
        {
            treeViewItem = null;
            treeController = null;

            // if we are hovering over a window, set it as active
            foreach (var window in hierarchyWindows)
            {
                var controller = ReflectionHelper.GetTreeController(window);

                if (controller != null)
                {
                    treeViewItem = GetItem(instanceID, out int _, controller);

                    treeController = controller;

                    if (treeViewItem != null)
                    {
                        if (ReflectionHelper.GetHoverItem(controller) != null)
                        {
                            treeController = controller;
                        }

                        break;
                    }
                }
            }
        }

        static TreeViewItem GetItem(int instanceID, out int row, object treeController)
        {
            row = ReflectionHelper.GetRow(instanceID, treeController);

            if (row < 0)
            {
                return null;
            }
            if (row >= ReflectionHelper.GetRowCount(treeController))
            {
                return null;
            }

            var item = ReflectionHelper.GetItem(row, treeController);

            if (item != null)
            {
                return item;
            }

            return null;
        }
    }
}