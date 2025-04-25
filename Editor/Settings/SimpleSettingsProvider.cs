using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OpenToolkit.HierarchyIcons.Settings
{
    class SimpleSettingsProvider : SettingsProvider
    {
        readonly List<Setting> _settings;

        readonly SettingBool _enabledSetting;

        public event Action OnSettingsChange;

        public SimpleSettingsProvider(string path, List<Setting> settings, SettingBool enabledSetting = null, SettingsScope scope = SettingsScope.User)
             : base(path, scope)
        {
            _settings = settings;
            _enabledSetting = enabledSetting;
        }

        public void Add(Setting setting)
        {
            _settings.Add(setting);

            _settings.Sort(SettingSort);
        }

        static int SettingSort(Setting a, Setting b)
        {
            string stA = a.Category + b.Label;
            string stB = b.Category + b.Label;
            return stA.CompareTo(stB);
        }

        public void SetEditorSettings()
        {
            _enabledSetting.Save();

            foreach (var setting in _settings)
            {
                setting.Save();
            }

            OnSettingsChange?.Invoke();
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            foreach (var setting in _settings)
            {
                setting.Load();
            }
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUI.BeginChangeCheck();

            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.margin = new RectOffset(7, 0, 7, 0);
            EditorGUILayout.BeginVertical(style, GUILayout.MaxWidth(220));

            EditorGUIUtility.labelWidth = 250;

            if (_enabledSetting != null)
            {
                _enabledSetting.Draw();
                EditorGUILayout.Space(12);
                EditorGUI.BeginDisabledGroup(!_enabledSetting.Value);
            }

            for (var i = 0; i < _settings.Count; i++)
            {
                var setting = _settings[i];
                if (i == 0)
                {
                    if (!string.IsNullOrEmpty(setting.Category))
                    {
                        EditorGUILayout.LabelField(setting.Category, EditorStyles.boldLabel);
                    }
                }
                else if (setting.Category != _settings[i - 1].Category)
                {
                    EditorGUILayout.Space(12);
                    EditorGUILayout.LabelField(setting.Category, EditorStyles.boldLabel);
                }

                setting.Draw();
            }

            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                SetEditorSettings();
            }

            if (_enabledSetting != null)
            {
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}