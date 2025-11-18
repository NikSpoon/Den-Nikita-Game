using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CameraLoocerController : MonoBehaviour
{
    [SerializeField] private float _maxDistance = 3f;     
    [SerializeField] private float _smooth = 6f;         

    private InputSystem _inputSystem;
    private GameObject _player;
    private PlayerController _playerController;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _inputSystem = _player.GetComponent<InputSystem>();
        _playerController = _player.GetComponent<PlayerController>();
    }

    private void LateUpdate()
    {
        Vector3 playerPos = _player.transform.position;
        Vector3 hitPos = _inputSystem.HitInfo;

        hitPos.y = playerPos.y; //??????????????????????

        Vector3 direction = hitPos - playerPos;

        if (direction.magnitude > _maxDistance)
            direction = direction.normalized * _maxDistance;

        Vector3 targetPos = playerPos + direction;

        transform.position = Vector3.Lerp(transform.position, targetPos, _smooth * Time.deltaTime);

        _playerController.InitRotation(direction);
     
       
    }

}