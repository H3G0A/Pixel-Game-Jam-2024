using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueHook : MonoBehaviour
{
    [SerializeField] float _range;
    [SerializeField] float _tongueSpeed;
    [SerializeField] float _pullSpeed;
    [SerializeField] float _tongueCd;
    [SerializeField] float _wallJumpForce;
    [SerializeField] float _waterCost;
    [SerializeField] LayerMask _collisionMask;
    [SerializeField] GameObject _tongue;

    [SerializeField] CapsuleCollider2D _collider;
    Rigidbody2D _rb;
    Animator _animator;
    
    PlayerMovementController _movementControllerScr;
    WaterMeter _waterMeterScr;

    [HideInInspector] public bool IsCasting = false;
    [HideInInspector] public bool AwaitingJump = false;

    float _gScale;
    float _initialTongueLength;
    Vector3 _lookDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponentsInChildren<CapsuleCollider2D>()[1];
        _animator = GetComponentInChildren<Animator>();
        _movementControllerScr = GetComponent<PlayerMovementController>();
        _waterMeterScr = GetComponent<WaterMeter>();
    }

    private void Start()
    {
        _tongue.SetActive(false);
        _gScale = _rb.gravityScale;
    }

    public void OnHook()
    {
        if (IsCasting || this.enabled == false) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        if (mousePos.x - transform.position.x < 0 )
        {
            _lookDirection = Vector2.left;
        }
        else
        {
            _lookDirection = Vector2.right;
        }

        transform.right = _lookDirection;


        Vector2 direction = new(mousePos.x - _tongue.transform.position.x, mousePos.y - _tongue.transform.position.y);
        _tongue.transform.right = direction;
        StartCoroutine(TongueLash());
    }

    public void OnJump()
    {
        if (AwaitingJump)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePos - (Vector2)transform.position;
            direction.Normalize();

            _rb.gravityScale = _gScale;

            _rb.AddForce(Vector2.up * _movementControllerScr.JumpForce, ForceMode2D.Impulse);

            _movementControllerScr.enabled = true;
            transform.up = Vector2.up;
            transform.right = _movementControllerScr.CurrentDirection;
            AwaitingJump = false;
            _animator.SetBool("IsSticked", false);
        }
    }

    public void OnCrouch()
    {
        if (AwaitingJump)
        {
            _rb.gravityScale = _gScale;
            _movementControllerScr.enabled = true;
            transform.up = Vector2.up;
            transform.right = _movementControllerScr.CurrentDirection;
            AwaitingJump = false;
            _animator.SetBool("IsSticked", false);
        }
    }

    private IEnumerator TongueLash()
    {
        _tongue.SetActive(true);
        IsCasting = true;
        if(!AwaitingJump) _gScale = _rb.gravityScale;
        _rb.gravityScale = 0;
        Vector2 storedVel = _rb.velocity;
        _rb.velocity = Vector2.zero;
        _animator.SetFloat("XSpeed", _rb.velocity.x);
        _animator.SetFloat("YSpeed", _rb.velocity.y);
        _movementControllerScr.enabled = false;
        _initialTongueLength = _tongue.transform.localScale.x;

        while (_tongue.transform.localScale.x < _range)
        {
            float rayCastIncrement;
            Vector3 tongueLength;
            if (_tongue.transform.localScale.x > _range)
            {
                tongueLength = new(_range, _tongue.transform.localScale.y, _tongue.transform.localScale.z);
                rayCastIncrement = _tongue.transform.localScale.x - _range;
            }
            else
            {
                tongueLength = new(_tongue.transform.localScale.x + _tongueSpeed, _tongue.transform.localScale.y, _tongue.transform.localScale.z);
                rayCastIncrement = _tongueSpeed;
            }

            RaycastHit2D rayCastHit = Physics2D.Raycast(_tongue.transform.position, _tongue.transform.right, _tongue.transform.localScale.x + rayCastIncrement, _collisionMask);
            _tongue.transform.localScale = tongueLength;

            if (rayCastHit.collider != null)
            {
                StartCoroutine(PullPlayer(rayCastHit.point));
                yield break;
            }


            yield return null;
        }
        
        
        yield return new WaitForSeconds(.05f);
        
        while(_tongue.transform.localScale.x > _initialTongueLength)
        {
            _tongue.transform.localScale = new(_tongue.transform.localScale.x - _tongueSpeed, _tongue.transform.localScale.y, _tongue.transform.localScale.z);
            if (_tongue.transform.localScale.x < _initialTongueLength) _tongue.transform.localScale = new(_initialTongueLength, _tongue.transform.localScale.y, _tongue.transform.localScale.z);
            
            yield return null;
        }
        _tongue.SetActive(false);

        if (!AwaitingJump)
        {
            _movementControllerScr.enabled = true;
            _rb.gravityScale = _gScale;
            _rb.velocity = storedVel;
        }

        yield return new WaitForSeconds(_tongueCd);
        IsCasting = false;
    }

    private IEnumerator PullPlayer(Vector2 contact)
    {
        _waterMeterScr.DrainWater(_waterCost);

        _animator.SetBool("IsSticked", false);

        Vector2 initialSize = _collider.size;
        _collider.size = new((_collider.size.x * .5f) - .05f, (_collider.size.y * .5f) - .05f);

        Vector2 direction = contact - (Vector2)transform.position;
        direction.Normalize();
        Vector2 moveVector = direction * _pullSpeed;

        //RaycastHit2D rayCastHit = Physics2D.Raycast(_tongue.transform.position, _tongue.transform.right, _tongue.transform.localScale.y + _pullSpeed, _collisionMask);
        RaycastHit2D rayCastHit = Physics2D.CapsuleCast(_collider.bounds.center, _collider.bounds.size, _collider.direction, 0, direction, _pullSpeed, _collisionMask);
        //RaycastHit2D rayCastHit = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0, direction, _pullSpeed, _collisionMask);

        while (rayCastHit.collider == null)
        {
            transform.position = new(transform.position.x + moveVector.x, transform.position.y + moveVector.y, transform.position.z);
            _tongue.transform.localScale = new(_tongue.transform.localScale.x - _pullSpeed, _tongue.transform.localScale.y, _tongue.transform.localScale.z);

            //rayCastHit = Physics2D.Raycast(_tongue.transform.position, _tongue.transform.right, _tongue.transform.localScale.y + _pullSpeed, _collisionMask);
            rayCastHit = Physics2D.CapsuleCast(_collider.bounds.center, _collider.bounds.size, _collider.direction, 0, direction, _pullSpeed, _collisionMask);
            //rayCastHit = Physics2D.BoxCast(_collider.bounds.center, _collider.bounds.size, 0, direction, _pullSpeed, _collisionMask);

            yield return null;
        }

        Vector3 fixedPos = new(transform.position.x + rayCastHit.normal.x * initialSize.x*.3f, transform.position.y + rayCastHit.normal.y * initialSize.y*.3f, transform.position.z);
        transform.position = fixedPos;

        _tongue.transform.localScale = new(_initialTongueLength, _tongue.transform.localScale.y, _tongue.transform.localScale.z);
        _tongue.SetActive(false);

        IsCasting = false;

        if(rayCastHit.normal == Vector2.up)
        {
            transform.up = rayCastHit.normal;
            _rb.gravityScale = _gScale;
            _movementControllerScr.enabled = true;
            AwaitingJump = false;
            //transform.right = Vector2.right;
        }
        else
        {
            transform.right = rayCastHit.normal;
            AwaitingJump = true;
            _animator.SetBool("IsSticked", true);
        }

        _collider.size = initialSize;
    }

    private void OnDrawGizmos()
    {

    }
}
