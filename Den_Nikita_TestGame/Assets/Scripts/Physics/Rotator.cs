using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 15f;

    private Camera _camera;

    private Collider _selfCollider;

    private void Awake()
    {
        _selfCollider = GetComponent<Collider>();
    }
    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 2000f))
        {
            if (hit.collider == _selfCollider)
                return;

            Vector3 target = hit.point;

            Vector3 direction = (target - transform.position);
            direction.y = 0;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, _rotateSpeed * Time.deltaTime);
            }
        }
    }
}


