using UnityEngine;
using System.Collections;

public class DoubleTapListener
{
    readonly string m_monitorInputName;
    readonly float m_coolDown;

    float coolDown;
    int tapCount;

    bool lastInput;

    public bool IsDoubleTapping
    {
        get
        {
            if (coolDown > 0 && tapCount > 1)
            {
                coolDown = m_coolDown;
                tapCount = 0;
                return true;
            }
            return false;
        }
    }

    public DoubleTapListener(float coolDown, string inputName)
    {
        m_coolDown = coolDown;
        m_monitorInputName = inputName;
    }

    public void Update()
    {
        coolDown -= Time.deltaTime;
        if (coolDown < 0)
        {
            coolDown = m_coolDown;
            tapCount = 0;
        }

        bool currentInput = CrossPlatfromInput.GetAxis(m_monitorInputName) > 0;
        if (!lastInput && currentInput)
        {
            tapCount++;
        }
        lastInput = currentInput;
    }
}
