using UnityEngine;
public class Database
{
    [SerializeField] private HeroDatabase _heroDatabase;
    public HeroDatabase HeroDatabase => _heroDatabase;
}
