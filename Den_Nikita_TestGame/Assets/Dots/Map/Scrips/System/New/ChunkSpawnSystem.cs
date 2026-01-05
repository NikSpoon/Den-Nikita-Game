using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation | WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(ChunkPipelineGroup))]
public partial class ChunkSpawnSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<ChunkWorldSettings>();
        RequireForUpdate<PlayerTag>();
    }

    protected override void OnUpdate()
    {
        var s = SystemAPI.GetSingleton<ChunkWorldSettings>();

        // ✅ ВАЖНО: размер чанка в мире = (voxels-1) * cellSize
        float3 chunkWorldSize = new float3(
            (s.Voxels.x - 1) * s.CellSize,
            (s.Voxels.y - 1) * s.CellSize,
            (s.Voxels.z - 1) * s.CellSize
        );

        var player = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerPos = SystemAPI.GetComponent<LocalTransform>(player).Position;

        int2 playerChunkXZ = new int2(
            (int)math.floor(playerPos.x / chunkWorldSize.x),
            (int)math.floor(playerPos.z / chunkWorldSize.z)
        );

        // существующие чанки (координаты)
        var existing = new NativeHashSet<int3>(4096, Allocator.Temp);
        foreach (var c in SystemAPI.Query<RefRO<ChunkCoord>>())
            existing.Add(c.ValueRO.Value);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        int spawnedThisFrame = 0;
        int yMin = -s.UndergroundY;
        int yMax = math.max(0, s.SkyY); // sky можно, но обычно 0

        for (int cy = yMin; cy <= yMax; cy++)
            for (int dz = -s.RadiusXZ; dz <= s.RadiusXZ; dz++)
                for (int dx = -s.RadiusXZ; dx <= s.RadiusXZ; dx++)
                {
                    if (spawnedThisFrame >= s.MaxSpawnPerFrame)
                        goto DONE;

                    int3 coord = new int3(playerChunkXZ.x + dx, cy, playerChunkXZ.y + dz);
                    if (existing.Contains(coord)) continue;

                    var chunk = ecb.CreateEntity();

                    ecb.AddComponent(chunk, new ChunkCoord { Value = coord });
                    ecb.AddComponent<ChunkSpawnedTag>(chunk);
                    ecb.AddComponent(chunk, new ChunkState { Stage = ChunkStage.Spawned });

                    // якорь: coord(0,0,0) => position(0,0,0)
                    float3 origin = coord * chunkWorldSize;
                    ecb.AddComponent(chunk, LocalTransform.FromPosition(origin));

                    if (coord.y > 0) ecb.AddComponent<SkyChunkTag>(chunk);
                    else if (coord.y < 0) ecb.AddComponent<UndergroundChunkTag>(chunk);
                    else ecb.AddComponent<SurfaceChunkTag>(chunk);

                    // буферы (всем, кроме sky можно тоже дать, но sky мы не мешим/не терраформим)
                    ecb.AddBuffer<VoxelDensity>(chunk);
                    ecb.AddBuffer<VoxelMaterial>(chunk);

                    ecb.AddBuffer<MeshVertex>(chunk);
                    ecb.AddBuffer<MeshNormal>(chunk);
                    ecb.AddBuffer<MeshIndex>(chunk);
                    ecb.AddBuffer<MeshUV>(chunk);

                    ecb.AddComponent(chunk, new MeshDirty { Value = 0 });

                    spawnedThisFrame++;
                }

            DONE:
        ecb.Playback(EntityManager);
        ecb.Dispose();
        existing.Dispose();
    }
}
