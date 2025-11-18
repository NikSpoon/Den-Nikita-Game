using UnityEngine;

public class CameraTargetSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _cameraTarget;
   
    public GameObject CameraLoocer(InputSystem inputSystem)
    {
        return Instantiate(_cameraTarget, inputSystem.LockDirection, gameObject.transform.rotation);
    }
}
