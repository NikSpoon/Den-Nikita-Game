using UnityEngine;

public class Humen : BaseHero
{

    public void Start()
    {
        _player.GetComponent<SelectrdPlayerPos>().InitHero(gameObject.transform);
    }
    public override void Collect()
    {
        Debug.Log("Humen собирает ресурсы");
        Action(0);
    }

    public override void LightAttack()
    {
        Debug.Log("Humen лёгкая атака");
    }

    public override void HeavyAttack()
    {
        Debug.Log("Humen тяжёлая атака");
    }

    public override void SpecialAttack()
    {
        Debug.Log("Humen особая способность");
    }
}
