using UnityEngine;
using System.Collections;

public class MobileMinerController : MonoBehaviour
{
    public int placeBlockValue;

    [SerializeField]
    MobileControlPanel controlPanel;

    [SerializeField]
    GameObject cube;

    TerrainManager m_terrain;
    TerrainRaycaster m_raycaster;

    TerrainRaycaster.RaycastResult? lastResult;
    bool cooldownReady = true;

    private void Awake()
    {
        m_terrain = GetComponent<TerrainManager>();
        m_raycaster = GetComponent<TerrainRaycaster>();
    }

    private void Start()
    {
        placeBlockValue = GetComponent<BlockManager>().FindBlock("game:monkey_head");
    }

    private void Update()
    {
        cube.SetActive(false);

        if (controlPanel.PointerPosition.HasValue)
        {
            var result = m_raycaster.Raycast(Camera.main.ScreenPointToRay(controlPanel.PointerPosition.Value), 20);
            lastResult = result;

            cube.SetActive(result.HasValue);
            if (result.HasValue)
            {
                GetComponent<LabelRenderer>().AddLabel($"looking at: {result.Value.point}, face: {result.Value.face}");
                cube.transform.position = result.Value.point + new Vector3(0.5f, 0.5f, 0.5f);

                if (cooldownReady && controlPanel.IsLongHolding)
                {
                    var pos = result.Value.point;
                    m_terrain.SetCellValue(pos.x, pos.y, pos.z, 0);
                    StartCoroutine(CountCooldown());
                }
            }
        }
        else if (controlPanel.IsTaping && lastResult.HasValue)
        {
            var pos = lastResult.Value.point + CellFace.FACES[lastResult.Value.face];
            if (!Physics.CheckBox(pos + new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.4f, 0.4f, 0.4f)))
                m_terrain.SetCellValue(pos.x, pos.y, pos.z, placeBlockValue);
        }
    }

    private IEnumerator CountCooldown()
    {
        cooldownReady = false;
        yield return new WaitForSeconds(0.3f);
        cooldownReady = true;
    }
}
