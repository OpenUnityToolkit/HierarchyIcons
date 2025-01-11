using UnityEngine;

using UnityEditor;

namespace OpenToolkit.HierarchyIcons.Utility
{
    public static class Colors
    {
        public static readonly Color Backing = new Color32(56, 56, 56, 255);
        public static readonly Color BackingAlt = new Color32(58, 58, 58, 255);

        public static readonly Color Highlight = new Color32(44, 93, 135, 255);
        public static readonly Color Hover = new Color32(69, 69, 69, 255);
        public static readonly Color Both = new Color32(44, 93, 135, 255);
        public static readonly Color UnfocusedSelected = new Color32(77, 77, 77, 255);


        public static readonly Color TreeActive = new Color32(104, 104, 104, 255);
        public static readonly Color TreeInactive = new Color32(104, 104, 104, 128);


        public static readonly Color PrefabTreeActive = new Color32(91, 115, 150, 255);
        public static readonly Color PrefabTreeInactive = new Color32(91, 115, 150, 128);
        public static readonly Color PrefabInactive = new Color32(67, 97, 148, 255);
        public static readonly Color Disabled = new Color32(127, 127, 127, 255);
        public static readonly Color Prefab = new Color32(125, 173, 243, 255);


        static Texture2D _overlayBackground;
        public static Texture2D OverlayBackground
        {
            get
            {
                if (_overlayBackground == null)
                {
                    _overlayBackground = IconUtil.LoadAsset("overlayBackground");
                }
                return _overlayBackground;
            }
        }

        static Texture2D _prefabAddedOverlay;
        public static Texture2D PrefabAddedOverlay
        {
            get
            {
                if (_prefabAddedOverlay == null)
                {
                    _prefabAddedOverlay = EditorGUIUtility.FindTexture("PrefabOverlayAdded Icon");
                }
                return _prefabAddedOverlay;
            }
        }

        static Texture2D _prefabModifiedOverlay;
        public static Texture2D PrefabModifiedOverlay
        {
            get
            {
                if (_prefabModifiedOverlay == null)
                {
                    _prefabModifiedOverlay = EditorGUIUtility.FindTexture("PrefabOverlayModified Icon");
                }
                return _prefabModifiedOverlay;
            }
        }

        static Texture2D _prefabRootOverlay;
        public static Texture2D PrefabRootOverlay
        {
            get
            {
                if (_prefabRootOverlay == null)
                {
                    _prefabRootOverlay = IconUtil.LoadAsset("prefab overlay");
                }
                return _prefabRootOverlay;
            }
        }

        public static Color GetBackgroundColour(bool isFocused, bool isOdd, bool isSelected, bool isHovering)
        {
            Color backgroundColor = isOdd ? BackingAlt : Backing;

            if (isHovering)
            {
                backgroundColor = Hover;
            }

            if (isSelected)
            {
                backgroundColor = Highlight;
            }

            if (!isFocused && isSelected)
            {
                backgroundColor = UnfocusedSelected;
            }

            return backgroundColor;
        }
    }
}