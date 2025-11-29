using UnityEngine;
using UnityEngine.UI;

public class PlayreCanvasFinder : MonoBehaviour
{
    [SerializeField] private GameObject _image;
    private Transform _target;
    private void Start()
    {
        _target = Camera.main.transform; 
    }
    private void Update()
    {
        _image.transform.forward = _target.position - gameObject.transform.position;
    }
}
