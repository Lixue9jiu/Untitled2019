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

    private void Start()
    {
        UpdateCenter();
    }

    private void UpdateCenter()
    {
        center = RectTransformUtility.PixelAdjustRect(GetComponent<RectTransform>(), GetComponentInParent<Canvas>()).center;
        var scaler = GetComponentInParent<CanvasScaler>();

        float referenceWidth = scaler.referenceResolution.x;

        float referenceHeight = scaler.referenceResolution.y;

        float match = scaler.matchWidthOrHeight;

        float offect = (Screen.width / referenceWidth) * (1 - match) + (Screen.height / referenceHeight) * match;

        center *= offect;
    }

    private void Update()
    {
        leftButton.Pressed = false;
        rightButton.Pressed = false;
        forwardButton.Pressed = false;
        backButton.Pressed = false;

        CrossPlatfromInput.SetAxis("Vertical", 0);
        CrossPlatfromInput.SetAxis("Horizontal", 0);

        if (pointerPos.HasValue)
        {
            var p = pointerPos.Value - center;
            if (Mathf.Abs(p.x) > Mathf.Abs(p.y))
            {
                if (p.x < 0)
                {
                    leftButton.Pressed = true;
                    CrossPlatfromInput.SetAxis("Horizontal", -1);
                }
                else
                {
                    rightButton.Pressed = true;
                    CrossPlatfromInput.SetAxis("Horizontal", 1);
                }
            }
            else
            {
                if (p.y < 0)
                {
                    backButton.Pressed = true;
                    CrossPlatfromInput.SetAxis("Vertical", -1);
                }
                else
                {
                    forwardButton.Pressed = true;
                    CrossPlatfromInput.SetAxis("Vertical", 1);
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
