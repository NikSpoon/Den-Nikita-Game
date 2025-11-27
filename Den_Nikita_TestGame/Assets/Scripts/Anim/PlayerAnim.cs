using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    public InputSystem MyInput;
    private void Update()
    {
        if (MyInput.VerticalImput != 0 || MyInput.HorizontalImput != 0)
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
