using Unity.Entities;
using UnityEngine;

public class CreateOneChunkEntity : MonoBehaviour
{
    public ChunkMeshApplyBridge Bridge;

    void Start()
    {
        var world = World.DefaultGameObjectInjectionWorld;
        var em = world.EntityManager;

        var e = em.CreateEntity();

        em.AddComponentData(e, new ChunkSettings
        {
            Width = 32,
            Height = 10,
            Surface = 0.5f,
            Seed = 123
        });

        em.AddBuffer<Density>(e);
        em.AddBuffer<MeshVertex>(e);
        em.AddBuffer<MeshTri>(e);

        em.AddComponent<NeedDensityBuild>(e);

        // линк в мост
        Bridge.ChunkEntity = e;
    }
}
