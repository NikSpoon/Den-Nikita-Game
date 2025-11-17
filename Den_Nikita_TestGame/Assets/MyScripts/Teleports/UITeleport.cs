using UnityEngine;

public class UITeleport : MonoBehaviour
{
    public void ToMainMenu()
    {
        Context.Instance.UIApp.Trigger(Fsm.UIApp.AppTriger.ToMainMenu);
    }
    public void ToSave()
    {
        Context.Instance.UIApp.Trigger(Fsm.UIApp.AppTriger.ToSave);
    }
    public void To2DGame()
    {
        Context.Instance.UIApp.Trigger(Fsm.UIApp.AppTriger.ToGame2D);
    }
    public void ToFinish()
    {
        Context.Instance.UIApp.Trigger(Fsm.UIApp.AppTriger.ToFinish);
    }
    public void To3DGame()
    {
        Context.Instance.UIApp.Trigger(Fsm.UIApp.AppTriger.ToGame3D);
    }
    public void Exit()
    {
        gameObject.SetActive(false);
    }
}
