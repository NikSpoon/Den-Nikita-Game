using UnityEngine;

public class StoneElement : BaseTerrainElement
{
    protected override void ConfigureCollider()
    {
        elementCollider.material = ElementPhysicMaterial;
    }
}
