using UnityEngine;

using UnityEditor;

namespace OpenToolkit.HierarchyIcons.Utility
{
    public static class ComponentUtil
    {
        static string[] s_skippedComponents = new string[] { "CanvasRenderer", "MeshRenderer" };
        static string[] s_lowPrioComponents = new string[] { "CanvasGroup", "Mask" };

        public static IconData IconFromGameObject(GameObject gameObject)
        {
            IconData iconData;

            var components = gameObject.GetComponents<Component>();

            iconData = new IconData();
            iconData.GameObject = gameObject;
            iconData.Components = components;

            // a null component can occur if a script is removed
            if (HasNullComponent(components))
            {
                iconData.Icon = IconUtil.LoadBuiltinTexture("Warning");
            }

            // if a gameObject has an icon set
            if (iconData.Icon == null)
            {
                iconData.Icon = IconUtil.GetIconForUnityObject(gameObject);
            }

            if (iconData.Icon == null)
            {
                if (components.Length == 1)
                {
                    // Only a transform present
                    iconData.Icon = LoadComponentIcon(components[0]);
                    iconData.Component = components[0];
                }
                else
                {
                    iconData.Icon = GetIconFromComponents(components, out Component component);
                    iconData.Component = component;
                }
            }

            iconData.PrefabIcon = IconUtil.GetPrefabIcon(gameObject);

            if (TryGetMesh(components, out Mesh mesh))
            {
                iconData.Mesh = mesh;
            }

            return iconData;
        }

        static Texture2D GetIconFromComponents(Component[] components, out Component component)
        {
            Texture2D icon;
            // start at 1 to skip transforms
            for (int i = 1; i < components.Length; i++)
            {
                icon = GetIconFromComponent(components[i]);
                if (icon != null)
                {
                    component = components[i];
                    return icon;
                }
            }

            // no icon returned, check the lower priority components
            component = GetLowPriorityComponent(components);
            icon = LoadComponentIcon(component);

            // no icon found, use a script icon
            if (icon == null)
            {
                icon = IconUtil.LoadBuiltinIcon("cs Script");
                component = components[1];
            }

            return icon;
        }

        private static Texture2D GetIconFromComponent(Component component)
        {
            if (IsComponentInvalid(component))
            {
                return null;
            }

            if (IsSkippedComponent(component))
            {
                return null;
            }

            Texture2D componentIcon = LoadComponentIcon(component);
            if (componentIcon != null)
            {
                return componentIcon;
            }

            return null;
        }

        public static Texture2D LoadComponentIcon(Component component)
        {
            if (component == null)
            {
                return null;
            }

            // icon set in editor - ie a script icon
            Texture2D editorObjectIcon = IconUtil.GetIconForUnityObject(MonoScript.FromMonoBehaviour(component as MonoBehaviour));
            if (editorObjectIcon != null)
            {
                return editorObjectIcon;
            }

            // load as direct type name
            return IconUtil.LoadBuiltinIcon(component.GetType().Name);
        }

        static bool IsComponentInvalid(Component component)
        {
            // skip disabled components
            if (component is MonoBehaviour monoBehaviour && !monoBehaviour.enabled)
            {
                return true;
            }

            if (component is MeshFilter meshFilter)
            {
                MeshRenderer renderer = meshFilter.gameObject.GetComponent<MeshRenderer>();

                if (renderer == null)
                {
                    return true;
                }

                if (!renderer.enabled)
                {
                    return true;
                }

                if (meshFilter.sharedMesh == null)
                {
                    return true;
                }
            }

            return false;
        }

        static bool TryGetMesh(Component[] components, out Mesh mesh)
        {
            foreach (var component in components)
            {
                if (component is MeshFilter meshFilter && meshFilter.sharedMesh != null)
                {
                    mesh = meshFilter.sharedMesh;
                    return true;
                }

                if (component is MeshCollider meshCollider && meshCollider.sharedMesh != null)
                {
                    mesh = meshCollider.sharedMesh;
                    return true;
                }
            }

            mesh = null;
            return false;
        }

        static Component GetLowPriorityComponent(Component[] components)
        {
            foreach (var typeString in s_lowPrioComponents)
            {
                // start at 1 to skip transforms
                for (var i = 1; i < components.Length; i++)
                {
                    var component = components[i];
                    if (component.GetType().Name == typeString)
                    {
                        return component;
                    }
                }
            }

            return null;
        }

        static bool IsSkippedComponent(Component component)
        {
            foreach (var type in s_skippedComponents)
            {
                if (component.GetType().Name == type)
                {
                    return true;
                }
            }

            foreach (var type in s_lowPrioComponents)
            {
                if (component.GetType().Name == type)
                {
                    return true;
                }
            }

            return false;
        }

        static bool HasNullComponent(Component[] components)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}