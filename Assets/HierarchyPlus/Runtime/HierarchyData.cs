using UnityEngine;

namespace HierarchyPlus
{
    [DisallowMultipleComponent]
    public class HierarchyData : MonoBehaviour
    {
        [Header("Background")]
        public bool hasBackground;
        public Color background = new Color(0.2f, 0.6f, 0.9f, 0.18f);

        [Header("Header")]
        public bool isHeader;
        public bool boldHeader = true;

        [Header("Icon")]
        public bool showIcon = true;      // show icon at all
        public bool hasIcon;              // use custom icon name
        public string customIconName;     // Unity icon key (e.g. "Camera Icon")

        private void Reset()
        {
            hideFlags = HideFlags.HideInInspector;
        }
    }
}