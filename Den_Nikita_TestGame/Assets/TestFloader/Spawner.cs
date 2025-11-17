using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _root;
    private void Awake()
    {
        if (_player != null)
        {
          var player =  Instantiate(_player, _root.position,_root.rotation,null);
    
        }
    }
}
