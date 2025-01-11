using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Animation Settings")]
    [SerializeField] private float minAnimationSpeed = 0.3f;
    [SerializeField] private float animationSpeedMultiplier = 0.2f;
    [SerializeField] private float runThreshold = 0.4f;
    
    private static readonly int IsRunHash = Animator.StringToHash("isRun");
    private static readonly int IsJumpHash = Animator.StringToHash("isJump");
    
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private float _horizontalInput;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();    
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 점프
        if (Input.GetButtonDown("Jump") && !_animator.GetBool(IsJumpHash))
        {
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            _animator.SetBool(IsJumpHash, true);
        }
        
        // 입력에 따라 Sprite 좌우 반전
        if (Input.GetButton("Horizontal"))
        {
            _spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        // 입력에 따라 애니메이션 속도 조절
        if (Mathf.Abs(_rigidbody.linearVelocityX) < runThreshold)
        {
            _animator.SetBool(IsRunHash, false);
            _animator.speed = minAnimationSpeed;
        } else {
            _animator.SetBool(IsRunHash, true);
            _animator.speed = _rigidbody.linearVelocity.magnitude * animationSpeedMultiplier;
        }
    }

    void FixedUpdate()
    {
        // 좌우 이동
        float y = Input.GetAxisRaw("Horizontal");
        
        _rigidbody.AddForce(Vector2.right * y, ForceMode2D.Impulse);

        if (_rigidbody.linearVelocityX > maxSpeed)
        {
            _rigidbody.linearVelocity = new Vector2(maxSpeed, _rigidbody.linearVelocityY);
        } else if (_rigidbody.linearVelocityX < -maxSpeed)
        {
            _rigidbody.linearVelocity = new Vector2(-maxSpeed, _rigidbody.linearVelocityY);
        }
        
        // 점프 후 땅에 닿았는지 체크
        if (_rigidbody.linearVelocityY < 0)
        {
            Debug.DrawRay(_rigidbody.position, Vector3.down, Color.green);
            RaycastHit2D rayHit = Physics2D.Raycast(_rigidbody.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    _animator.SetBool(IsJumpHash, false);
                }
            }
        }
    }
}
