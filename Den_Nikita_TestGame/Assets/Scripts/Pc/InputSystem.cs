using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public bool JumpPressed { get; private set; }
    public Vector3 Direction { get; private set; }
    public float HorizontalImput { get; private set; }
    public float VerticalImput { get; private set; }
    public Vector3 LockDirection { get; private set; }

    public Vector3 HitInfo { get; private set; }
    private Camera _camera;
    private Collider _myColl;


    public void Start()
    {
        _myColl = gameObject.GetComponent<Collider>();
        _camera = Camera.main;
    }
    private void Update()
    {
        HorizontalImput = Input.GetAxis("Horizontal");
        VerticalImput = Input.GetAxis("Vertical");

        var locatDir = new Vector3(HorizontalImput, 0, VerticalImput);
        Direction = transform.TransformDirection(locatDir);
        JumpPressed = Input.GetKey(KeyCode.Space);

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hitInfo, 2000f);
        if (hitInfo.collider == _myColl)
        {
            return;
        }
        else
        {
            HitInfo = hitInfo.point;
            var direction = hitInfo.point - transform.position;
            direction.y = 0;
            LockDirection = direction;
        }
    }
}
