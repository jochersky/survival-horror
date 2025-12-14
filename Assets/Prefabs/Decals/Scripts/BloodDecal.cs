using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BloodDecal : MonoBehaviour
{
    private DecalProjector _decalProjector;
    
    [Header("Properties")]
    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;
    
    void Start()
    {
        _decalProjector = GetComponent<DecalProjector>();
        
        float sideLength = Random.Range(minSize, maxSize);
        float projectionDepth = _decalProjector.size.z;
        _decalProjector.size = new Vector3(sideLength, sideLength, projectionDepth);
        
    }
}
