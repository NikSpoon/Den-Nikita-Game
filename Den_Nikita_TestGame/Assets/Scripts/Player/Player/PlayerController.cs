using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputSystem _inputSystem;
    [SerializeField] private Movements _movements;
    [SerializeField] private Rotator _rotator;

    private Vector3 _lockDirection;
    public void InitRotation(Vector3 direction)
    {
        _lockDirection = direction;
    }
    private void Update()
    {
        _movements.Direction = _inputSystem.Direction;

        _rotator.Direction = _lockDirection;

        if (_inputSystem.JumpPressed)
        {
            _movements.Jump();
        }
    }
}
