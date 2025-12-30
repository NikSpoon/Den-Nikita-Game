using UnityEngine;

public abstract class BaseTerrainElement : MonoBehaviour
{
    public string ElementName { get; protected set; }
    public Material ElementMaterial { get; protected set; }
    public float Health { get; protected set; }
    public float Density { get; protected set; }


    public PhysicsMaterial ElementPhysicMaterial { get; protected set; }

    protected Collider elementCollider;
    protected abstract void ConfigureCollider();

    public virtual void Initialize(string name, Material material, float health, float density, PhysicsMaterial physicMaterial)
    {
        ElementName = name;
        ElementMaterial = material;
        Health = health;
        Density = density;
        ElementPhysicMaterial = physicMaterial;

        elementCollider = gameObject.AddComponent<MeshCollider>();

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = ElementMaterial;
        }

        ConfigureCollider();
    }
}