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
        _name.text = Context.Instance.PlayerProfale.Name;
        _levl.text =  Context.Instance.PlayerProfale.Lewl.ToString();
        _id.text =  Context.Instance.PlayerProfale.ID.ToString();
    }

}
