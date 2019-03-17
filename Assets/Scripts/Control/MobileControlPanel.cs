using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MobileControlPanel : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    const float SHORT_TAP_TIME = 0.2f;
    const float LONG_HOLD_TIME = 0.4f;
    const float LONG_HOLD_MIN_OFFSET = 4f;
    const float MOUSE_OFFSET_DIVIDER = 8;

    int screenWidth;
    int screenHeight;

    bool isTouching;
    Vector2 pointerPosition;
    Vector2 pointerDelta;
    float cumulativeTime;

    bool isHolding;

    public bool IsLongHolding => isHolding && cumulativeTime > LONG_HOLD_TIME;

    public bool IsTaping
    {
        get
        {
            if (!isTouching && cumulativeTime < SHORT_TAP_TIME)
            {
                cumulativeTime = LONG_HOLD_TIME;
                return true;
            }
            return false;
        }
    }

    public Vector2? PointerPosition => isTouching ? new Vector2?(pointerPosition) : null;

    private void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    private void Update()
    {
        CrossPlatfromInput.SetAxis("Mouse X", 0);
        CrossPlatfromInput.SetAxis("Mouse Y", 0);

        if (isTouching)
        {
            CrossPlatfromInput.SetAxis("Mouse X", pointerDelta.x / MOUSE_OFFSET_DIVIDER);
            CrossPlatfromInput.SetAxis("Mouse Y", pointerDelta.y / MOUSE_OFFSET_DIVIDER);

            cumulativeTime += Time.deltaTime;
            if (cumulativeTime < LONG_HOLD_TIME)
                isHolding &= pointerDelta.sqrMagnitude < LONG_HOLD_MIN_OFFSET;
        }
        pointerDelta = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        pointerDelta = eventData.delta;
        pointerPosition = eventData.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouching = true;
        pointerPosition = eventData.position;
        cumulativeTime = 0;
        isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTouching = false;
        isHolding = false;
    }
}
