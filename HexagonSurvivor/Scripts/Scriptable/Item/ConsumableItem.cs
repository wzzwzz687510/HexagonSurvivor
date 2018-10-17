namespace HexagonUtils
{
    using Sirenix.OdinInspector;

    public class ConsumableItem : ScriptableItem
    {
        [SuffixLabel("seconds ", true)]
        [BoxGroup(STATS_BOX_GROUP)]
        public float Cooldown;

        [HorizontalGroup(STATS_BOX_GROUP + "/Dur")]
        public bool ConsumeOverTime;

        [HideLabel]
        [HorizontalGroup(STATS_BOX_GROUP + "/Dur")]
        [SuffixLabel("seconds ", true), EnableIf("ConsumeOverTime")]
        [LabelWidth(20)]
        public float Duration;

        [VerticalGroup(LEFT_VERTICAL_GROUP)]
        public StatList Modifiers;

        public override ItemTypes[] SupportedItemTypes
        {
            get
            {
                return new ItemTypes[]
                {
                    ItemTypes.Consumable,
                    ItemTypes.Flask
                };
            }
        }
    }
}