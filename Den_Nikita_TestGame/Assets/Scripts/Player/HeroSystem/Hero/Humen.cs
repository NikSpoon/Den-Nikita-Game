using UnityEngine;

public class Humen : BaseHero
{
    [SerializeField] private Rigidbody _rb;
    protected override Rigidbody HeroRigidbody => _rb;

    private new void Start()
    {
        InitRb();
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
