using System.Security.Cryptography;
using UnityEngine;

public class InputSystem : MonoBehaviour 
{
    public bool JumpPressed { get; private set; }
    public Vector3 Direction { get; private set; }
    public float HorizontalImput { get; private set; }
    public float VerticalImput { get; private set; }

    private void Update() 
    {
        Direction = new Vector3(HorizontalImput, 0, VerticalImput);
        JumpPressed = Input.GetKey(KeyCode.Space);
        HorizontalImput = Input.GetAxis("Horizontal");
        VerticalImput = Input.GetAxis("Vertical");
    }
}
