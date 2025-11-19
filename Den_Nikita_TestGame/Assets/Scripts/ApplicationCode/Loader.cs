using System.Collections;
using System.Xml.Schema;
using UnityEngine;

public class Loader : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(WheitContect());
    }
   
    private IEnumerator WheitContect()
    {
        while (Context.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        var app = Context.Instance.UIApp;
       // app.Trigger(Fsm.UIApp.AppTriger.ToMainMenu);
    }
}
