

using UnityEngine;

public class MarchingTransform : Marching
{
    [SerializeField] private MarchingDebuger MarchingDebuger;
    private void Update()
    {
        if (Input.GetKey(KeyCode.Z))
            PlaseTerraine(MarchingDebuger.TargetCube);

        if (Input.GetKey(KeyCode.X))
            RemuveTerraine(MarchingDebuger.TargetCube);
    }
}
