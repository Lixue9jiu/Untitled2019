using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class InventoryItemFrame : MonoBehaviour
{
    private static Camera sharedCamera;

    TerrainRenderer terrainRenderer;

    private RawImage m_image;

    [SerializeField]
    private RawImage m_tint;

    private bool m_selected;

    public bool Selected
    {
        get
        {
            return m_selected;
        }
        set
        {
            m_selected = value;
            m_tint.enabled = m_selected;
        }
    }

    private void Awake()
    {
        m_image = GetComponent<RawImage>();
    }

    private void Start()
    {
        if (terrainRenderer == null)
            terrainRenderer = FindObjectOfType<TerrainRenderer>();

        if (sharedCamera == null)
        {
            sharedCamera = new Camera();
            sharedCamera.orthographic = true;
            sharedCamera.backgroundColor = new Color(1, 1, 1, 0);
        }
    }

    private void DrawBlock(int value)
    {
        MeshBuilder builder = new MeshBuilder();
        IBlockRenderer r = terrainRenderer.BlockRenderers[BlockData.GetContent(value)];
        //r.DrawStandalone(value, builder);
    }

    private void OnDestroy()
    {
        if (sharedCamera != null)
        {
            Destroy(sharedCamera);
            sharedCamera = null;
        }
    }
}
