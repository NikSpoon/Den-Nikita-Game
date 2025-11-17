using Fsm.UIApp;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private Transform _root;

    [SerializeField] private GameObject _loaderPanel;
    
    [SerializeField] private GameObject _loading;
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _game3D;
    [SerializeField] private GameObject _game2D;
    [SerializeField] private GameObject _finish;
    [SerializeField] private GameObject _save;

    private GameObject _currentScreen;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (FindObjectsByType<UIController>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

    
    }
    private void OnEnable()
    {
        _currentScreen = Instantiate(_loading, _root.transform);
       
        var appSystem = Context.Instance.UIApp;
        Context.Instance.UIApp.OnStateChange += OnStateChange;
            
    }
    private void OnStateChange(StateChangeData<AppState, AppTriger> data)
    {
        if (_currentScreen != null)
        {
            Destroy(_currentScreen);
        }
        switch (data.NewState)
        {

            case AppState.MainMenu: _currentScreen = Instantiate(ReloadScene("MainMenu",_mainMenu),_root.transform); break;

            case AppState.Game3D: _currentScreen = Instantiate(ReloadScene("Test3DMap", _game3D), _root.transform); break;

            case AppState.Game2D: _currentScreen = Instantiate(ReloadScene("Test2DMap", _game2D), _root.transform); break;

            case AppState.Finish: _currentScreen = Instantiate(ReloadScene("Finish", _finish), _root.transform); break;

            case AppState.Save: _currentScreen = Instantiate(ReloadScene("Save", _save), _root.transform); break;

            default:
                break;

        }
    }



    private GameObject ReloadScene(string newScene, GameObject neme)
    {
        var load = Instantiate(_loaderPanel);
        SceneManager.LoadScene(newScene);
        Destroy(load);
        return neme;
    }
}

