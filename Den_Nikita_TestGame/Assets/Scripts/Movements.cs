using JetBrains.Annotations;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Movements : MonoBehaviour
{
    public Vector3 Direction { get; set; }
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _groundCheckDistance = 0.2f; // длина луча вниз
    [SerializeField] private LayerMask _groundMask; // слой земли
    private void FixedUpdate()
    {
        var nextPosition = _rb.position + Direction * _speed * Time.fixedDeltaTime;

        _rb.MovePosition(nextPosition);

        Debug.DrawLine(_rb.position, _rb.position + Vector3.down * _groundCheckDistance, Color.red);
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
        // Выпускаем луч вниз из позиции Rigidbody
        return Physics.Raycast(_rb.position, Vector3.down, _groundCheckDistance, _groundMask);
    }

    private IEnumerator JumpWaiter()
    {
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.1f);
    }
}

