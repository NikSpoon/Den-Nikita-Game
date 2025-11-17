using Unity.Cinemachine;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _root;
    [SerializeField] private CinemachineCamera _camera;
    private void Awake()
    {
        if (_player != null)
        {
          var player =  Instantiate(_player, _root.position,_root.rotation,null);

            var camera = _camera.GetComponent<CinemachineCamera>();
            camera.Target.TrackingTarget = player.transform;
            camera.Target.LookAtTarget = player.transform;
        }
    }
}
