namespace HexagonSurvivor
{
    using UnityEngine;

    public class MaterialItem : ScriptableItem
    {
        public override ItemTypes[] SupportedItemTypes
        {
            get
            {
                return new ItemTypes[]
                {
                    ItemTypes.Material
                };
            }
        }
    }
}
