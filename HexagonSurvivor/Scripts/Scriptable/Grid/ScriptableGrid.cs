﻿namespace HexagonSurvivor
{
    using Sirenix.OdinInspector;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public abstract class ScriptableGrid : ScriptableObject
    {
        protected const string LEFT_VERTICAL_GROUP = "Split/Left";
        protected const string STATS_BOX_GROUP = "Split/Left/Stats";
        protected const string GENERAL_SETTINGS_VERTICAL_GROUP = "Split/Left/General Settings/Split/Right";

        [HideLabel, PreviewField(55)]
        [VerticalGroup(LEFT_VERTICAL_GROUP)]
        [HorizontalGroup(LEFT_VERTICAL_GROUP + "/General Settings/Split", 55, LabelWidth = 67)]
        public Texture Icon;

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

        [VerticalGroup("Split/Right")]
        public StatList Requirements;

        [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
        public GridType type;

        [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
        public Sprite[] images;

        [BoxGroup(STATS_BOX_GROUP)]
        public float GridRarity;

        static Dictionary<int, ScriptableGrid> cache;
        public static Dictionary<int, ScriptableGrid> dict
        {
            get
            {
                // load if not loaded yet
                return cache ?? (cache = Resources.LoadAll<ScriptableGrid>("").ToDictionary(
                    item => item.name.GetHashCode(), item => item)
                );
            }
        }
    }
}