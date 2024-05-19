using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformationController : MonoBehaviour
{
    [SerializeField] bool _enableVapor = false;
    [SerializeField] float _waterCost;
    [SerializeField] float _transformationDelay;

    [Header("Normal form")]
    [SerializeField] float _normalJumpForce;
    [SerializeField] float _normalSpeed;
    [SerializeField] float _normalGravity;

    [Header("Water form")]
    [SerializeField] float _waterJumpForce;
    [SerializeField] float _waterSpeed;

    [Header("Vapor form")]
    [SerializeField] float _vaporJumpForce;
    [SerializeField] float _vaporSpeed;
    [SerializeField] float _vaporGravity;

    [HideInInspector] public  PlayerForm CurrentForm = PlayerForm.NORMAL;
    bool _isTransforming = false;

    Rigidbody2D _rb;
    SpriteRenderer _playerSprite;
    Collider2D _collider;
    Animator _animator;
    
    PlayerMovementController _movementControllerScr;
    WaterMeter _waterMeterScr;
    TongueHook _tongueHookScr;
    StealthController _stealthControllerScr;

    public enum PlayerForm
    {
        NORMAL, WATER, VAPOR
    }


    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerSprite = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _animator = GetComponentInChildren<Animator>();

        _movementControllerScr = GetComponent<PlayerMovementController>();
        _tongueHookScr = GetComponent<TongueHook>();
        _waterMeterScr = GetComponent<WaterMeter>();
        _stealthControllerScr = GetComponent<StealthController>();
    }

    private void Start()
    {
        TurnNormal();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HotZone")) _enableVapor = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("HotZone"))
        {
            _enableVapor = false;
            TurnNormal();
        }
    }

    public void TurnNormal()
    {
        CurrentForm = PlayerForm.NORMAL;
        _animator.SetTrigger("TransformNormal");

        _movementControllerScr.JumpForce = _normalJumpForce;
        _movementControllerScr.MaxSpeed = _normalSpeed;
        _rb.gravityScale = _normalGravity;
        _collider.enabled = true;

        _playerSprite.color = new Color(_playerSprite.color.r, _playerSprite.color.g, _playerSprite.color.b, 1);
    }

    public void TurnToWater()
    {
        CurrentForm = PlayerForm.WATER;
        _animator.SetTrigger("TransformWater");

        _movementControllerScr.JumpForce = _waterJumpForce;
        _movementControllerScr.MaxSpeed = _waterSpeed;
        _collider.enabled = true;

        _playerSprite.color = new Color(_playerSprite.color.r, _playerSprite.color.g, _playerSprite.color.b, 1);
    }

    public void TurnToVapor()
    {
        CurrentForm = PlayerForm.VAPOR;
        _animator.SetTrigger("TransformSteam");

        _movementControllerScr.JumpForce = _vaporJumpForce;
        _movementControllerScr.MaxSpeed = _vaporSpeed;
        _rb.gravityScale = _vaporGravity;
        _collider.enabled = false;

        _playerSprite.color = new Color(_playerSprite.color.r, _playerSprite.color.g, _playerSprite.color.b, .6f);
    }

    public void OnTransform()
    {
        if (!this.enabled || !_movementControllerScr.IsGrounded() || _tongueHookScr.IsCasting || _tongueHookScr.AwaitingJump || 
            _isTransforming || _movementControllerScr.IsCrouched) return;

        if (CurrentForm != PlayerForm.NORMAL)
        {
            TurnNormal();
            return; //Avoid water cost when turning back to normal
        }
        else
        {
            StartCoroutine(TransformAfterDelay());
        }

    }

    private IEnumerator TransformAfterDelay()
    {
        _isTransforming = true;
        _movementControllerScr.enabled = false;
        _tongueHookScr.enabled = false;
        _rb.velocity = Vector2.zero;
        _animator.SetFloat("XSpeed", _rb.velocity.x);
        _animator.SetFloat("YSpeed", _rb.velocity.y);

        yield return new WaitForSeconds(_transformationDelay);

        if (!_enableVapor)
        {
            TurnToWater();
        }
        else
        {
            TurnToVapor();
        }

        _waterMeterScr.DrainWater(_waterCost);

        _isTransforming = false;
        _movementControllerScr.enabled = true;
        _tongueHookScr.enabled = true;
        transform.right = _movementControllerScr.CurrentDirection;
    }
}
