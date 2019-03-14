using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CrossPlatfromInput
{
    static Dictionary<string, float> axis = new Dictionary<string, float>();

    public static void SetAxis(string name, float value)
    {
#if UNITY_ANDROID
        axis[name] = value;
#endif
    }

    public static float GetAxis(string name)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!axis.ContainsKey(name))
            axis[name] = 0;
        return axis[name];
#else
        return Input.GetAxis(name);
#endif
    }
}
