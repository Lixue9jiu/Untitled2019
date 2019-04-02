using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RawImage))]
public class InventoryItemFrame : MonoBehaviour, IPointerClickHandler
{
    private static Matrix4x4 m_view = Matrix4x4.Rotate(Quaternion.AngleAxis(135, Vector3.up) * Quaternion.FromToRotation(new Vector3(1, 1, 1), new Vector3(1, 0, 1)));

    private Camera sharedCamera;
    [SerializeField]
    private Material sharedMaterial;

    TerrainRenderer terrainRenderer;
    BlockTextureManager blockTextureManager;

    private RawImage m_image;

    [SerializeField]
    private RawImage m_tint;

    private int iconLayer;

    public int blockValue = 7;

    private bool m_selected;

    private Matrix4x4 scale;
    private Matrix4x4 position;

    public ItemFrameClickEvent onClick = new ItemFrameClickEvent();

    [SerializeField]
    public class ItemFrameClickEvent : UnityEvent<InventoryItemFrame>
    {
    }

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
        iconLayer = LayerMask.NameToLayer("Icons");

        sharedCamera = GameObject.FindWithTag("StandaloneBlockRenderer").GetComponent<Camera>();

        terrainRenderer = FindObjectOfType<TerrainRenderer>();
        blockTextureManager = FindObjectOfType<BlockTextureManager>();
    }

    private void UpdateTransform()
    {
        var trans = GetComponent<RectTransform>();
        float s = transform.lossyScale.y * trans.sizeDelta.y / 2;
        scale = Matrix4x4.Scale(new Vector3(s, s, s));

        var pos = transform.position;
        position = Matrix4x4.Translate(new Vector3(pos.x, pos.y, 0));
    }

    private void Update()
    {
        if (blockValue != 0)
        {
            IBlockRenderer r = terrainRenderer.BlockRenderers[BlockData.GetContent(blockValue)];
            r.DrawStandalone(blockValue, position * scale * m_view, sharedMaterial, blockTextureManager, iconLayer, sharedCamera);
        }
        if (transform.hasChanged)
        {
            UpdateTransform();
            transform.hasChanged = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke(this);
    }
}
