using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class ArrowKey : MonoBehaviour
{
    Image image;
    [SerializeField]
    Color NormalColor = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField]
    Color PressedColor = new Color(1f, 1f, 1f, 0.8f);

    bool m_isPressed;
    Rect m_rect;
    
    public bool Pressed
    {
        set
        {
            m_isPressed = value;
        }
    }

    public bool Contains(Vector2 position)
    {
        return m_rect.Contains(position);
    }

    private void Awake()
    {
        image = GetComponent<Image>();
        var trans = GetComponent<RectTransform>();
        m_rect = trans.rect;
        m_rect.position += new Vector2(trans.position.x, trans.position.y);
        Debug.Log(m_rect);
    }

    private void Update()
    {
        image.color = m_isPressed ? PressedColor : NormalColor;
    }
}
