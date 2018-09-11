namespace HexagonSurvivor
{
    using UnityEngine;

    public class GridEntity : MonoBehaviour
    {
        [HideInInspector]
        public GridElement gridElement;
        [HideInInspector]
        public Vector2 position;
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
            this.position = position;
            m_spriteManager.SetImage(gridElement.image);
        }

    }
}