using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;
    private void Update()
    {
        transform.position = _target.position + _offset;
        transform.rotation = _target.rotation;
    }

}

