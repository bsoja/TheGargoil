using System.Collections;
using UnityEngine;

public class GargoilController : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _renderer;
    private CharacterData _gargoil;

    public float _attackDelay;
    public float _x;
    public float _y;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _gargoil = GetComponent<CharacterData>();
    }

    // Update is called once per frame
    void Update()
    {
        _x = transform.position.x;
        _y = transform.position.y;

        if (_gargoil.IsDead)
        {
            _gargoil.State = CharacterState.Dead;
            return;
        }

        if(_gargoil.State == CharacterState.Attacking)
        {
            return;
        }

        if (_attackDelay >= 0)
        {
            _attackDelay -= Time.deltaTime * _gargoil.Speed;
        }

        _renderer.flipX = false;

        var movX = Input.GetAxisRaw("Horizontal");
        var movY = Input.GetAxisRaw("Vertical");

        transform.position += new Vector3(movX, movY, 0) * _gargoil.Speed * Time.deltaTime;

        if (movY > 0)
        {
            _gargoil.State = CharacterState.MovingUp;
        }
        else if (movY < 0)
        {
            _gargoil.State = CharacterState.MovingDown;
        }
        else if (movX > 0)
        {
            _gargoil.State = CharacterState.MovingRight;
            _gargoil.AnimationIsPreviousLeft = false;
        }
        else if (movX < 0)
        {
            _gargoil.State = CharacterState.MovingLeft;
            _gargoil.AnimationIsPreviousLeft = true;
        }
        else
        {
            _gargoil.State = CharacterState.Idle;
        }

        if (Input.GetKey(KeyCode.Space) && _attackDelay <= 0)
        {
            _gargoil.State = CharacterState.Attacking;
            _renderer.flipX = !_gargoil.AnimationIsPreviousLeft;
            _attackDelay = 1;
        }

        if (_gargoil.State == CharacterState.Attacking)
        {
            StartCoroutine("OnCompleteAttackAnimation");
        }
    }

    private void OnAttackEnd()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(var enemy in enemies)
        {
            var enemyBounds = enemy.GetComponent<SpriteRenderer>().bounds;
            if (_renderer.bounds.Intersects(enemyBounds))
            {
                enemy.GetComponent<CharacterData>().Hit(_gargoil);
            }
        }
    }

    IEnumerator OnCompleteAttackAnimation()
    {
        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.99f )
        {
            yield return null;
        }
        
        OnAttackEnd();
        _gargoil.State = CharacterState.Idle;
    }
}