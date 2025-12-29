

using UnityEngine;

public class MarchingTransform : Marching
{
    [SerializeField] private MarchingDebuger MarchingDebuger;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && MarchingDebuger.TargetCube != null)
        {
            PlaseTerraine(MarchingDebuger.TargetCube);
        }
        if (Input.GetKeyDown(KeyCode.X) && MarchingDebuger.TargetCube != null)
        {
            RemuveTerraine(MarchingDebuger.TargetCube);
        }
    }
}
