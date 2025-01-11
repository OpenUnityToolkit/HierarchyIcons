using UnityEngine;

namespace OpenToolkit.HierarchyIcons
{
    public class IconData
    {
        public Color ColorOverride;
        public bool HasColorOverride;
        public Texture2D IconUncolored;

        public Texture2D Icon;
        public Texture2D IconExpanded;

        public Texture2D IconOverlay;

        public Texture2D PrefabIcon;

        public Component Component;

        public Component[] Components;

        public GameObject GameObject;

        public Mesh Mesh;

        public bool HideIconWhenPrefab;

        public IconData() { }

    }
}