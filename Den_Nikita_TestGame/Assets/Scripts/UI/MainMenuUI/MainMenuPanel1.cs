using UnityEditor;
using UnityEngine;

public class MainMenuPanel1 : MonoBehaviour
{
    [SerializeField] private GameObject _options;
    [SerializeField] private GameObject _multiplayer;
    [SerializeField] private GameObject _myPlayre;
    [SerializeField] private GameObject _myWord;
    public void StatGame()
    {
        Context.Instance.UIApp.Trigger(Fsm.UIApp.AppTriger.ToGame3D);
    }
    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void OpenOptions()
    {
        _options.SetActive(true);
    }

    public void OpenMultiplayer()
    {
        _multiplayer.SetActive(true);
    }

    public void OpenMyPlayer()
    {
        _myPlayre.SetActive(true);
    }

    public void OpenMyWorld()
    {
        _myWord.SetActive(true);
    }
}
