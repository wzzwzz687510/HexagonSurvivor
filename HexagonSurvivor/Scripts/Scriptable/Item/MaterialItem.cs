namespace HexagonUtils
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
