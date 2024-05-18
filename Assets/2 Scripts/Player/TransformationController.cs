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

        _movementControllerScr.JumpForce = _normalJumpForce;
        _movementControllerScr.MaxSpeed = _normalSpeed;
        _rb.gravityScale = _normalGravity;
        _collider.enabled = true;

        _playerSprite.color = new Color32(255, 255, 255, 255);
    }

    public void TurnToWater()
    {
        CurrentForm = PlayerForm.WATER;

        _movementControllerScr.JumpForce = _waterJumpForce;
        _movementControllerScr.MaxSpeed = _waterSpeed;
        _collider.enabled = true;

        _playerSprite.color = new Color32(100, 101, 231, 255);
    }

    public void TurnToVapor()
    {
        CurrentForm = PlayerForm.VAPOR;

        _movementControllerScr.JumpForce = _vaporJumpForce;
        _movementControllerScr.MaxSpeed = _vaporSpeed;
        _rb.gravityScale = _vaporGravity;
        _collider.enabled = false;

        _playerSprite.color = new Color32(144, 243, 233, 155);
    }

    public void OnTransform()
    {
        if (!this.enabled || !_movementControllerScr.IsGrounded() || _tongueHookScr.IsCasting || _tongueHookScr.AwaitingJump || _isTransforming) return;

        if (CurrentForm != PlayerForm.NORMAL)
        {
            TurnNormal();
            return; //Avoid water cost when turning back to normal
        }
        else if (!_enableVapor)
        {
            TurnToWater();
        }
        else
        {
            TurnToVapor();
        }

        _waterMeterScr.DrainWater(_waterCost);

        StartCoroutine(WaitForDelay());
    }

    private IEnumerator WaitForDelay()
    {
        _isTransforming = true;
        _movementControllerScr.enabled = false;
        _tongueHookScr.enabled = false;
        _rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(_transformationDelay);

        _isTransforming = false;
        _movementControllerScr.enabled = true;
        _tongueHookScr.enabled = true;
        transform.right = _movementControllerScr.CurrentDirection;
    }
}
