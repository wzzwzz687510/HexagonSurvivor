namespace HexagonUtils
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class BiomeGrid : ScriptableGrid
    {
        [BoxGroup(STATS_BOX_GROUP)]
        public Sprite[] blockSprites;

        public override GridType[] SupportedGridTypes
        {
            get
            {
                return new GridType[]
                {
                    GridType.Biome
                };
            }
        }
    }
}
