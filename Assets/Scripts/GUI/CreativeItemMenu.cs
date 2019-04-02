using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class CreativeItemMenu : MonoBehaviour
{
    [SerializeField]
    GameObject itemFramePrefab;

    GridLayoutGroup grid;

    private void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
    }

    private void Start()
    {
        var blocks = FindObjectOfType<BlockManager>().Blocks;
        for (int i = 0; i < blocks.Length; i++)
        {
            if (!string.IsNullOrEmpty(blocks[i].renderer))
            {
                var itemFrame = Instantiate(itemFramePrefab, transform).GetComponent<InventoryItemFrame>();
                itemFrame.blockValue = i;
                itemFrame.onClick.AddListener(OnItemClicked);
            }
        }
    }

    private void OnItemClicked(InventoryItemFrame frame)
    {
        FindObjectOfType<MobileMinerController>().placeBlockValue = frame.blockValue;
        FindObjectOfType<WindowManager>().Escape();
    }
}
