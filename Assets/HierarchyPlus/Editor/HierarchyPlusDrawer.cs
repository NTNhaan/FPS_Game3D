using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HierarchyPlus;

namespace HierarchyPlus.Editor
{
    [InitializeOnLoad]
    public static class HierarchyPlusDrawer
    {
        private static readonly Dictionary<int, HierarchyData> _cache = new();
        private static HierarchyPlusSettings _settings;

        // Unity-like constants
        private const float ROW_HEIGHT = 18f;
        private const float INDENT_WIDTH = 14f;
        private const float FOLDOUT_WIDTH = 14f;
        private const float LINE_OFFSET = 7f;

        static HierarchyPlusDrawer()
        {
            _settings = HierarchyPlusSettings.GetOrCreate();
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
            EditorApplication.hierarchyChanged += ClearCache;
        }

        private static void ClearCache()
        {
            _cache.Clear();
            _settings = HierarchyPlusSettings.GetOrCreate();
            EditorApplication.RepaintHierarchyWindow();
        }

        private static void OnHierarchyGUI(int id, Rect rect)
        {
            if (!_settings.enabled) return;

            var go = EditorUtility.InstanceIDToObject(id) as GameObject;
            if (!go) return;

            var data = GetData(go);

            if (_settings.drawHierarchyLines)
                DrawUnityStyleTree(go, rect);
        }

        // =====================================================
        // UNITY-STYLE TREE (MATCH IMAGE 2)
        // =====================================================
        private static void DrawUnityStyleTree(GameObject go, Rect rect)
        {
            Transform t = go.transform;
            if (!t.parent) return;

            float midY = rect.y + ROW_HEIGHT * 0.5f;

            int depth = GetDepth(t);

            Handles.color = _settings.hierarchyLineColor;

            // ===== 1. Draw ancestor vertical columns =====
            Transform ancestor = t.parent;
            int level = depth - 1;

            while (ancestor != null && ancestor.parent != null)
            {
                bool ancestorHasNextSibling =
                    ancestor.GetSiblingIndex() < ancestor.parent.childCount - 1;

                if (ancestorHasNextSibling)
                {
                    float x = rect.x - FOLDOUT_WIDTH - LINE_OFFSET - (depth - level) * INDENT_WIDTH;
                    Handles.DrawLine(
                        new Vector3(x, rect.y),
                        new Vector3(x, rect.yMax)
                    );
                }

                ancestor = ancestor.parent;
                level--;
            }

            // ===== 2. Draw vertical for this level (STOP at midY) =====
            float baseX =
                rect.x - FOLDOUT_WIDTH - LINE_OFFSET;

            Handles.DrawLine(
                new Vector3(baseX, rect.y),
                new Vector3(baseX, midY)
            );

            // ===== 3. Draw horizontal (ALWAYS) =====
            bool hasChildren = t.childCount > 0;

            float endX = hasChildren
                ? rect.x - FOLDOUT_WIDTH - 4f   // short (has foldout)
                : rect.x - 2f;                  // long (leaf)

            Handles.DrawLine(
                new Vector3(baseX, midY),
                new Vector3(endX, midY)
            );
        }

        // =====================================================
        // UTILS
        // =====================================================
        private static int GetDepth(Transform t)
        {
            int d = 0;
            while (t.parent != null)
            {
                d++;
                t = t.parent;
            }
            return d;
        }

        private static HierarchyData GetData(GameObject go)
        {
            int id = go.GetInstanceID();
            if (_cache.TryGetValue(id, out var d)) return d;
            d = go.GetComponent<HierarchyData>();
            _cache[id] = d;
            return d;
        }
    }
}
