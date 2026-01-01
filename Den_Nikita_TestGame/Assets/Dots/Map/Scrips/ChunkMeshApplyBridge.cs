using Unity.Entities;
using UnityEngine;

public class ChunkMeshApplyBridge : MonoBehaviour
{
    public Entity ChunkEntity;

    MeshFilter _mf;
    MeshCollider _mc;
    Mesh _mesh;

    [SerializeField] float colliderDebounce = 0.08f;
    float _nextColliderTime;

    void Awake()
    {
        _mf = GetComponent<MeshFilter>();
        _mc = GetComponent<MeshCollider>();

        _mesh = new Mesh();
        _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // на всякий
        _mf.sharedMesh = _mesh;
    }
    World FindServerWorld()
    {
        foreach (var w in World.All)
            if (w.IsCreated && (w.Flags & WorldFlags.GameServer) != 0)
                return w;
        return default;
    }
    void Update()
    {
        var world = FindServerWorld();
        if (!world.IsCreated) return;

        var em = world.EntityManager;

        var q = em.CreateEntityQuery(ComponentType.ReadOnly<ChunkRootTag>());
        if (q.IsEmpty) return;

        var chunkEntity = q.GetSingletonEntity();

        var vbuf = em.GetBuffer<MeshVertex>(ChunkEntity);
        var ibuf = em.GetBuffer<MeshTri>(ChunkEntity);

        // копируем в managed (для простоты первого теста)
        var v = new Vector3[vbuf.Length];
        for (int i = 0; i < v.Length; i++)
            v[i] = (Vector3)vbuf[i].Value;

        var t = new int[ibuf.Length];
        for (int i = 0; i < t.Length; i++)
            t[i] = ibuf[i].Value;

        _mesh.Clear();
        _mesh.vertices = v;
        _mesh.triangles = t;
        _mesh.RecalculateNormals();
        _mesh.RecalculateTangents();

        // MeshCollider debounce (как ты хотел)
        if (_mc != null && Time.time >= _nextColliderTime)
        {
            _nextColliderTime = Time.time + colliderDebounce;
            _mc.sharedMesh = null;
            _mc.sharedMesh = _mesh;
        }

        em.RemoveComponent<MeshDirty>(ChunkEntity);
    }
}
public struct ChunkRootTag : IComponentData { }