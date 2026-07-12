using UnityEditor;
using UnityEngine;
using HierarchyPlus;

namespace HierarchyPlus.Editor
{
    public class HierarchyPlusStylePickerWindow : EditorWindow
    {
        private static readonly Color[] ColorPresets =
        {
            new Color(0.15f,0.15f,0.15f,0.25f),
            new Color(0.85f,0.25f,0.25f,0.30f),
            new Color(0.95f,0.75f,0.25f,0.30f),
            new Color(0.35f,0.85f,0.45f,0.30f),
            new Color(0.35f,0.55f,0.95f,0.30f),
            new Color(0.65f,0.45f,0.95f,0.30f),
            new Color(0.95f,0.55f,0.25f,0.30f),
            new Color(0.55f,0.35f,0.25f,0.30f),

            new Color(0.95f,0.95f,0.95f,0.25f),
            new Color(0.25f,0.75f,0.95f,0.30f),
            new Color(0.25f,0.95f,0.65f,0.30f),
            new Color(0.95f,0.85f,0.35f,0.30f),
            new Color(0.95f,0.55f,0.55f,0.30f),
            new Color(0.75f,0.45f,0.95f,0.30f),
            new Color(0.65f,0.55f,0.35f,0.30f),
            new Color(0.35f,0.35f,0.35f,0.30f),
        };

        private static readonly string[] IconPresets =
        {
            "Camera Icon",
            "Light Icon",
            "Canvas Icon",
            "EventSystem Icon",
            "RectTransform Icon",
            "Terrain Icon",
            "Prefab Icon",
            "GameObject Icon",

            "d_ScriptableObject Icon",
            "cs Script Icon",
            "d_Settings Icon",
            "d_UnityEditor.SceneHierarchyWindow",
            "Favorite Icon",
            "d_Folder Icon",
            "d_PlayButton",
            "d_PauseButton"
        };


        public static void Open()
        {
            var w = CreateInstance<HierarchyPlusStylePickerWindow>();
            w.titleContent = new GUIContent("Hierarchy Style");
            w.minSize = new Vector2(260, 220);
            w.maxSize = new Vector2(260, 220);

            // Open near mouse if possible, otherwise center
            Vector2 pos = GUIUtility.GUIToScreenPoint(Event.current != null
                ? Event.current.mousePosition
                : new Vector2(Screen.width / 2f, Screen.height / 2f));

            w.position = new Rect(pos.x - 130, pos.y - 110, 260, 220);
            w.ShowUtility();
        }

        private void OnGUI()
        {
            GUILayout.Space(6);
            DrawColorGrid();
            GUILayout.Space(10);
            DrawIconGrid();
            GUILayout.Space(10);
            DrawBottomButtons();
        }

        private void DrawColorGrid()
        {
            EditorGUILayout.LabelField("Color Presets", EditorStyles.boldLabel);

            const int columns = 8;
            const int size = 24;

            int col = 0;
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < ColorPresets.Length; i++)
            {
                var r = GUILayoutUtility.GetRect(size, size, GUILayout.Width(size), GUILayout.Height(size));
                EditorGUI.DrawRect(r, ColorPresets[i]);

                // border
                Handles.color = new Color(0, 0, 0, 0.25f);
                Handles.DrawAAPolyLine(1f,
                    new Vector3(r.x, r.y),
                    new Vector3(r.xMax, r.y),
                    new Vector3(r.xMax, r.yMax),
                    new Vector3(r.x, r.yMax),
                    new Vector3(r.x, r.y));

                if (GUI.Button(r, GUIContent.none, GUIStyle.none))
                    ApplyColor(ColorPresets[i]);

                col++;
                if (col >= columns)
                {
                    col = 0;
                    EditorGUILayout.EndHorizontal();
                    if (i != ColorPresets.Length - 1) EditorGUILayout.BeginHorizontal();
                }
            }
            if (col != 0) EditorGUILayout.EndHorizontal();
        }

        private void DrawIconGrid()
        {
            EditorGUILayout.LabelField("Icon Presets", EditorStyles.boldLabel);

            const int columns = 8;
            const int size = 24;

            int col = 0;
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < IconPresets.Length; i++)
            {
                var r = GUILayoutUtility.GetRect(size, size, GUILayout.Width(size), GUILayout.Height(size));
                var icon = HierarchyPlusIcons.Safe(IconPresets[i]);

                if (GUI.Button(r, icon))
                    ApplyIcon(IconPresets[i]);

                col++;
                if (col >= columns)
                {
                    col = 0;
                    EditorGUILayout.EndHorizontal();
                    if (i != IconPresets.Length - 1) EditorGUILayout.BeginHorizontal();
                }
            }
            if (col != 0) EditorGUILayout.EndHorizontal();
        }

        private void ApplyColor(Color c)
        {
            foreach (var go in Selection.gameObjects)
            {
                var data = EnsureData(go);
                Undo.RecordObject(data, "Set Hierarchy Color");
                data.hasBackground = true;
                data.background = c;
                EditorUtility.SetDirty(data);
            }
            EditorApplication.RepaintHierarchyWindow();
        }

        private void ApplyIcon(string iconName)
        {
            foreach (var go in Selection.gameObjects)
            {
                var data = EnsureData(go);
                Undo.RecordObject(data, "Set Hierarchy Icon");
                data.hasIcon = true;
                data.customIconName = iconName;
                EditorUtility.SetDirty(data);
            }
            EditorApplication.RepaintHierarchyWindow();
        }

        private void DrawBottomButtons()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Clear"))
            {
                foreach (var go in Selection.gameObjects)
                {
                    var data = go.GetComponent<HierarchyData>();
                    if (!data) continue;

                    Undo.RecordObject(go, "Clear Hierarchy Style");
                    Undo.DestroyObjectImmediate(data);
                }
                EditorApplication.RepaintHierarchyWindow();
                Close();
            }

            if (GUILayout.Button("Close"))
                Close();

            EditorGUILayout.EndHorizontal();
        }

        private static HierarchyData EnsureData(GameObject go)
        {
            var d = go.GetComponent<HierarchyData>();
            if (d) return d;

            d = Undo.AddComponent<HierarchyData>(go);
            d.hideFlags = HideFlags.HideInInspector;
            EditorUtility.SetDirty(go);
            return d;
        }
    }
}
