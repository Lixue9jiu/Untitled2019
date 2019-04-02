using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MobileArrowKeys : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]
    ButtonBasePanel leftButton;
    [SerializeField]
    ButtonBasePanel rightButton;
    [SerializeField]
    ButtonBasePanel forwardButton;
    [SerializeField]
    ButtonBasePanel backButton;

    Vector2 center;

    Vector2? pointerPos;

    private void UpdateCenter()
    {
        Camera c = GameObject.FindWithTag("StandaloneBlockRenderer").GetComponent<Camera>();
        center = c.WorldToScreenPoint(RectTransformUtility.PixelAdjustRect(GetComponent<RectTransform>(), GetComponentInParent<Canvas>()).center);
        center *= transform.lossyScale.x;
    }

    private void Update()
    {
        if (transform.hasChanged)
        {
            UpdateCenter();
            transform.hasChanged = false;
        }

        leftButton.Pressed = false;
        rightButton.Pressed = false;
        forwardButton.Pressed = false;
        backButton.Pressed = false;

        var input = CrossPlatfromInput.instance;
        input.SetAxis("Vertical", 0);
        input.SetAxis("Horizontal", 0);

        if (pointerPos.HasValue)
        {
            var p = pointerPos.Value - center;
            if (Mathf.Abs(p.x) > Mathf.Abs(p.y))
            {
                if (p.x < 0)
                {
                    leftButton.Pressed = true;
                    input.SetAxis("Horizontal", -1);
                }
                else
                {
                    rightButton.Pressed = true;
                    input.SetAxis("Horizontal", 1);
                }
            }
            else
            {
                if (p.y < 0)
                {
                    backButton.Pressed = true;
                    input.SetAxis("Vertical", -1);
                }
                else
                {
                    forwardButton.Pressed = true;
                    input.SetAxis("Vertical", 1);
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        pointerPos = eventData.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerPos = null;
    }
}
