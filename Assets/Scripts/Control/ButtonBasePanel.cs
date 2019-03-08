using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class ButtonBasePanel : MonoBehaviour
{
    Image image;
    [SerializeField]
    Color NormalColor = new Color(1f, 1f, 1f, 0.2f);
    [SerializeField]
    Color PressedColor = new Color(1f, 1f, 1f, 0.8f);

    bool m_isPressed;
    
    public bool Pressed
    {
        get
        {
            return m_isPressed;
        }
        set
        {
            m_isPressed = value;
        }
    }

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        image.color = m_isPressed ? PressedColor : NormalColor;
    }
}
