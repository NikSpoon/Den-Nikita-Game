using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EcsTerrainMeshRenderer : MonoBehaviour
{
    private Mesh _mesh;

    private EntityManager _em;
    private Entity _meshEntity;
    private bool _hasEntity;

    void Awake()
    {
        _mesh = new Mesh
        {
            name = "ECS Terrain Mesh",
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };

        GetComponent<MeshFilter>().sharedMesh = _mesh;
    }

    static World GetClientOrDefaultWorld()
    {
        // Если есть NetCode — берём ClientWorld
        foreach (var w in World.All)
        {
            if (w != null && w.IsCreated && w.Name.Contains("Client"))
                return w;
        }

        return World.DefaultGameObjectInjectionWorld;
    }

    void Update()
    {
        var world = GetClientOrDefaultWorld();
        if (world == null || !world.IsCreated)
            return;

        _em = world.EntityManager;

        // 1) Найти entity с мешем
        if (!_hasEntity)
        {
            var query = _em.CreateEntityQuery(
                typeof(TerrainMeshTag),
                typeof(MeshDirty),
                typeof(MeshVertex),
                typeof(MeshIndex)
            );

            if (query.IsEmptyIgnoreFilter)
                return;

            _meshEntity = query.GetSingletonEntity();
            _hasEntity = true;
        }

        // 2) Проверить dirty
        var dirty = _em.GetComponentData<MeshDirty>(_meshEntity);
        if (dirty.Value == 0)
            return;

        var vBuf = _em.GetBuffer<MeshVertex>(_meshEntity);
        var iBuf = _em.GetBuffer<MeshIndex>(_meshEntity);
        bool hasNormals = _em.HasBuffer<MeshNormal>(_meshEntity);
        var nBuf = hasNormals ? _em.GetBuffer<MeshNormal>(_meshEntity) : default;

        if (vBuf.Length == 0 || iBuf.Length == 0)
        {
            _mesh.Clear();
            _em.SetComponentData(_meshEntity, new MeshDirty { Value = 0 });
            return;
        }

        // 3) Копируем в Unity Mesh
        var vertices = new Vector3[vBuf.Length];
        for (int i = 0; i < vBuf.Length; i++)
        {
            var p = vBuf[i].Value;
            vertices[i] = new Vector3(p.x, p.y, p.z);
        }

        var triangles = new int[iBuf.Length];
        for (int i = 0; i < iBuf.Length; i++)
            triangles[i] = iBuf[i].Value;

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;

        if (hasNormals && nBuf.Length == vBuf.Length)
        {
            var normals = new Vector3[nBuf.Length];
            for (int i = 0; i < nBuf.Length; i++)
            {
                var n = nBuf[i].Value;
                normals[i] = new Vector3(n.x, n.y, n.z);
            }
            _mesh.normals = normals;
        }
        else
        {
            _mesh.RecalculateNormals();
        }

        _mesh.RecalculateBounds();

        // 4) Сброс dirty
        _em.SetComponentData(_meshEntity, new MeshDirty { Value = 0 });
    }
}
