namespace HexagonSurvivor
{
    using UnityEngine;

    public class SpriteManager : MonoBehaviour
    {

        public bool isSelected;
        public SpriteRenderer m_spriteRenderer;
        public SpriteRenderer selectSpriteRenderer;

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

            selectSpriteRenderer.color = Color.yellow;
            selectSpriteRenderer.enabled = true;
        }

        public void Select()
        {
            selectSpriteRenderer.color = Color.green;
            selectSpriteRenderer.enabled = true;
            isSelected = true;
        }

        public void SetImage(Sprite image)
        {
            m_spriteRenderer.sprite = image;
        }
    }
}