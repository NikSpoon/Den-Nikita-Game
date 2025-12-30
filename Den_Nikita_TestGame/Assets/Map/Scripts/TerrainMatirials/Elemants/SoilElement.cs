using UnityEngine;

public class SoilElement : BaseTerrainElement
{
    protected override void ConfigureCollider()
    {
        elementCollider.material = ElementPhysicMaterial;
    }
}