namespace HexagonUtils
{
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class HexagonSurvivorEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("Tools/HexagonSurvivor/Editor")]
        private static void Open()
        {
            var window = GetWindow<HexagonSurvivorEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;

            tree.AddAllAssetsAtPath("Characters", "Assets/HexagonSurvivor/Resources", typeof(ScriptableCharacter), true, true)
                .SortMenuItemsByName();

            // Create character overview.
            CharacterOverview.Instance.UpdateCharacterOverview();
            tree.Add("Characters", new CharacterTable(CharacterOverview.Instance.AllCharacters));

            // Add all scriptable object grids.
            tree.AddAllAssetsAtPath("", "Assets/HexagonSurvivor/Resources", typeof(ScriptableGrid), true)
                .SortMenuItemsByName();

            // Add all scriptable object items.
            tree.AddAllAssetsAtPath("", "Assets/HexagonSurvivor/Resources", typeof(ScriptableItem), true)
                .SortMenuItemsByName()
                .ForEach(this.AddDragHandles);

            // Add icons to grids.
            tree.EnumerateTree().AddIcons<ScriptableCharacter>(x => x.Icon);
            tree.EnumerateTree().AddIcons<ScriptableGrid>(x => x.Icon);
            tree.EnumerateTree().AddIcons<ScriptableItem>(x => x.Icon);

            // Add drag handles to grids, so they can be easily dragged into specific map etc...
            tree.EnumerateTree().Where(x => x.ObjectInstance as ScriptableItem)
                .ForEach(AddDragHandles);

            return tree;
        }

        private void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.ObjectInstance, false, false);
        }

        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Grid")))
                {
                    ScriptableObjectCreator.ShowDialog<ScriptableGrid>("Assets/HexagonSurvivor/Resources/Grids", obj =>
                    {
                        obj.Name = obj.name;
                        base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                    });
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Item")))
                {
                    ScriptableObjectCreator.ShowDialog<ScriptableItem>("Assets/HexagonSurvivor/Resources/Items", obj =>
                    {
                        obj.Name = obj.name;
                        base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                    });
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Character")))
                {
                    ScriptableObjectCreator.ShowDialog<ScriptableCharacter>("Assets/HexagonSurvivor/Resources/Character", obj =>
                    {
                        obj.Name = obj.name;
                        base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor
                    });
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}