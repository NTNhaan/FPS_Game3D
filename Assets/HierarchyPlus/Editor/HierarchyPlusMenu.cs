using UnityEditor;
using UnityEngine;
using HierarchyPlus;

namespace HierarchyPlus.Editor
{
    public static class HierarchyPlusMenu
    {
        [MenuItem("GameObject/HierarchyPlus/Style Picker", false, 0)]
        public static void OpenStylePicker()
        {
            HierarchyPlusStylePickerWindow.Open();
        }

        [MenuItem("GameObject/HierarchyPlus/Set As Header", false, 1)]
        public static void SetAsHeader()
        {
            foreach (var go in Selection.gameObjects)
            {
                var data = EnsureData(go);
                Undo.RecordObject(data, "Set Header");
                data.isHeader = true;
                data.hasBackground = false; // header uses settings color
                EditorUtility.SetDirty(data);
            }
            EditorApplication.RepaintHierarchyWindow();
        }

        [MenuItem("GameObject/HierarchyPlus/Clear Style", false, 2)]
        public static void ClearStyle()
        {
            foreach (var go in Selection.gameObjects)
            {
                var data = go.GetComponent<HierarchyData>();
                if (!data) continue;

                Undo.RecordObject(go, "Clear Hierarchy Style");
                Undo.DestroyObjectImmediate(data);
            }
            EditorApplication.RepaintHierarchyWindow();
        }

        [MenuItem("GameObject/HierarchyPlus/Toggle Icon", false, 3)]
        public static void ToggleIcon()
        {
            foreach (var go in Selection.gameObjects)
            {
                var data = EnsureData(go);
                Undo.RecordObject(data, "Toggle Hierarchy Icon");
                data.showIcon = !data.showIcon;
                EditorUtility.SetDirty(data);
            }
            EditorApplication.RepaintHierarchyWindow();
        }

        private static HierarchyData EnsureData(GameObject go)
        {
            var data = go.GetComponent<HierarchyData>();
            if (data) return data;

            data = Undo.AddComponent<HierarchyData>(go);
            data.hideFlags = HideFlags.HideInInspector;
            EditorUtility.SetDirty(go);
            return data;
        }
    }
}
