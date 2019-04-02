using UnityEngine;
using System.Collections;

public class DoubleTapListener
{
    readonly string m_monitorInputName;
    readonly float m_coolDown;

    float coolDown;
    int tapCount;

    public bool IsDoubleTapping
    {
        get
        {
            if (tapCount > 1 && coolDown > 0)
            {
                return true;
            }
            return false;
        }
        set
        {
            if (!value)
            {
                tapCount = 0;
            }
        }
    }

    public DoubleTapListener(float coolDown, string inputName)
    {
        m_coolDown = coolDown;
        m_monitorInputName = inputName;
    }

    public void Update()
    {
        if (CrossPlatfromInput.instance.GetButtonDown(m_monitorInputName))
        {
            tapCount++;
            if (tapCount == 1)
            {
                coolDown = m_coolDown;
            }
        }

        if (coolDown < 0)
        {
            tapCount = 0;
        }
        coolDown -= Time.deltaTime;
    }
}
