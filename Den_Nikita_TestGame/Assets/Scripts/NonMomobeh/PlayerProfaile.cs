
using Mono.Cecil.Cil;
using System.Collections.Generic;

public class PlayerProfaile
{
    public string Name { get; private set; }
    public string Pasword { get; private set; }
    public int ID { get; private set; }
    public int Lewl { get; private set; }

    private int _curreuntHeroes;
    private const int _maxHeroes = 4;

    private readonly List<BaseHero> _heroes = new List<BaseHero>();
    public int UnlockedHeroSlots => _curreuntHeroes;
    public IReadOnlyList<BaseHero> Heroes => _heroes;
    private BaseHero _currentHero;

    public void InitNewProfail(string name, string pasword, BaseHero hero)
    {
        Name = name;
        Pasword = pasword;
        ID = 1;
        Lewl = 1;

        _curreuntHeroes = 1;
        _heroes.Clear();
        _heroes.Add(hero);
        _currentHero = (hero);
    }
    public void GetProff(string name, int id, int lewl)
    {

    }
    public void AddHeroSlot()
    {
        if (_curreuntHeroes < _maxHeroes)
        {
            _curreuntHeroes++;
        }
    }
    public bool TryAddHero(BaseHero hero)
    {
        if (hero == null)
            return false;

        if (_heroes.Count >= _curreuntHeroes)
            return false;

        if (_heroes.Contains(hero))
            return false;

        _heroes.Add(hero);
        return true;
    }


    public bool RemoveHero(BaseHero hero)
    {
        if (hero == null)
            return false;

        return _heroes.Remove(hero);
    }

    public bool RemoveHeroAt(int index)
    {
        if (index < 0 || index >= _heroes.Count)
            return false;

        _heroes.RemoveAt(index);
        return true;
    }
    public BaseHero GetHero()
    {
        return _currentHero;
    }
    public BaseHero ChengeHero(BaseHero newHero)
    {
        foreach (var hero in _heroes)
        {
            if (newHero == hero)
            {
                _currentHero = newHero;
               return _currentHero;
            }
        }
        return null;
    }
}

