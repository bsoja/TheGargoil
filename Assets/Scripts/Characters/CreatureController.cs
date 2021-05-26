using System;
using System.Collections;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _renderer;
    private CharacterData _creature;
    
    public Vector3 MoveTarget;

    public float _attackDelay;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _creature = GetComponent<CharacterData>();
    }

    public enum CharacterDecission
    {
        Idle, // move around slowly
        Alert, // hold on and listen
        MoveToHome, // saw enemy, but he's gone, go back home
        MoveToEnemy, // saw enemy and moving to crash his head
        Attack, // attack enemy, in range of enemy
        
    }
    private CharacterDecission MakeDecission()
    {
        if (_creature.Player.GetComponent<CharacterData>().IsDead)
        {
            return CharacterDecission.Idle;
        }

        var enemyBounds = _creature.Player.GetComponent<SpriteRenderer>().bounds;

        var distance = Vector3.Distance(transform.position, _creature.Player.transform.position);

        if (distance > 8)
        {
            var homeDistance = Vector3.Distance(_creature.InitialPosition, transform.position);
            if (homeDistance <= 1f)
            {
                return CharacterDecission.Idle;
            }
            else
            {
                MoveTarget = _creature.InitialPosition;
                return CharacterDecission.MoveToHome;
            }
        }
        else if (distance > 6 && distance <= 8)
        {
            return CharacterDecission.Alert;
        }
        else
        {
            if (enemyBounds.Intersects(_renderer.bounds)) 
            {
                return CharacterDecission.Attack;
            }
            else
            {
                MoveTarget = enemyBounds.center;
                return CharacterDecission.MoveToEnemy;
            }
        }
    }

    void Update()
    {
        if (_creature.IsDead)
        {
            _creature.State = CharacterState.Dead;
            return;
        }

        if (_creature.State == CharacterState.Attacking)
        {
            return;
        }

        if (_attackDelay >= 0)
        {
            _attackDelay -= Time.deltaTime * _creature.Speed;
        }

        _renderer.flipX = false;

        var decission = MakeDecission();

        switch (decission)
        {
            case CharacterDecission.Idle:
                {
                    //TODO: move slowly around initial area
                    _creature.State = CharacterState.Idle;
                    break;
                }
            case CharacterDecission.Alert:
                {
                    //NOTE: stop and listen
                    _creature.State = CharacterState.Idle;
                    break;
                }
            case CharacterDecission.MoveToEnemy:
                {
                    MoveCharacter(MoveTarget, _creature.Speed);
                    break;
                }
            case CharacterDecission.MoveToHome:
                {
                    MoveCharacter(MoveTarget, _creature.IdleSpeed);
                    break;
                }
            case CharacterDecission.Attack:
                {
                    if (_attackDelay <= 0)
                    {
                        _renderer.flipX = !_creature.AnimationIsPreviousLeft;
                        _creature.State = CharacterState.Attacking;
                        _attackDelay = 1;
                    }
                    break;
                }
        }

        if (_creature.State == CharacterState.Attacking)
        {
            StartCoroutine("OnCompleteAttackAnimation");
        }
    }

    private void MoveCharacter(Vector3 target, float speed)
    {
        var deltaX = target.x - transform.position.x;
        var deltaY = target.y - transform.position.y;

        float movX = 0f;
        float movY = 0f;

        if (Math.Abs(deltaX) > Math.Abs(deltaY))
        {
            if (deltaX > 1)
            {
                movX = 1;
            }
            else if (deltaX < 1)
            {
                movX = -1;
            }
        }
        else
        {
            if (deltaY > 1)
            {
                movY = 1;
            }
            else if (deltaY < 1)
            {
                movY = -1;
            }
        }

        transform.position += new Vector3(movX, movY, 0) * speed * Time.deltaTime;

        if (movY > 0)
        {
            _creature.State = CharacterState.MovingUp;
        }
        else if (movY < 0)
        {
            _creature.State = CharacterState.MovingDown;
        }
        else if (movX > 0)
        {
            _creature.State = CharacterState.MovingRight;
            _creature.AnimationIsPreviousLeft = false;
        }
        else if (movX < 0)
        {
            _creature.State = CharacterState.MovingLeft;
            _creature.AnimationIsPreviousLeft = true;
        }
        else
        {
            _creature.State = CharacterState.Idle;
        }
    }

    private void OnAttackEnd()
    {
        var playerBounds = _creature.Player.GetComponent<SpriteRenderer>().bounds;
        if (_renderer.bounds.Intersects(playerBounds))
        {
            _creature.Player.GetComponent<CharacterData>().Hit(_creature);
        }
    }

    IEnumerator OnCompleteAttackAnimation()
    {
        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.99f)
        {
            yield return null;
        }

        OnAttackEnd();
        _creature.State = CharacterState.Idle;
    }
}
