namespace HexagonUtils
{
    using Sirenix.OdinInspector;
    using System.Collections.Generic;
    using UnityEngine;

    public class ResourceGrid : ScriptableGrid
    {
        [BoxGroup(STATS_BOX_GROUP)]
        [ValueDropdown("SupportedItemTypes")]
        public ItemTypes[] needItemTypes;

        [BoxGroup(STATS_BOX_GROUP)]
        public bool isAnd;

        [BoxGroup(STATS_BOX_GROUP)]
        public Dictionary<ScriptableItem, int> item = new Dictionary<ScriptableItem, int>();

        public override GridType[] SupportedGridTypes
        {
            get
            {
                return new GridType[]
                {
                    GridType.Resource
                };
            }
        }

        public ItemTypes[] SupportedItemTypes
        {
            get
            {
                return new ItemTypes[]
                {
                    ItemTypes.Axe
                };
            }
        }
    }
}