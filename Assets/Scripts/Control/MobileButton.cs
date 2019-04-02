using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(ButtonBasePanel))]
public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    string inputName;

    ButtonBasePanel basePanel;

    bool isPressed;

    private void Awake()
    {
        basePanel = GetComponent<ButtonBasePanel>();
    }

    private void Update()
    {
        basePanel.Pressed = isPressed;
        CrossPlatfromInput.instance.SetAxis(inputName, isPressed ? 1 : 0);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
}
