using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class MainMenuPanel1 : MonoBehaviour
{
    [SerializeField] private InputSystem _inputSystem;

    [Header("MainPanels")]
    [SerializeField] private GameObject _options;
    [SerializeField] private GameObject _multiplayer;
    [SerializeField] private GameObject _myPlayre;
    [SerializeField] private GameObject _myWord;
    [SerializeField] private GameObject _exitPanel;
    [Header("MultiplayerPanels")]
    [SerializeField] private GameObject _multiplayerOpenWorld;
    [SerializeField] private GameObject _multiplayerCreateWorld;
    [SerializeField] private GameObject _multiplayerHeroPanel;


    private GameObject _currentGameOb;
    private List<GameObject> _activePanels = new List<GameObject>();
    private void AddActivePanels(GameObject newCurrentPanel)
    {
        newCurrentPanel.SetActive(true);
        _currentGameOb = newCurrentPanel;
        _activePanels.Add(_currentGameOb);
    }
    public void ExitCurrentPanel()
    {
        if (_activePanels.Count == 0)
        {
            _exitPanel.SetActive(true);
            return;
        }

        if (_currentGameOb.activeSelf)
        {
            _currentGameOb.SetActive(false);
            _activePanels.Remove(_currentGameOb);
            _currentGameOb = _activePanels[_activePanels.Count - 1];
            
        }
        else
        {
            _exitPanel.SetActive(true);
        }
    }

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
        AddActivePanels(_options);
        
    }

    public void OpenMultiplayer()
    {
        AddActivePanels(_multiplayer);
    }

    public void OpenMyPlayer()
    {
        AddActivePanels(_myPlayre);
    }

    public void OpenMyWorld()
    {
        AddActivePanels(_myWord);
    }
    public void OpenMultiplayerMyWorld()
    {
        if (_multiplayer.activeSelf)
        {
            AddActivePanels(_multiplayerOpenWorld);
        }
    }
    public void OpenMultiplayerCreteWorld()
    {
        if (_multiplayer.activeSelf)
        {
            AddActivePanels(_multiplayerCreateWorld);
        }
    }
    public void OpenMultiplayerMyHero()
    {
        if (_multiplayer.activeSelf)
        {
            AddActivePanels(_multiplayerHeroPanel); 
        }
    }
    private void Update()
    {
        if (_inputSystem.Exit)
        {
            ExitCurrentPanel();
        }
    }
}
