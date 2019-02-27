using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MobileArrowKeys : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]
    ArrowKey leftButton;
    [SerializeField]
    ArrowKey rightButton;
    [SerializeField]
    ArrowKey forwardButton;
    [SerializeField]
    ArrowKey backButton;

    GraphicRaycaster raycaster;

    Vector2? pointerPos;

    private void Update()
    {
        leftButton.Pressed = false;
        rightButton.Pressed = false;
        forwardButton.Pressed = false;
        backButton.Pressed = false;

        if (pointerPos.HasValue)
        {
            var p = pointerPos.Value;
            if (leftButton.Contains(p))
            {
                leftButton.Pressed = true;
            }
            else if (rightButton.Contains(p))
            {
                rightButton.Pressed = true;
            }
            else if (forwardButton.Contains(p))
            {
                forwardButton.Pressed = true;
            }
            else if (backButton.Contains(p))
            {
                backButton.Pressed = true;
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
