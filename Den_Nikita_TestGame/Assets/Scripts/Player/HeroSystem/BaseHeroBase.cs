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
    protected abstract Rigidbody HeroRigidbody { get; }

    [Header("Spels")]
    [SerializeField] private BaseHeroSpel[] _spels;

    private GameObject _player;

    protected void Awake()
    {
        PlayerInit();

    }
    public void InitRb()
    {
        _player.GetComponent<Movements>().GetRB(HeroRigidbody);
    }
    private void PlayerInit()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        GetComponent<PlayerAnim>().MyInput = _player.GetComponent<InputSystem>();

    }

    public virtual void Action(int index)
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
