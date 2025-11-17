using UnityEngine;

public class PlayerCo : MonoBehaviour
{
    [SerializeField] private InputSystem _inputSystem;
    [SerializeField] private Movements _movements;

    private void Update()
    {
        _movements.Direction = _inputSystem.Direction;
        if (_inputSystem.JumpPressed)
        {
            _movements.Jump();
        }
    }
}
