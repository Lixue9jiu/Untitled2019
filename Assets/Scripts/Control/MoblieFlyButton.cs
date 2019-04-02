using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ButtonBasePanel))]
public class MoblieFlyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField]
    string inputName = "Fly";
    [SerializeField]
    float maxOffset = 100;

    float actualOffset;

    ButtonBasePanel basePanel;

    float flyValue;

    Vector2 startPos;

    private void Awake()
    {
        basePanel = GetComponent<ButtonBasePanel>();
    }

    private void Update()
    {
        if (transform.hasChanged)
        {
            actualOffset = maxOffset * transform.lossyScale.y;
            transform.hasChanged = false;
        }

        CrossPlatfromInput.instance.SetAxis(inputName, flyValue);
    }

    public void OnDrag(PointerEventData eventData)
    {
        flyValue = Mathf.Lerp(-1, 1, (eventData.position.y - startPos.y) / actualOffset);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        flyValue = 0;
    }
}
