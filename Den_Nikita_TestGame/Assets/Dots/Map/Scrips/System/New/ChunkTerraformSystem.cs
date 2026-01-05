
using Unity.Entities;
using Unity.Mathematics;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation | WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(ChunkPipelineGroup))]
[UpdateAfter(typeof(ChunkSpawnSystem))]
public partial class ChunkTerraformSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<ChunkWorldSettings>();
    }

    protected override void OnUpdate()
    {
        var s = SystemAPI.GetSingleton<ChunkWorldSettings>();
        int3 vox = s.Voxels;
        int pointsCount = vox.x * vox.y * vox.z;

        // размеры чанка (в мире) = (voxels-1)*cell
        float3 chunkWorldSize = new float3(
            (vox.x - 1) * s.CellSize,
            (vox.y - 1) * s.CellSize,
            (vox.z - 1) * s.CellSize
        );

        int doneThisFrame = 0;

        foreach (var (coordRO, stateRW, entity) in SystemAPI
                     .Query<RefRO<ChunkCoord>, RefRW<ChunkState>>()
                     .WithAll<ChunkSpawnedTag>()
                     .WithNone<SkyChunkTag>()
                     .WithEntityAccess())
        {
            if (doneThisFrame >= s.MaxTerraformPerFrame)
                break;

            if (stateRW.ValueRO.Stage != ChunkStage.Spawned)
                continue;

            var coord = coordRO.ValueRO.Value;

            var density = EntityManager.GetBuffer<VoxelDensity>(entity);
            var mat = EntityManager.GetBuffer<VoxelMaterial>(entity);

            density.ResizeUninitialized(pointsCount);
            mat.ResizeUninitialized(pointsCount);

            // world origin этого чанка
            float3 chunkOrigin = coord * chunkWorldSize;

            int idx = 0;
            for (int z = 0; z < vox.z; z++)
                for (int y = 0; y < vox.y; y++)
                    for (int x = 0; x < vox.x; x++, idx++)
                    {
                        // позиция точки в мире
                        float3 wp = chunkOrigin + new float3(x * s.CellSize, y * s.CellSize, z * s.CellSize);

                        // --- Surface SDF ---
                        float hNoise = noise.snoise(new float2(
                            (wp.x + s.WorldSeed) * s.HeightFrequency,
                            (wp.z + s.WorldSeed) * s.HeightFrequency
                        ));
                        float groundY = s.BaseGroundY + (hNoise * 0.5f + 0.5f) * s.HeightAmplitude;

                        // Положительное = solid, отрицательное = air
                        float surfaceSdf = groundY - wp.y;

                        // --- Caves SDF ---
                        // 0..1
                        float c01 = noise.snoise(new float3(
                            (wp.x + s.WorldSeed) * s.CaveFrequency,
                            (wp.y + s.WorldSeed) * s.CaveFrequency,
                            (wp.z + s.WorldSeed) * s.CaveFrequency
                        )) * 0.5f + 0.5f;

                        // caveSdf < 0 => “воздух”
                        float caveSdf = (s.CaveThreshold - c01) * s.CaveStrength;

                        // Маска: чтобы не дырявить поверхность
                        // чем глубже под землёй, тем сильнее пещеры
                        float depth = groundY - wp.y;                  // >0 под землёй
                        float caveMask = math.saturate((depth - 2f) / 8f); // 0 на поверхности, 1 глубже

                        // Итоговая плотность: поверхность + пещеры
                        float densityValue = math.lerp(surfaceSdf, math.min(surfaceSdf, caveSdf), caveMask);

                        density[idx] = new VoxelDensity { Value = densityValue };
                        mat[idx] = new VoxelMaterial { Value = (byte)(densityValue > s.IsoLevel ? 1 : 0) };
                    }

            stateRW.ValueRW.Stage = ChunkStage.Terraformed;
            doneThisFrame++;
        }
    }
}
