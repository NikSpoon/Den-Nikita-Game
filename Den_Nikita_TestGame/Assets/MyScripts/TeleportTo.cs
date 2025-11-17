using UnityEngine;

public class TeleportTo : MonoBehaviour
{
    private GameObject _toMainMenuPanel;
    private void Start()
    {
        _toMainMenuPanel = GameObject.FindGameObjectWithTag("ToMeinMenuTeleport");
        _toMainMenuPanel.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            // Debug.Log("Игрок вошел в триггер!");
            _toMainMenuPanel.SetActive(true);
          
          
        } 
    }
}
