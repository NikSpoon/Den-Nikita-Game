using TMPro;
using UnityEngine;

public class UILoader : MonoBehaviour
{
    [SerializeField] private TMP_InputField _load;
    [SerializeField] private TMP_InputField _pasword;

    [SerializeField] private GameObject _error;
    public void load()
    {
        if (ChekInputFild())
        {
            Context.Instance.PlayerProfaile.InitNewProfail(_load.text, _pasword.text, Context.Instance.Data.HeroDatabase.Heroes[0]);
            GoToMeinMenu();
        }
        else
        {
            _error.SetActive(true);
        }
    }
    public void TesterLoad()
    {
        Context.Instance.PlayerProfaile.InitNewProfail("TesterDima", "1", Context.Instance.Data.HeroDatabase.Heroes[0]);
        GoToMeinMenu();
    }
    private void GoToMeinMenu()
    {
        Context.Instance.UIApp.Trigger(Fsm.UIApp.AppTriger.ToMainMenu);

    }
    private bool ChekInputFild()
    {
        if (_load != null && _pasword != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
