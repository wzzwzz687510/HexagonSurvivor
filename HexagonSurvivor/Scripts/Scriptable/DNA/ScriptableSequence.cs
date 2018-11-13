namespace HexagonUtils
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class ScriptableSequence : ScriptableObject
    {
        protected const string LEFT_VERTICAL_GROUP = "Split/Left";
        protected const string STATS_BOX_GROUP = "Split/Left/Stats";
        protected const string GENERAL_SETTINGS_VERTICAL_GROUP = "Split/Left/General Settings/Split/Right";

        [HideLabel, PreviewField(55)]
        [VerticalGroup(LEFT_VERTICAL_GROUP)]
        [HorizontalGroup(LEFT_VERTICAL_GROUP + "/General Settings/Split", 55, LabelWidth = 67)]
        public Sprite Icon;

        [BoxGroup(LEFT_VERTICAL_GROUP + "/General Settings")]
        [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
        public string Name;

        [BoxGroup(STATS_BOX_GROUP)]
        public SequenceType type;

        [BoxGroup(STATS_BOX_GROUP)]
        public bool isDominant;

        [ShowIf("type", SequenceType.Property)]
        [BoxGroup(STATS_BOX_GROUP)]
        public RaceProperty properties;

    }
}
