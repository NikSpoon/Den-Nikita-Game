using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _root;
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private CameraTargetSpawner _cameraTarget;
    private void Awake()
    {
        if (_player != null)
        {
            var PlayerProfaile = Context.Instance.PlayerProfaile;

            var player = Instantiate(_player, _root.position, _root.rotation, null);
            var hero = Instantiate(PlayerProfaile.GetHero(), player.transform.position, player.transform.rotation, null);

            var camera = _camera.GetComponent<CinemachineCamera>();
            var input = hero.GetComponent<InputSystem>();

            camera.Target.TrackingTarget = player.transform;
            //  camera.Target.LookAtTarget = player.transform;
            // camera.Target.LookAtTarget = _cameraTarget.CameraLoocer(input).transform;
            _cameraTarget.CameraLoocer(input);
        }

    }
}
