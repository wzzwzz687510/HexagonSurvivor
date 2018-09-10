namespace HexagonSurvivor
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class ScriptableCharacter : SerializedScriptableObject
    {
        [HorizontalGroup("Split", 55, LabelWidth = 70)]
        [HideLabel, PreviewField(55, ObjectFieldAlignment.Left)]
        public Texture Icon;

        [VerticalGroup("Split/Meta")]
        public string Name;

        [VerticalGroup("Split/Meta"), Range(0, 100)]
        public int Level;

        [TabGroup("Starting Inventory")]
        public ItemSlot[,] Inventory = new ItemSlot[10, 4];

        [TabGroup("Starting Stats"), HideLabel]
        public CharacterStats Skills = new CharacterStats();

        [HideLabel]
        [TabGroup("Starting Equipment")]
        public CharacterEquipment StartingEquipment;
    }
}