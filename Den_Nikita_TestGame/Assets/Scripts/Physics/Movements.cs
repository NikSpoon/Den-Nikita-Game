using System.Collections;
using UnityEngine;

public class Movements : MonoBehaviour
{
    public Vector3 Direction { get; set; }
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask _groundMask;

    private void FixedUpdate()
    {
        var nextPosition = _rb.position + Direction * _speed * Time.fixedDeltaTime;
        _rb.MovePosition(nextPosition);
    }

    public void Jump()
    {
        if (IsGrounded())
        {
            StartCoroutine(JumpWaiter());
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(_rb.position, Vector3.down, _groundCheckDistance, _groundMask);
    }

    private IEnumerator JumpWaiter()
    {
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.1f);
    }
    public void GetRB(Rigidbody rb)
    {
        if (_rb == null)
        {
            _rb = rb;

        }
    }
}

