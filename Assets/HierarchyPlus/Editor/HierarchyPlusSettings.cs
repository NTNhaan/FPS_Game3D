using UnityEditor;
using UnityEngine;

namespace HierarchyPlus.Editor
{
    public class HierarchyPlusSettings : ScriptableObject
    {
        public bool enabled = true;

        [Header("Layout")]
        public float rightPadding = 6f;
        public float iconSize = 16f;
        public float toggleSize = 16f;
        public float iconGap = 2f;

        [Header("Behavior")]
        public bool drawActiveToggle = true;
        public bool drawComponentIcon = true;
        public bool drawHeaderRuleByName = true;
        public string headerPrefix = "---";

        [Header("Colors")]
        public Color headerBackground = new Color(0.18f, 0.18f, 0.18f, 1f);
        public Color headerText = Color.white;
        public Color inactiveTint = new Color(1f, 1f, 1f, 0.45f);

        [Header("Leading Icon")]
        public bool drawLeadingIcon = true;
        public float leadingIconSize = 18f;
        public float leadingIconPadding = 4f;
        
        [Header("Hierarchy Lines")]
        public bool drawHierarchyLines = true;
        public Color hierarchyLineColor = new Color(1f, 1f, 1f, 0.15f);
        
        internal const string AssetPath = "Assets/HierarchyPlus/Editor/HierarchyPlusSettings.asset";

        public static HierarchyPlusSettings GetOrCreate()
        {
            var s = AssetDatabase.LoadAssetAtPath<HierarchyPlusSettings>(AssetPath);
            if (s != null) return s;

            s = CreateInstance<HierarchyPlusSettings>();
            var dir = System.IO.Path.GetDirectoryName(AssetPath);
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            AssetDatabase.CreateAsset(s, AssetPath);
            AssetDatabase.SaveAssets();
            return s;
        }
    }

    public static class HierarchyPlusSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider Create()
        {
            return new SettingsProvider("Project/HierarchyPlus", SettingsScope.Project)
            {
                label = "HierarchyPlus",
                guiHandler = _ =>
                {
                    var s = HierarchyPlusSettings.GetOrCreate();

                    EditorGUILayout.Space(4);
                    s.enabled = EditorGUILayout.Toggle("Enabled", s.enabled);

                    EditorGUILayout.Space(8);
                    EditorGUILayout.LabelField("Layout", EditorStyles.boldLabel);
                    s.rightPadding = EditorGUILayout.Slider("Right Padding", s.rightPadding, 0, 20);
                    s.iconSize = EditorGUILayout.Slider("Icon Size", s.iconSize, 12, 20);
                    s.toggleSize = EditorGUILayout.Slider("Toggle Size", s.toggleSize, 12, 20);
                    s.iconGap = EditorGUILayout.Slider("Icon Gap", s.iconGap, 0, 6);

                    EditorGUILayout.Space(8);
                    EditorGUILayout.LabelField("Behavior", EditorStyles.boldLabel);
                    s.drawActiveToggle = EditorGUILayout.Toggle("Draw Active Toggle", s.drawActiveToggle);
                    s.drawComponentIcon = EditorGUILayout.Toggle("Draw Icon", s.drawComponentIcon);
                    s.drawHeaderRuleByName = EditorGUILayout.Toggle("Header By Name Prefix", s.drawHeaderRuleByName);
                    s.headerPrefix = EditorGUILayout.TextField("Header Prefix", s.headerPrefix);

                    EditorGUILayout.Space(8);
                    EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel);
                    s.headerBackground = EditorGUILayout.ColorField("Header Background", s.headerBackground);
                    s.headerText = EditorGUILayout.ColorField("Header Text", s.headerText);
                    s.inactiveTint = EditorGUILayout.ColorField("Inactive Tint", s.inactiveTint);

                    if (GUI.changed)
                    {
                        EditorUtility.SetDirty(s);
                        EditorApplication.RepaintHierarchyWindow();
                    }
                }
            };
        }
    }
}
