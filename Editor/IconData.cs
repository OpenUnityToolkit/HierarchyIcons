using UnityEngine;

namespace OpenToolkit.HierarchyIcons
{
    public class IconData
    {
        public Color ColorOverride { get; set; }
        public bool HasColorOverride { get; set; }
        public Texture2D IconUncolored { get; set; }

        public Texture2D Icon { get; set; }
        public Texture2D IconExpanded { get; set; }

        public Texture2D IconOverlay { get; set; }

        public Texture2D PrefabIcon { get; set; }

        public Component Component { get; set; }

        public Component[] Components { get; set; }

        public GameObject GameObject { get; set; }

        public Mesh Mesh { get; set; }

        public bool HideIconWhenPrefab { get; set; }

        public bool AllowOverride { get; set; } = true;
    }
}