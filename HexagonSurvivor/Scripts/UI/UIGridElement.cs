using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGridElement : MonoBehaviour {

    public bool isSelected;
    private SpriteRenderer m_spriteRenderer;

    void Start()
    {
        m_spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void Resume(bool isSelect)
    {
        if(isSelect) isSelected = false;
        if (isSelected) return;
        m_spriteRenderer.enabled = false;
    }

    public void Highlight()
    {
        if (isSelected) return;

        m_spriteRenderer.color = Color.yellow;
        m_spriteRenderer.enabled = true;
    }

    public void Select()
    {
        m_spriteRenderer.color = Color.green;
        m_spriteRenderer.enabled = true;
        isSelected = true;
    }
}
