namespace HexagonUtils
{
    using UnityEngine;

    public class GridEntity : MonoBehaviour
    {
        [HideInInspector]
        public GridElement gridElement;
        [HideInInspector]
        public HexCoordinate hex;
        public bool isBlocked;
        public SpriteManager m_spriteManager;

        void Start()
        {
            if (!m_spriteManager)
            {
                m_spriteManager = GetComponent<SpriteManager>();
                Debug.Log("[Grid Entity]Please set the target of m_spriteManager.");
            }
        }

        public void Init(GridElement gridElement,Vector2 position)
        {
            this.gridElement = gridElement;
            switch (gridElement.data.type)
            {
                case GridType.Biome:
                    System.Random r = new System.Random(position.ToString().GetStableHashCode());
                    if (r.Next(0, 100) > 5)
                    {
                        m_spriteManager.SetImage(gridElement.image);
                        break;
                    }
                    isBlocked = true;           
                    m_spriteManager.SetImage(((BiomeGrid)gridElement.data).
                        blockSprites[r.Next(0, ((BiomeGrid)gridElement.data).
                        blockSprites.Length)]);
                    break;
                case GridType.Normal:
                    m_spriteManager.SetImage(gridElement.image);
                    break;
                case GridType.Resource:
                    break;
                case GridType.Trigger:
                    break;
                default:
                    break;
            }
            this.hex = new HexCoordinate((int)position.x, (int)position.y);         
        }

    }
}