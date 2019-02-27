using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragPanel : MonoBehaviour
{
    Rect m_area;
    int touchIndex;
    bool isDraging;

    private void Start()
    {
        m_area = transform.GetComponent<RectTransform>().rect;
    }

    private void Update()
    {
        if (!isDraging)
        {
            LabelRenderer.AddLabel(Input.touchCount);
            foreach(Touch t in Input.touches)
            {
                if (m_area.Contains(t.position))
                {
                    touchIndex = t.fingerId;
                    isDraging = true;
                }
            }
        }
        else
        {
            var delta = Input.GetTouch(touchIndex).deltaPosition;
            CrossPlatfromInput.SetAxis("Horizontal", delta.x);
            CrossPlatfromInput.SetAxis("Vertical", delta.y);
        }
    }

    private void LateUpdate()
    {
        CrossPlatfromInput.SetAxis("Horizontal", 0);
        CrossPlatfromInput.SetAxis("Vertical", 0);
    }
}
