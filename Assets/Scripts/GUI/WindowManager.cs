using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    [SerializeField]
    Canvas mainCanvas;

    GameObject currentWindow;

    public void AddWindow(GameObject windowPrefab)
    {
        currentWindow = Instantiate(windowPrefab, mainCanvas.transform);
    }

    public bool IsShowing<T>() where T : MonoBehaviour
    {
        return currentWindow != null && currentWindow.GetComponent<T>() != null;
    }

    public bool Escape()
    {
        if (currentWindow != null)
        {
            Destroy(currentWindow);
            currentWindow = null;
            return false;
        }
        return true;
    }
}
