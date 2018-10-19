namespace HexagonUtils
{
    using UnityEngine;

    public class SpriteManager : MonoBehaviour
    {
        public Color selectColor;
        public Color highlightColor;
        public bool isSelected;
        public SpriteRenderer m_spriteRenderer;
        public SpriteRenderer selectSpriteRenderer;
        public GridEntity m_gridEntity;

        void Start()
        {
            if (!m_spriteRenderer)
            {
                m_spriteRenderer = GetComponent<SpriteRenderer>();
                Debug.Log("[Sprite Manager]Please set the target of m_spriteRenderer.");
            }

            if (!selectSpriteRenderer)
            {
                selectSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
                Debug.Log("[Sprite Manager]Please set the target of selectSpriteRenderer.");
            }

            if (!m_gridEntity)
            {
                m_gridEntity = GetComponent<GridEntity>();
                Debug.Log("[Sprite Manager]Please set the target of m_gridEntity.");
            }
        }

        public void Resume(bool isSelect)
        {
            if (isSelect) isSelected = false;
            if (isSelected) return;
            selectSpriteRenderer.enabled = false;
        }

        public void Highlight()
        {
            if (isSelected) return;
            selectSpriteRenderer.color = highlightColor;
            selectSpriteRenderer.enabled = true;
        }

        public void Select()
        {
            selectSpriteRenderer.color = selectColor;
            selectSpriteRenderer.enabled = true;
            isSelected = true;
        }

        public void SetImage(Sprite image)
        {
            m_spriteRenderer.sprite = image;
        }
    }
}