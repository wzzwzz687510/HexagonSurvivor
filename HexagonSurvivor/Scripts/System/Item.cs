namespace HexagonUtils
{
    using System;
    using UnityEngine;

    [Serializable]
    public partial struct Item
    {
        // hashcode used to reference the real ScriptableItem (can't link to data
        // directly because synclist only supports simple types). and syncing a
        // string's hashcode instead of the string takes WAY less bandwidth.
        public int hash;

        // constructors
        public Item(ScriptableItem data)
        {
            hash = data.name.GetStableHashCode();
        }

        // wrappers for easier access
        public ScriptableItem data { get { return ScriptableItem.dict[hash]; } }
        public string name { get { return data.name; } }
        public int maxStackSize { get { return data.maxStackSize; } }
        public long buyPrice { get { return data.buyPrice; } }
        public long sellPrice { get { return data.sellPrice; } }
        public long itemMallPrice { get { return data.itemMallPrice; } }
        public bool sellable { get { return data.sellable; } }
        public bool tradable { get { return data.tradable; } }
        public bool destroyable { get { return data.destroyable; } }
        public Sprite Icon { get { return data.Icon; } }
    }
}