using UnityEngine;

public class BlockMeshManager : MonoBehaviour
{
    private AssetBundle m_bundle;

    private void Start()
    {
        m_bundle = GetComponent<ContentManager>().GetBundle("meshes");
    }
}
