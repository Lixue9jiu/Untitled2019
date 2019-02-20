using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LabelRenderer : MonoBehaviour
{
    List<string> labels = new List<string>();
    bool isDirty;

    public void AddLabel(string str)
    {
        if (isDirty)
        {
            isDirty = false;
            labels.Clear();
        }
        labels.Add(str);
    }

#if DEBUG
    float fps;

    private void Start()
    {
        InvokeRepeating("UpdateFPS", 0, 1.5f);
    }

    private void Update()
    {
        AddLabel($"FPS: {fps}");
    }

    private void UpdateFPS()
    {
        fps = 1 / Time.deltaTime;
    }
#endif

    private void OnGUI()
    {
        for (int i = 0; i < labels.Count; i++)
        {
            GUI.Label(new Rect(0, i * 20, 400, 20), labels[i]);
        }
    }

    private void LateUpdate()
    {
        isDirty = true;
    }
}
