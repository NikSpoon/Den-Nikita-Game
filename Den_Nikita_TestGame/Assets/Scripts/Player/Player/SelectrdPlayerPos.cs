using UnityEngine;

public class SelectrdPlayerPos : MonoBehaviour
{
    private Transform _targetHrto;
    public void InitHero(Transform hero)
    {
        _targetHrto =  hero ;
    }
    private void Update()
    {
        if (_targetHrto != null)
        {
            transform.position = _targetHrto.position;
        }
    }
}
