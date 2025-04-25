using UnityEditor;

using UnityEngine;

namespace OpenToolkit.HierarchyIcons.Settings
{
    public abstract class Setting
    {
        public string Key { get; set; }
        public string Label { get; set; }

        public string Tooltip { get; set; }

        public string Category { get; set; }

        public abstract void Draw();
        public abstract void Load();
        public abstract void Save();
    }

    public class SettingBool : Setting
    {
        public bool Value => _value;

        bool _value;

        public SettingBool(string key, string label, bool defaultValue = true)
        {
            Key = key;
            Label = label;
            _value = defaultValue;

            Load();
        }

        public override void Draw()
        {
            var content = new GUIContent(Label, Tooltip);
            _value = EditorGUILayout.Toggle(content, _value);
        }

        public override void Load()
        {
            _value = EditorPrefs.GetBool(Key, _value);
        }

        public override void Save()
        {
            EditorPrefs.SetBool(Key, _value);
        }
    }
}