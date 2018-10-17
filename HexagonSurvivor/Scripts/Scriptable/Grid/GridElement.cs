namespace HexagonUtils
{
    using System;
    using UnityEngine;

    [Serializable]
    public class GridElement
    {
        public int hash;
        System.Random r;

        public GridElement(ScriptableGrid data)
        {
            hash = data.name.GetStableHashCode();
            r = new System.Random(hash);
        }

        // wrappers for easier access
        public ScriptableGrid data { get { return ScriptableGrid.dict[hash]; } }
        public string name { get { return data.name; } }
        public Sprite image { get { return data.images[r.Next(0, data.images.Length)]; } }
        public float cost { get { return data.cost; } }
    }
}
