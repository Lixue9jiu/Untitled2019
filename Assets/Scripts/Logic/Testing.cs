using UnityEngine;
using System.Collections;

public class Testing : MonoBehaviour
{
    Mesh mesh;
    Plane[] frustum;
    Rect r;

    private void Update()
    {
        if (frustum == null)
        {
            Camera c = GetComponentInChildren<Camera>();
            var t = c.transform;
            Vector3[] vs = { new Vector3Int(1, 1, 2), new Vector3Int(0, 1, 2), new Vector3Int(0, 1, 1), new Vector3Int(1, 1, 1) };
            //Vector3[] vs = { new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 0, 1) };
            //r = new Rect();
            //var vs2 = SortVertices(c, vs, out r);

            //foreach (Vector3 v in vs)
            //    Debug.Log(v);
            //var fs = CullingManager.ClampFrustum2(GeometryUtility.CalculateFrustumPlanes(c), vs2, t.position, t.forward, t.right, t.up);
            //frustum = fs;

            //mesh = new Mesh();
            //mesh.vertices = vs;
            //mesh.triangles = new int[] { 0, 1, 3, 1, 2, 3 };
            //mesh.RecalculateNormals();
        }
    }

    private void OnGUI()
    {
        GUI.Button(r, "test");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Vector3.zero, frustum[0].normal);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(Vector3.zero, frustum[1].normal);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(Vector3.zero, frustum[2].normal);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(Vector3.zero, frustum[3].normal);

        Gizmos.DrawWireMesh(mesh, Vector3.zero);
    }

    public static Vector3[] SortVertices(Camera c, Vector3[] vectors, out Rect rect)
    {
        var first = c.WorldToScreenPoint(vectors[0]);
        first.y = Screen.height - first.y;
        float minx = first.x;
        float miny = first.y;
        float maxx = first.x;
        float maxy = first.y;

        for (int i = 1; i < 4; i++)
        {
            var current = c.WorldToScreenPoint(vectors[i]);
            current.y = Screen.height - current.y;
            if (minx > current.x)
            {
                minx = current.x;
            }
            else if (maxx < current.x)
            {
                maxx = current.x;
            }
            if (miny > current.y)
            {
                miny = current.y;
            }
            else if (maxy < current.y)
            {
                maxy = current.y;
            }
        }
        rect = new Rect(minx, miny, maxx - minx, maxy - miny);
        return new Vector3[]
        {
            c.ScreenToWorldPoint(new Vector3(minx, miny, 1)),
            c.ScreenToWorldPoint(new Vector3(minx, miny, 1)),
            c.ScreenToWorldPoint(new Vector3(minx, miny, 1)),
            c.ScreenToWorldPoint(new Vector3(minx, miny, 1))
        };
    }
}
