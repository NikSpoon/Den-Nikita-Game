using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private InputSystem _input;
    private void Update()
    {
        if (_input.VerticalImput != 0 || _input.HorizontalImput != 0)
        {
            _animator.SetBool("IsRunning", true);

        }
        else
        {
            _animator.SetBool("IsRunning", false);
        }

        if (Input.GetKey(KeyCode.Mouse0) == true)
        {
            _animator.SetBool("IsAttack", true);
        }
        else
        {
            _animator.SetBool("IsAttack", false);
        }
       
    }
}
