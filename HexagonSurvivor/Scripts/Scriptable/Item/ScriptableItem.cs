namespace HexagonUtils
{
    using Sirenix.OdinInspector;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public abstract class ScriptableItem : ScriptableObject
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

        [BoxGroup("Split/Right/Description")]
        [HideLabel, TextArea(4, 14)]
        public string Description;

        [HorizontalGroup("Split", 0.5f, MarginLeft = 5, LabelWidth = 130)]
        [BoxGroup("Split/Right/Notes")]
        [HideLabel, TextArea(4, 9)]
        public string Notes;

        [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
        [ValueDropdown("SupportedItemTypes")]
        [ValidateInput("IsSupportedType")]
        public ItemTypes Type;

        [VerticalGroup("Split/Right")]
        public StatList requirements;

        [BoxGroup(STATS_BOX_GROUP)]
        public int maxStackSize = 1;

        [BoxGroup(STATS_BOX_GROUP),Range(0,1)]
        public float itemRarity;

        [BoxGroup(STATS_BOX_GROUP)]
        public long buyPrice;

        [BoxGroup(STATS_BOX_GROUP)]
        public long sellPrice;

        [BoxGroup(STATS_BOX_GROUP)]
        public long itemMallPrice;

        [BoxGroup(STATS_BOX_GROUP)]
        public bool sellable;

        [BoxGroup(STATS_BOX_GROUP)]
        public bool tradable;

        [BoxGroup(STATS_BOX_GROUP)]
        public bool destroyable;

        public abstract ItemTypes[] SupportedItemTypes { get; }

        private bool IsSupportedType(ItemTypes type)
        {
            return this.SupportedItemTypes.Contains(type);
        }

        // caching /////////////////////////////////////////////////////////////////
        // we can only use Resources.Load in the main thread. we can't use it when
        // declaring static variables. so we have to use it as soon as 'dict' is
        // accessed for the first time from the main thread.
        // -> we save the hash so the dynamic item part doesn't have to contain and
        //    sync the whole name over the network
        static Dictionary<int, ScriptableItem> cache;
        public static Dictionary<int, ScriptableItem> dict
        {
            get
            {
                // load if not loaded yet
                return cache ?? (cache = Resources.LoadAll<ScriptableItem>("").ToDictionary(
                    item => item.name.GetStableHashCode(), item => item)
                );
            }
        }
    }
}
