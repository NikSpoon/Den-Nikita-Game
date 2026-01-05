using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation | WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(ChunkPipelineGroup))]
[UpdateAfter(typeof(ChunkMarchingCubesMeshSystem))]
public partial class ChunkPhysicsBuildSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<ChunkWorldSettings>();
    }

    protected override void OnUpdate()
    {
        var s = SystemAPI.GetSingleton<ChunkWorldSettings>();
        int built = 0;

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (stateRO, entity) in SystemAPI
                     .Query<RefRO<ChunkState>>()
                     .WithEntityAccess())
        {
            if (built >= s.MaxPhysicsPerFrame) break;

            if (stateRO.ValueRO.Stage != ChunkStage.Meshed)
                continue;

            // уже есть коллайдер — не трогаем
            if (EntityManager.HasComponent<PhysicsCollider>(entity))
                continue;

            var vBuf = EntityManager.GetBuffer<MeshVertex>(entity);
            var iBuf = EntityManager.GetBuffer<MeshIndex>(entity);

            if (vBuf.Length < 3 || iBuf.Length < 3)
                continue;

            // Unity.Physics.MeshCollider.Create требует NativeArray
            var verts = new NativeArray<float3>(vBuf.Length, Allocator.Temp);
            for (int i = 0; i < vBuf.Length; i++) verts[i] = vBuf[i].Value;

            int triCount = iBuf.Length / 3;
            var tris = new NativeArray<int3>(triCount, Allocator.Temp);
            for (int t = 0; t < triCount; t++)
            {
                int i0 = iBuf[t * 3 + 0].Value;
                int i1 = iBuf[t * 3 + 1].Value;
                int i2 = iBuf[t * 3 + 2].Value;
                tris[t] = new int3(i0, i1, i2);
            }

            var filter = CollisionFilter.Default;

            var collider = Unity.Physics.MeshCollider.Create(verts, tris, filter);

            // чистим temp
            verts.Dispose();
            tris.Dispose();

            // ✅ ВАЖНО: добавляем через ECB (структурные изменения)
            ecb.AddComponent(entity, new PhysicsCollider { Value = collider });

            // обновляем stage через ECB не обязательно (это не structural), но делаем аккуратно:
            var st = EntityManager.GetComponentData<ChunkState>(entity);
            st.Stage = ChunkStage.PhysicsBuilt;
            EntityManager.SetComponentData(entity, st);

            built++;
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
