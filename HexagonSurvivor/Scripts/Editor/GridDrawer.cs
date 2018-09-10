namespace HexagonSurvivor
{
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    [OdinDrawer]
    public class GridDrawer<TGrid> : OdinValueDrawer<TGrid>
        where TGrid : ScriptableGrid
    {
        protected override void DrawPropertyLayout(IPropertyValueEntry<TGrid> entry, GUIContent label)
        {
            var rect = EditorGUILayout.GetControlRect(label != null, 45);

            if (label != null)
            {
                rect.xMin = EditorGUI.PrefixLabel(rect.AlignCenterY(15), label).xMin;
            }
            else
            {
                rect = EditorGUI.IndentedRect(rect);
            }

            ScriptableGrid grid = entry.SmartValue;
            Texture texture = null;

            if (grid)
            {
                texture = GUIHelper.GetAssetThumbnail(grid.Icon, typeof(TGrid), true);
                GUI.Label(rect.AddXMin(50).AlignMiddle(16), EditorGUI.showMixedValue ? "-" : grid.Name);
            }

            entry.WeakSmartValue = SirenixEditorFields.UnityPreviewObjectField(rect.AlignLeft(45), grid, texture, entry.BaseValueType);
        }
    }
}
