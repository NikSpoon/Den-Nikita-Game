using UnityEngine;

public class TestExitOnePanel : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    public void ExitPanel()
    {
        _panel.SetActive(false);
    }
}
