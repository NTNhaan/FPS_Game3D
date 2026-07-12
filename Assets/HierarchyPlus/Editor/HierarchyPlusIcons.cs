using UnityEditor;
using UnityEngine;

namespace HierarchyPlus.Editor
{
    internal static class HierarchyPlusIcons
    {
        public static GUIContent Safe(string iconName)
        {
            try
            {
                var c = EditorGUIUtility.IconContent(iconName);
                return (c != null && c.image != null) ? c : GUIContent.none;
            }
            catch
            {
                return GUIContent.none;
            }
        }
        public static GUIContent GetBestIcon(GameObject go, string customIconName, bool allowAutoComponentIcon)
        {
            // 1) Custom icon by name
            if (!string.IsNullOrEmpty(customIconName))
            {
                var gc = SafeIconContent(customIconName);
                if (gc != null && gc.image != null) return gc;
            }

            if (!allowAutoComponentIcon) return GUIContent.none;

            // 2) Auto component icons
            if (go.GetComponent<Camera>()) return SafeIconContent("Camera Icon");
            if (go.GetComponent<AudioSource>()) return SafeIconContent("AudioSource Icon");

            // 3) Prefab-ish
            if (PrefabUtility.GetPrefabAssetType(go) != PrefabAssetType.NotAPrefab)
                return SafeIconContent("Prefab Icon");

            // 4) Default
            return SafeIconContent("d_CubeAsset Icon");
        }

        private static GUIContent SafeIconContent(string iconName)
        {
            try { return EditorGUIUtility.IconContent(iconName); }
            catch { return GUIContent.none; }
        }
    }
}