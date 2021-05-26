using System;
using System.Collections;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _renderer;
    private BoxCollider2D _boxCollider;

    private Vector3 _initialPosition;
    public CharacterState _state;

    public float Health = 100;
    public int Strength = 40;
    public float Speed = 5;
    public float IdleSpeed = 5;

    public bool AnimationIsPreviousLeft { get; set; }

    public bool IsDead
    {
        get { return Health <= 0; }
    }

    public Vector3 InitialPosition
    {
        get { return _initialPosition; }
    }

    public Bounds Bounds
    {
        get { return _renderer.bounds; }
    }

    public GameObject Player { get; private set; }

    public CharacterState State
    {
        get { return _state; }
        set
        {
            _state = value;
            _animator.SetInteger("CharacterState", (int)_state);
        }
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();

        _initialPosition = transform.position;
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Hit(CharacterData enemy)
    {
        bool isHit = UnityEngine.Random.Range(0, 2) > 0;
        if (isHit)
        {
            var hitPower = UnityEngine.Random.Range(1, enemy.Strength);
            Health -= hitPower;
            if (IsDead)
            {
                _boxCollider.enabled = false;
                return;
            }
            StartCoroutine("CollideFlash");
        }
    }

    IEnumerator CollideFlash()
    {
        _renderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _renderer.color = Color.white;

        //Material m = _renderer.material;
        //Color32 c = _renderer.material.color;
        //_renderer.material = null;
        //_renderer.material.color = Color.red;
        //yield return new WaitForSeconds(0.1f);
        //_renderer.material = m;
        //_renderer.material.color = c;
    }
}
