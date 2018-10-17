namespace HexagonUtils
{
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector.Editor.Drawers;
    using Sirenix.Utilities.Editor;
    using Sirenix.Utilities;
    using UnityEditor;
    using Sirenix.OdinInspector;

    // 
    // In Character.cs we have a two dimention array of ItemSlots which is our inventory.
    // And instead of using the the TableMatrix attribute to customize it there, we in this case 
    // instead create a custom drawer that will work for all two-dimentional ItemSlotData arrays,
    // so we don't have to make the same CustomDrawer via the TableMatrix attribute again and again.
    // 

    [OdinDrawer]
    internal sealed class ItemSlotCellDrawer<TArray> : TwoDimensionalArrayDrawer<TArray, ItemSlotData>
        where TArray : System.Collections.IList
    {
        protected override TableMatrixAttribute GetDefaultTableMatrixAttributeSettings()
        {
            return new TableMatrixAttribute()
            {
                SquareCells = true,
                HideColumnIndices = true,
                HideRowIndices = true,
                ResizableColumns = false
            };
        }

        protected override ItemSlotData DrawElement(Rect rect, ItemSlotData value)
        {
            var id = DragAndDropUtilities.GetDragAndDropId(rect);
            DragAndDropUtilities.DrawDropZone(rect, value.item ? value.item.Icon : null, null, id); // Draws the drop-zone using the items icon.

            if (value.item != null)
            {
                // Item count
                var countRect = rect.Padding(2).AlignBottom(16);
                value.amount = EditorGUI.IntField(countRect, Mathf.Max(1, value.amount));
                GUI.Label(countRect, "/ " + value.item.maxStackSize, SirenixGUIStyles.RightAlignedGreyMiniLabel);
            }

            value = DragAndDropUtilities.DropZone(rect, value);                                     // Drop zone for ItemSlotData structs.
            value.item = DragAndDropUtilities.DropZone<ScriptableItem>(rect, value.item);                     // Drop zone for Item types.
            value = DragAndDropUtilities.DragZone(rect, value, true, true);                         // Enables dragging of the ItemSlotData

            return value;
        }

        protected override void DrawPropertyLayout(IPropertyValueEntry<TArray> entry, GUIContent label)
        {
            base.DrawPropertyLayout(entry, label); // Draws the table-matrix for us.

            // Draws a drop-zone where we can destroy items.
            var rect = GUILayoutUtility.GetRect(0, 40).Padding(2);
            var id = DragAndDropUtilities.GetDragAndDropId(rect);
            DragAndDropUtilities.DrawDropZone(rect, null as UnityEngine.Object, null, id);
            DragAndDropUtilities.DropZone<ItemSlotData>(rect, new ItemSlotData(), false, id);
        }
    }
}