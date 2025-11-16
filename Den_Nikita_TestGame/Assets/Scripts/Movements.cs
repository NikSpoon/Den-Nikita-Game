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

    private void FixedUpdate()
    {
        var nextPosition = _rb.position + Direction * _speed * Time.fixedDeltaTime;
        _rb.MovePosition(nextPosition);
    }

    public void Jump()
    {
        StartCoroutine(JumpWaiter());

    }
    public IEnumerator JumpWaiter()
    {
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.1f);
    }

}

