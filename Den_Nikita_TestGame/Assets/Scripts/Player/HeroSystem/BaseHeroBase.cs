using NUnit.Framework;
using System;
using UnityEngine;

public abstract class BaseHero : MonoBehaviour
{
    [Header("Base Hero Data")]
    [SerializeField] protected Sprite _heroSprite;
    [SerializeField] protected GameObject _heroPrefab;

    [Header("Movement")]
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _jumpForce;

    [Header("Physics")]
    [SerializeField] protected float _mass;

    [Header("Spels")]
    [SerializeField] private BaseHeroSpel[] _spels;

    protected GameObject _player;

    protected void Awake()
    {
        PlayerInit();
    }

    public  void PlayerInit()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public  void Action(int index)
    {
        if (index < 0 || index >= _spels.Length) return;
        if (_spels[index] == null) return;

        _spels[index].Cast();
    }
    
    public abstract void Collect();
    public abstract void LightAttack();
    public abstract void HeavyAttack();
    public abstract void SpecialAttack();
 

}
