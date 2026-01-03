using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
public partial class GenerateTerrainSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<TerrainSettingsComponent>();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (settingsRO, settingsEntity) in SystemAPI
                     .Query<RefRO<TerrainSettingsComponent>>()
                     .WithEntityAccess())
        {
            var settings = settingsRO.ValueRO;

            var filter = new CollisionFilter
            {
                BelongsTo = 1u,
                CollidesWith = 1u,
                GroupIndex = 0
            };

            var boxGeo = new BoxGeometry
            {
                Center = float3.zero,
                Size = new float3(1f),
                Orientation = quaternion.identity,
                BevelRadius = 0f
            };

          //  var collider = BoxCollider.Create(boxGeo, filter);

            for (int x = 0; x < settings.Width; x++)
                for (int y = 0; y < settings.Height; y++)
                    for (int z = 0; z < settings.Depth; z++)
                    {
                        var pos = new float3(x, y, z);
                        var e = ecb.CreateEntity();

                        ecb.AddComponent(e, LocalTransform.FromPosition(pos));
                       // ecb.AddComponent(e, new PhysicsCollider { Value = collider });

                        // ВАЖНО: чтобы физика вообще включила entity в сборку мира
                        ecb.AddComponent<Simulate>(e);

                        // ВАЖНО: PhysicsWorldIndex в 1.4.x — SharedComponent
                        ecb.AddSharedComponent(e, new PhysicsWorldIndex { Value = 0 });

                        ecb.AddComponent(e, new BoxMapTeg());
                    }

            ecb.RemoveComponent<TerrainSettingsComponent>(settingsEntity);
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
