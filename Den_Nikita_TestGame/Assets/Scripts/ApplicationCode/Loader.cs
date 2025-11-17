using System.Xml.Schema;
using UnityEngine;

public class Loader : MonoBehaviour
{
    private void Start()
    {
        var app = Context.Instance.UIApp;
        app.Trigger(Fsm.UIApp.AppTriger.ToMainMenu);
    }
}
