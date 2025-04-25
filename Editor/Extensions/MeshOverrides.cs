using OpenToolkit.HierarchyIcons.Settings;
using OpenToolkit.HierarchyIcons.Utility;

using UnityEditor;
using UnityEngine;

namespace OpenToolkit.HierarchyIcons.Extensions
{
    public static class MeshOverrides
    {
        const string KEY = "MeshOverrides.config";
        public static bool ShowPrimitivesIcon => s_showPrimitivesIcon.Value;
        private static SettingBool s_showPrimitivesIcon = new SettingBool($"{KEY}.showPrimitivesIcon", "Show indicative icons for meshes")
        {
            Category = "Meshes"
        };
        public static bool ShowColliderShape => s_showColliderShape.Value;
        private static SettingBool s_showColliderShape = new SettingBool($"{KEY}.config.showColliderShape", "Show colliders shapes as icons")
        {
            Category = "Meshes"
        };

        public static readonly Color MeshColor = new Color32(168, 176, 235, 255);
        public static readonly Color TriggerColor = new Color32(247, 162, 80, 255);
        public static readonly Color ColliderColor = new Color32(177, 255, 96, 255);

        [InitializeOnLoadMethod]
        public static void Init()
        {
            DoSubscriptions();

            HierarchyIconsSettings.OnSettingsChange += DoSubscriptions;

            HierarchyIconsSettings.Add(s_showPrimitivesIcon);
            HierarchyIconsSettings.Add(s_showColliderShape);
        }

        static void DoSubscriptions()
        {
            HierarchyIcons.OnCreateIconData -= IconDataCreated;

            if (ShowPrimitivesIcon || ShowColliderShape)
            {
                HierarchyIcons.OnCreateIconData += IconDataCreated;
            }
        }

        static void IconDataCreated(IconData iconData)
        {
            if (!iconData.AllowOverride)
            {
                return;
            }

            Component component = iconData.Component;

            if (ShowPrimitivesIcon && component is MeshFilter meshFilter)
            {
                Texture2D texture = FindPrimitiveIcons(meshFilter);
                if (texture != null)
                {
                    iconData.Icon = texture;
                    iconData.HasColorOverride = true;
                    iconData.ColorOverride = MeshColor;

                    return;
                }
            }

            if (ShowColliderShape && component is Collider collider)
            {
                Texture2D texture = GetColliderIcon(collider);
                if (texture != null)
                {
                    iconData.Icon = texture;
                    iconData.HasColorOverride = true;
                    iconData.ColorOverride = collider.isTrigger ? TriggerColor : ColliderColor;
                }
            }
        }

        static Texture2D FindPrimitiveIcons(MeshFilter meshFilter)
        {
            string meshIdentifier = MeshIdentifier(meshFilter.sharedMesh, meshFilter.gameObject);
            if (!string.IsNullOrEmpty(meshIdentifier))
            {
                Texture2D texture = IconUtil.LoadAsset(meshIdentifier);
                if (texture != null)
                {
                    return texture;
                }
            }

            if (meshFilter.sharedMesh.vertexCount > 5000)
            {
                return IconUtil.LoadAsset("Mesh/mesh");
            }
            else if (meshFilter.sharedMesh.vertexCount > 1000)
            {
                return IconUtil.LoadAsset("Mesh/mesh_mid");
            }
            else
            {
                return IconUtil.LoadAsset("Mesh/mesh_low");
            }
        }

        static Texture2D GetColliderIcon(Collider collider)
        {
            string additive = collider.isTrigger ? "_wire" : string.Empty;

            if (collider is BoxCollider)
            {
                return IconUtil.LoadAsset("Mesh/cube" + additive);
            }

            if (collider is SphereCollider)
            {
                return IconUtil.LoadAsset("Mesh/sphere" + additive);
            }

            if (collider is CapsuleCollider)
            {
                return IconUtil.LoadAsset("Mesh/capsule" + additive);
            }

            if (collider is MeshCollider meshCollider)
            {
                Mesh mesh = meshCollider.sharedMesh;
                if (mesh == null)
                {
                    return null;
                }

                string meshIdentifier = MeshIdentifier(mesh, collider.gameObject);
                if (!string.IsNullOrEmpty(meshIdentifier))
                {
                    return IconUtil.LoadAsset(meshIdentifier + additive);
                }
                else if (mesh.vertexCount > 1000)
                {
                    return IconUtil.LoadAsset("Mesh/mesh_high" + additive);
                }
                else if (mesh.vertexCount > 250)
                {
                    return IconUtil.LoadAsset("Mesh/mesh_mid" + additive);
                }
                else
                {
                    return IconUtil.LoadAsset("Mesh/mesh_low" + additive);
                }
            }

            return null;
        }

        static string MeshIdentifier(Mesh mesh, GameObject gameObject)
        {
            if (mesh == null)
            {
                return null;
            }

            if (TryParsePrimitive(mesh.name, out string primitive))
            {
                return primitive;
            }

            if (TryParsePrimitive(gameObject.name, out primitive))
            {
                return primitive;
            }

            // vertex count extrapolation
            if (mesh.vertexCount == 24)
            {
                return "cube";
            }

            if (mesh.vertexCount < 24)
            {
                return "quad";
            }

            return null;
        }

        private static bool TryParsePrimitive(string toParse, out string primitive)
        {
            toParse = toParse.ToUpperInvariant();

            if (toParse.Contains("CUBE"))
            {
                primitive = "cube";
                return true;
            }

            if (toParse.Contains("SPHERE"))
            {
                primitive = "sphere";
                return true;
            }

            if (toParse.Contains("PLANE"))
            {
                primitive = "plane";
                return true;
            }

            if (toParse.Contains("CYLINDER"))
            {
                primitive = "cylinder";
                return true;
            }

            if (toParse.Contains("CAPSULE"))
            {
                primitive = "capsule";
                return true;
            }

            primitive = string.Empty;
            return false;
        }
    }
}