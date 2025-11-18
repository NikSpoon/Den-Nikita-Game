using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Rotator : MonoBehaviour
{
   
    [SerializeField] private float _rotateSpeed = 2;
    public Vector3 Direction { get; set; }
    private void Update()
    {
        Quaternion targetRot = Quaternion.LookRotation(Direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, _rotateSpeed * Time.deltaTime);
    }
}


