using UnityEngine;

using UnityEditor;

using OpenToolkit.HierarchyIcons.Settings;
using OpenToolkit.HierarchyIcons.Utility;

namespace OpenToolkit.HierarchyIcons.Extensions
{
    public static class MeshOverrides
    {
        static readonly string KEY = $"{typeof(MeshOverrides).FullName}.config";
        public static bool ShowPrimitivesIcon => _showPrimitivesIcon.Value;
        private static SettingBool _showPrimitivesIcon = new SettingBool($"{KEY}.showPrimitivesIcon", "Show indicative icons for meshes")
        {
            Category = "Meshes"
        };
        public static bool ShowColliderShape => _showColliderShape.Value;
        private static SettingBool _showColliderShape = new SettingBool($"{KEY}.config.showColliderShape", "Show colliders shapes as icons")
        {
            Category = "Meshes"
        };

        public static readonly Color MESH_COLOR = new Color32(168, 176, 235, 255);
        public static readonly Color TRIGGER_COLOR = new Color32(247, 162, 80, 255);
        public static readonly Color COLLIDER_COLOR = new Color32(177, 255, 96, 255);

        [InitializeOnLoadMethod]
        public static void Init()
        {
            DoSubscriptions();

            HierarchyIconsSettings.OnSettingsChange += DoSubscriptions;

            HierarchyIconsSettings.Add(_showPrimitivesIcon);
            HierarchyIconsSettings.Add(_showColliderShape);
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
                    iconData.ColorOverride = MESH_COLOR;

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
                    iconData.ColorOverride = collider.isTrigger ? TRIGGER_COLOR : COLLIDER_COLOR;
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
            string additive = collider.isTrigger ? "_wire" : "";

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

            string meshName = mesh.name.ToLower();

            if (TryParsePrimitive(meshName, out string primitive))
            {
                return primitive;
            }

            string gameObjectName = gameObject.name.ToLower();

            if (TryParsePrimitive(gameObjectName, out primitive))
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

        private static bool TryParsePrimitive(string meshName, out string primitive)
        {
            if (meshName.Contains("cube"))
            {
                primitive = "cube";
                return true;
            }

            if (meshName.Contains("sphere"))
            {
                primitive = "sphere";
                return true;
            }

            if (meshName.Contains("plane"))
            {
                primitive = "plane";
                return true;
            }

            if (meshName.Contains("cylinder"))
            {
                primitive = "cylinder";
                return true;
            }

            if (meshName.Contains("capsule"))
            {
                primitive = "capsule";
                return true;
            }

            primitive = string.Empty;
            return false;
        }
    }
}