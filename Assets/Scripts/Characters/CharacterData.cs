using UnityEngine;

public class CharacterData : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _renderer;

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

        _initialPosition = transform.position;
        Player = GameObject.FindGameObjectWithTag("Player");
    }
}
