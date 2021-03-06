﻿using UnityEngine;
using System.Collections;

public class TerrainRaycaster : MonoBehaviour
{
    [SerializeField]
    private LayerMask ground;

    TerrainManager m_terrain;
    BlockManager m_blockManager;

#if !UNITY_ANDROID
    [SerializeField]
    GameObject cube;
    bool cooldownReady = true;
#endif

    public struct RaycastResult
    {
        public Vector3Int point;
        public int value;
        public int face;
    }

    private void Awake()
    {
        m_terrain = GetComponent<TerrainManager>();
        m_blockManager = GetComponent<BlockManager>();
    }

#if !UNITY_ANDROID
    private void Update()
    {
        var result = Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f)), 20);

        cube.SetActive(result.HasValue);
        if (result.HasValue)
        {
            GetComponent<LabelRenderer>().AddLabel($"looking at: {result.Value.point}, face: {result.Value.face}");
            cube.transform.position = result.Value.point + new Vector3(0.5f, 0.5f, 0.5f);
        }

        if (Input.GetKey(KeyCode.Mouse0) && result.HasValue)
        {
            var pos = result.Value.point;
            m_terrain.SetCellValue(pos.x, pos.y, pos.z, 0);
            StartCoroutine(CountCooldown());
        }
    }

    private IEnumerator CountCooldown()
    {
        cooldownReady = false;
        yield return new WaitForSeconds(0.3f);
        cooldownReady = true;
    }
#endif

    public RaycastResult? Raycast(Ray ray, float maxDist = 20)
    {
        Vector3 delta = new Vector3(ray.direction.x == 0 ? int.MaxValue : Mathf.Abs(1 / ray.direction.x), ray.direction.y == 0 ? int.MaxValue : Mathf.Abs(1 / ray.direction.y), ray.direction.z == 0 ? int.MaxValue : Mathf.Abs(1 / ray.direction.z));
        Vector3Int map = Vector3Int.FloorToInt(ray.origin);
        Vector3Int step = new Vector3Int();
        Vector3 side = new Vector3();
        if (ray.direction.x < 0)
        {
            step.x = -1;
            side.x = (ray.origin.x - map.x) * delta.x;
        }
        else
        {
            step.x = 1;
            side.x = (map.x + 1 - ray.origin.x) * delta.x;
        }
        if (ray.direction.y < 0)
        {
            step.y = -1;
            side.y = (ray.origin.y - map.y) * delta.y;
        }
        else
        {
            step.y = 1;
            side.y = (map.y + 1 - ray.origin.y) * delta.y;
        }
        if (ray.direction.z < 0)
        {
            step.z = -1;
            side.z = (ray.origin.z - map.z) * delta.z;
        }
        else
        {
            step.z = 1;
            side.z = (map.z + 1 - ray.origin.z) * delta.z;
        }
        int dist = 0;
        int dim = 0;
        while (dist < maxDist)
        {
            int value = m_terrain.GetCellValue(map.x, map.y, map.z);
            if (value != 0)
            {
                return new RaycastResult
                {
                    point = map,
                    face = dim * 2 + (step[dim] + 1) / 2,
                    value = value
                };
            }

            Min(side.x, side.y, side.z, out dim);
            switch (dim)
            {
                case 0:
                    map.x += step.x;
                    side.x += delta.x;
                    break;
                case 1:
                    map.y += step.y;
                    side.y += delta.y;
                    break;
                case 2:
                    map.z += step.z;
                    side.z += delta.z;
                    break;
            }
            dist++;
        }
        return null;
    }

    private void Min(float a, float b, float c, out int min)
    {
        float temp;
        if (a < b)
        {
            temp = a;
            min = 0;
        }
        else
        {
            temp = b;
            min = 1;
        }
        if (c < temp)
            min = 2;
    }
}
