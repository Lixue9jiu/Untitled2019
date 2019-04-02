using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossPlatfromInput : MonoBehaviour
{
    public static CrossPlatfromInput instance;

    Dictionary<string, float> axis = new Dictionary<string, float>();
    Dictionary<string, bool> buttonDown = new Dictionary<string, bool>();
    Dictionary<string, bool> buttonUp = new Dictionary<string, bool>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void LateUpdate()
    {
        buttonDown.Clear();
        buttonUp.Clear();
    }

    public void SetAxis(string name, float value)
    {
        if (axis.ContainsKey(name))
            if (axis[name] < value)
            {
                buttonDown[name] = true;
            }
            else if (axis[name] > value)
            {
                buttonUp[name] = true;
            }
        axis[name] = value;
    }

    public float GetAxis(string name)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!axis.ContainsKey(name))
            axis[name] = 0;
        return axis[name];
#else
        return Input.GetAxis(name);
#endif
    }

    public bool GetButtonDown(string name)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!buttonDown.ContainsKey(name))
            buttonDown[name] = false;
        return buttonDown[name];
#else
        return Input.GetButtonDown(name);
#endif
    }

    public bool GetButtonUp(string name)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!buttonUp.ContainsKey(name))
            buttonUp[name] = false;
        return buttonUp[name];
#else
        return Input.GetButtonUp(name);
#endif
    }
}
