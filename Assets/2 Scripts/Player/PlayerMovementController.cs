using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    public float MaxSpeed;
    public float JumpForce;
    [SerializeField] CapsuleCollider2D _collider;
    [SerializeField] CapsuleCollider2D _trigger;
    [SerializeField] LayerMask _groundedMask;

    [HideInInspector] public Vector2 CurrentDirection;
    float _direction;
    bool _onPuddle = false;
    public bool IsCrouched = false;
    bool _wantsToStand = false;
    Vector2 _colliderSize;
    Vector2 _triggerSize;

    Rigidbody2D _rb;
    Animator _animator;
    TransformationController _transformControllerScr;
    TongueHook _tongueHookScr;
    StealthController _stealthControllerScr;


    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _transformControllerScr = GetComponent<TransformationController>();
        _tongueHookScr = GetComponent<TongueHook>();
        _stealthControllerScr = GetComponent<StealthController>();
    }

    private void Start()
    {
        _colliderSize = _collider.size;
        _triggerSize = _trigger.size;
    }

    // Update is called once per frame
    void Update()
    {
        ManageMovement();
        ManageStandingUp();
        SetAnimations();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Puddle")) _onPuddle = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Puddle")) _onPuddle = false;
    }

    private void ManageMovement()
    {
        _rb.velocity = new Vector2(_direction * MaxSpeed, _rb.velocity.y);
    }

    public void OnRun(InputValue value)
    {
        _direction = value.Get<float>();

        if (_direction > 0) CurrentDirection = Vector2.right;
        else if (_direction < 0) CurrentDirection = Vector2.left;

        if (!this.enabled) return;
        transform.right = CurrentDirection;
    }

    public void OnJump()
    {
        if (IsGrounded() && this.enabled)
        {
            _rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        }
    }

    public void OnCrouch(InputValue value)
    {
        if (!this.enabled || _transformControllerScr.CurrentForm != TransformationController.PlayerForm.WATER || 
            !IsGrounded() || _tongueHookScr.IsCasting || _tongueHookScr.AwaitingJump) return;
        if (_onPuddle && value.isPressed)
        {
            _stealthControllerScr.HideInWater();
        }
        else if (value.isPressed && !IsCrouched)
        {
            Vector2 colliderNewSize = new(_collider.size.x - .05f, (_collider.size.y * .5f) - .05f);
            Vector2 triggerNewSize = new(_trigger.size.x - .10f, (_trigger.size.y * .5f) - .10f);

            _collider.offset = new(_collider.offset.x, _collider.offset.y - (_colliderSize.y - colliderNewSize.y) * .5f);
            _trigger.offset = new(_trigger.offset.x, _trigger.offset.y - (_triggerSize.y - triggerNewSize.y)*.5f);
            _collider.size = colliderNewSize;
            _trigger.size = triggerNewSize;

            IsCrouched = true;
            //_animator.SetTrigger("Crouch");
            _animator.SetBool("IsCrouching", true);
        }
        else if (IsCrouched && CanStand() && !value.isPressed)
        {
            StandUp();
        }
        else if (IsCrouched && !value.isPressed)
        {
            _wantsToStand = true;
        }
    }

    private void StandUp()
    {
        _collider.offset = Vector2.zero;
        _trigger.offset = Vector2.zero;
        _collider.size = _colliderSize;
        _trigger.size = _triggerSize;

        IsCrouched = false;
        _wantsToStand = false;
        //_animator.SetTrigger("Stand");
        _animator.SetBool("IsCrouching", false);
    }

    private void ManageStandingUp()
    {
        if (_wantsToStand && CanStand()) StandUp();
    }

    public bool IsGrounded()
    {
        Vector2 origin = new(_collider.bounds.center.x, _collider.bounds.min.y);
        RaycastHit2D rayCastHit = Physics2D.CircleCast(origin, _collider.size.x * .5f, Vector2.down, .1f, _groundedMask);
        //RaycastHit2D rayCastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, groundCheckDirection, extraHeight, groundLayer);

        //Debug.Log(rayCastHit.collider);
        bool isGrounded = rayCastHit.collider != null;

        return isGrounded;
    }

    private bool CanStand()
    {
        RaycastHit2D rayCastMax = Physics2D.Linecast(_collider.bounds.max, new(_collider.bounds.max.x, _collider.bounds.max.y + .55f), _groundedMask);
        RaycastHit2D rayCastMin = Physics2D.Linecast(new(_collider.bounds.min.x, _collider.bounds.max.y), new(_collider.bounds.min.x, _collider.bounds.max.y + .55f), _groundedMask);
        bool result = (rayCastMax.collider == null && rayCastMin.collider == null);
        return result;
    }

    private void SetAnimations()
    {
        _animator.SetFloat("XSpeed", Mathf.Abs(_rb.velocity.x));
        _animator.SetFloat("YSpeed", _rb.velocity.y);
        _animator.SetBool("IsGrounded", IsGrounded());
    }

    private void OnDrawGizmosSelected()
    {
        Color red = new Color(1, 0, 0, .3f);
        Color green = new Color(0, 1, 0, .3f);

        Vector2 origin = new(_collider.bounds.center.x, _collider.bounds.min.y);

        if (IsGrounded()) Gizmos.color = green;
        else              Gizmos.color = red;
        Gizmos.DrawSphere(origin, _collider.size.x * .5f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(_collider.bounds.max, new(_collider.bounds.max.x, _collider.bounds.max.y + .55f));
        Gizmos.DrawLine(new(_collider.bounds.min.x, _collider.bounds.max.y), new(_collider.bounds.min.x, _collider.bounds.max.y + .55f));

    }
}
