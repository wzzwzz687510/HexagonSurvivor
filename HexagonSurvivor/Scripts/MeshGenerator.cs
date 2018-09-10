namespace HexagonSurvivor
{
    using System.Collections.Generic;
    using UnityEngine;

    public class MeshGenerator : MonoBehaviour
    {
        public GameObject m_prefab;
        public Dictionary<int[,], GameObject> mapRenderDictionary = new Dictionary<int[,], GameObject>();

        private GameObject mapParent;

        public void GenerateMesh(List<MapGenerator.MapGrid> mapGrids)
        {
            Destroy(mapParent);
            mapParent = new GameObject("MapHolder");

            mapRenderDictionary.Clear();

            foreach (var mapGrid in mapGrids)
            {
                GameObject go = Instantiate(m_prefab, new Vector3((mapGrid.tileX + mapGrid.tileY % 2 * 0.5f) * 1.25f, mapGrid.tileY * 1.0875f), Quaternion.identity, mapParent.transform);
                go.GetComponent<SpriteRenderer>().sprite = mapGrid.gridElement.image;
                mapRenderDictionary.Add(new int[mapGrid.tileX, mapGrid.tileY], go);

            }
        }

    }
}