using UnityEditor;
using UnityEngine;

public class MainMenuPanel1 : MonoBehaviour
{
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
}
