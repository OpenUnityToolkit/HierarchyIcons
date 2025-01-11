using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.Reflection;

namespace OpenToolkit.HierarchyIcons.Utility
{
    public static class IconUtil
    {
        const string PACKAGE_NAME = "com.opentoolkit.hierarchyicons";
        const string FOLDER_NAME = "OpenToolkit/HierarchyIcons";
        const string ICON_FOLDER = "Editor Resources/Textures";

        static readonly string[] POTENTIAL_FOLDERS =
        {
            $"Packages/{PACKAGE_NAME}/{ICON_FOLDER}",
            $"Assets/Plugins/{FOLDER_NAME}/{ICON_FOLDER}",
            $"Assets/{FOLDER_NAME}/{ICON_FOLDER}",
        };

        static MethodInfo s_loadIconMethod;
        static MethodInfo s_getObjectIconMethod;

        static readonly BindingFlags FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        static Dictionary<string, Texture2D> s_cache = new Dictionary<string, Texture2D>();

        public static Texture2D LoadAsset(string path)
        {
            path = $"{path}.png";
            return FindTextureAsset(path, POTENTIAL_FOLDERS);
        }
        public static Texture2D FindTextureAsset(string path, string[] locations)
        {
            foreach (string location in locations)
            {
                string fullPath = $"{location}/{path}";

                if (s_cache.ContainsKey(path))
                {
                    return s_cache[path];
                }
                else
                {
                    Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(fullPath, typeof(Texture2D));
                    if (texture != null)
                    {
                        s_cache[path] = texture;
                        return texture;
                    }
                }
            }

            return null;
        }

        public static Texture2D LoadBuiltinTexture(string name)
        {
            if (s_loadIconMethod == null)
            {
                s_loadIconMethod = typeof(EditorGUIUtility).GetMethod("LoadIcon", FLAGS);
            }

            return s_loadIconMethod.Invoke(null, new object[] { name }) as Texture2D;
        }

        public static Texture2D LoadBuiltinIcon(string name)
        {
            return LoadBuiltinTexture(name + " Icon");
        }

        public static Texture2D GetIconForUnityObject(Object obj)
        {
            if (obj == null)
            {
                return null;
            }

            if (s_getObjectIconMethod == null)
            {
                s_getObjectIconMethod = typeof(EditorGUIUtility).GetMethod("GetIconForObject", FLAGS);
            }

            return s_getObjectIconMethod.Invoke(null, new object[] { obj }) as Texture2D;
        }

        public static Texture2D GetPrefabIcon(GameObject gameObject)
        {
            // prefab model
            if (PrefabUtility.IsAnyPrefabInstanceRoot(gameObject) && PrefabUtility.IsPartOfModelPrefab(gameObject))
            {
                return LoadAsset("Prefabs/PrefabModel");
            }

            // special icon for a prefab which contains an override
            else if (PrefabUtility.IsAnyPrefabInstanceRoot(gameObject) && PrefabUtility.HasPrefabInstanceAnyOverrides(gameObject, false) && !PrefabUtility.IsPartOfModelPrefab(gameObject))
            {
                return LoadAsset("Prefabs/PrefabOverride");
            }

            else if (IsPrefabOverride(gameObject))
            {
                return LoadAsset("Prefabs/PrefabOverride");
            }

            // is a prefab variant
            else if (PrefabUtility.IsAnyPrefabInstanceRoot(gameObject) && PrefabUtility.IsPartOfVariantPrefab(gameObject))
            {
                return LoadAsset("Prefabs/PrefabVariant");
            }

            // is a prefab - use original
            else if (PrefabUtility.IsAnyPrefabInstanceRoot(gameObject))
            {
                return LoadAsset("Prefabs/Prefab");
            }

            return null;
        }

        public static bool IsPrefabOverride(GameObject gameObject)
        {
            if (!PrefabUtility.IsAnyPrefabInstanceRoot(gameObject))
            {
                return false;
            }

            if (!PrefabUtility.HasPrefabInstanceAnyOverrides(gameObject, false))
            {
                return false;
            }

            var overrides = PrefabUtility.GetObjectOverrides(gameObject);

            foreach (var ovr in overrides)
            {
                if (gameObject == ovr.instanceObject)
                {
                    return true;
                }

                Component overrideComponent = ovr.instanceObject as Component;
                if (overrideComponent == null)
                {
                    continue;
                }
                if (overrideComponent.transform.IsChildOf(gameObject.transform))
                {
                    if (overrideComponent as Transform)
                    {
                        continue;
                    }

                    return true;
                }
            }

            return false;
        }
    }
}