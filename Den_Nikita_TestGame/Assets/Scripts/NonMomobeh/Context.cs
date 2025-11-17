using Fsm.UIApp;
using UnityEngine;

public class Context : MonoBehaviour
{
    public static Context Instance { get; private set; }

    public IAppSystem UIApp = new UIApp();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
