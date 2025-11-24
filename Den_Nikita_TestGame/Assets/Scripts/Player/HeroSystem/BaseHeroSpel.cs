using UnityEngine;

public abstract class  BaseHeroSpel : MonoBehaviour
{
    [SerializeField] protected Sprite _sprite;
    public abstract void Cast();
}
