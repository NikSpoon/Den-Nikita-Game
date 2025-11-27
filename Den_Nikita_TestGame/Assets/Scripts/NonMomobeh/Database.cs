using UnityEngine;
public class Database : MonoBehaviour
{
    [SerializeField] private HeroDatabase _heroDatabase;
    public HeroDatabase HeroDatabase => _heroDatabase;
}
