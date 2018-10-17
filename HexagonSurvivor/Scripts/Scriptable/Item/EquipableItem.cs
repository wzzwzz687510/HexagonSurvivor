using Sirenix.OdinInspector;

namespace HexagonUtils
{
    public abstract class EquipableItem : ScriptableItem
    {
        [BoxGroup(STATS_BOX_GROUP)]
        public float Durability;

        [VerticalGroup(LEFT_VERTICAL_GROUP + "/Modifiers")]
        public StatList Modifiers;
    }
}