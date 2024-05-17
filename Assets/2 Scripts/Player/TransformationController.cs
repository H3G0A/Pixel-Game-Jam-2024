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

    PlayerForm _currentForm = PlayerForm.NORMAL;
    Rigidbody2D _rb;
    PlayerMovementController _movementControllerScr;
    WaterMeter _waterMeterScr;
    TongueHook _tongueHookScr;

    SpriteRenderer _playerSprite;

    public enum PlayerForm
    {
        NORMAL, WATER, VAPOR
    }


    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _movementControllerScr = GetComponent<PlayerMovementController>();
        _tongueHookScr = GetComponent<TongueHook>();
        _waterMeterScr = GetComponent<WaterMeter>();

        _playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        TurnNormal();
    }

    public void TurnNormal()
    {
        _currentForm = PlayerForm.NORMAL;

        _movementControllerScr.JumpForce = _normalJumpForce;
        _movementControllerScr.MaxSpeed = _normalSpeed;
        _rb.gravityScale = _normalGravity;

        _playerSprite.color = new Color32(209, 204, 70, 255);
    }

    public void TurnToWater()
    {
        _currentForm = PlayerForm.WATER;

        _movementControllerScr.JumpForce = _waterJumpForce;
        _movementControllerScr.MaxSpeed = _waterSpeed;

        _playerSprite.color = new Color32(100, 101, 231, 255);
    }

    public void TurnToVapor()
    {
        _currentForm = PlayerForm.VAPOR;

        _movementControllerScr.JumpForce = _vaporJumpForce;
        _movementControllerScr.MaxSpeed = _vaporSpeed;
        _rb.gravityScale = _vaporGravity;

        _playerSprite.color = new Color32(144, 243, 233, 255);
    }

    public void OnTransform()
    {
        if (!this.enabled || !_movementControllerScr.IsGrounded() || _tongueHookScr.IsCasting || _tongueHookScr.AwaitingJump) return;

        if (_currentForm != PlayerForm.NORMAL)
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
        _movementControllerScr.enabled = false;
        _tongueHookScr.enabled = false;
        _rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(_transformationDelay);

        _movementControllerScr.enabled = true;
        _tongueHookScr.enabled = true;
        transform.right = _movementControllerScr.CurrentDirection;
    }
}
