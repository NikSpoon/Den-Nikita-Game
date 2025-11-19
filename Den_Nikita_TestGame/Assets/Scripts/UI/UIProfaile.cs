using TMPro;
using UnityEngine;

public class UIProfaile : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _levl;
    [SerializeField] private TextMeshProUGUI _id;

    public void Start()
    {
        InitProf();
    }
    private void InitProf()
    {
        _name.text = Context.Instance.PlayerProfaile.Name;
        _levl.text =  Context.Instance.PlayerProfaile.Lewl.ToString();
        _id.text =  Context.Instance.PlayerProfaile.ID.ToString();
    }
}
