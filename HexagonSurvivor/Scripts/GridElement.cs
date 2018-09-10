namespace HexagonSurvivor
{
    using System;
    using UnityEngine;

    [Serializable]
    public class GridElement
    {
        public int hash;
        private int imageId;
        System.Random r;

        public GridElement(ScriptableGrid data)
        {
            hash = data.name.GetHashCode();
            r = new System.Random(hash);
        }

        // wrappers for easier access
        public ScriptableGrid data { get { return ScriptableGrid.dict[hash]; } }
        public string name { get { return data.name; } }
        public Sprite image { get { return data.images[r.Next(0, data.images.Length)]; } }
    }
}