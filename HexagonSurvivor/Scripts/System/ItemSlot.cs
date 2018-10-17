namespace HexagonUtils
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct ItemSlot
    {
        public int amount;
        public Item item;

        // constructors
        public ItemSlot(Item item, int amount = 1)
        {
            this.item = item;
            this.amount = amount;
        }

        // helper functions to increase/decrease amount more easily
        // -> returns the amount that we were able to increase/decrease by
        public int DecreaseAmount(int reduceBy)
        {
            // as many as possible
            int limit = Mathf.Clamp(reduceBy, 0, amount);
            amount -= limit;
            return limit;
        }

        public int IncreaseAmount(int increaseBy)
        {
            // as many as possible
            int limit = Mathf.Clamp(increaseBy, 0, item.maxStackSize - amount);
            amount += limit;
            return limit;
        }
    }
}
