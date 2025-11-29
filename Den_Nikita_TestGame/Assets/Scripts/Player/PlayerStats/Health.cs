using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    public event Action<int, int> OnHealthChanged;

    private void Awake()
    {
        int heroMaxHealth = Context.Instance.PlayerProfale.GetHero().MaxHeals;

        if (_maxHealth < heroMaxHealth)
        {
            _maxHealth = heroMaxHealth;
            _currentHealth = heroMaxHealth;
        }
    }
    public void AddHealth(int value)
    {
        _currentHealth += value;
        InitInvoke();
    }

    public void SetHealth(int value)
    {
        _currentHealth = value;
        InitInvoke();
    }

    public void TakeDamage(int value)
    {
        _currentHealth -= value;
        InitInvoke();
    }
    public void InitInvoke()
    {
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }
   
}
