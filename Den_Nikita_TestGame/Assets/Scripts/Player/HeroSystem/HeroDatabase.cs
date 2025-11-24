using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Hero Database")]
public class HeroDatabase : ScriptableObject
{
    [SerializeField] private List<BaseHero> _heroes = new List<BaseHero>();

    public IReadOnlyList<BaseHero> Heroes => _heroes;
}
