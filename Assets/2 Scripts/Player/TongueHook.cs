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

    Rigidbody2D _rb;
    CapsuleCollider2D _collider;
    
    PlayerMovementController _movementControllerScr;
    WaterMeter _waterMeterScr;

    bool _isCasting = false;
    bool _awaitingJump = false;

    float _gScale;
    float _initialTongueLength;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _movementControllerScr = GetComponent<PlayerMovementController>();
        _waterMeterScr = GetComponent<WaterMeter>();
    }

    private void Start()
    {
        //_tongue.SetActive(false);
        _gScale = _rb.gravityScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if((_collisionMask & (1 << collision.gameObject.layer)) != 0)
        {

        }
    }

    public void OnHook()
    {
        if (_isCasting) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x - transform.position.x < 0)
        {
            transform.right = Vector2.left;
        }
        else
        {
            transform.right = Vector2.right;
        }


        Vector2 direction = new(mousePos.x - _tongue.transform.position.x, mousePos.y - _tongue.transform.position.y);
        _tongue.transform.right = direction;
        StartCoroutine(TongueLash());
    }

    public void OnJump()
    {
        if (_awaitingJump)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePos - (Vector2)transform.position;
            direction.Normalize();

            _rb.AddForce(Vector2.up * _wallJumpForce, ForceMode2D.Impulse);

            _rb.gravityScale = _gScale;
            _movementControllerScr.enabled = true;
            transform.up = Vector2.up;
            _awaitingJump = false;
        }
    }

    public void OnCrouch()
    {
        if (_awaitingJump)
        {
            _rb.gravityScale = _gScale;
            _movementControllerScr.enabled = true;
            transform.up = Vector2.up;
            _awaitingJump = false;
        }
    }

    private IEnumerator TongueLash()
    {
        _tongue.SetActive(true);
        _isCasting = true;
        _rb.gravityScale = 0;
        _rb.velocity = Vector2.zero;
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

        _rb.gravityScale = _gScale;
        _movementControllerScr.enabled = true;
        _tongue.transform.right = Vector2.right;
        //_tongue.SetActive(false);

        yield return new WaitForSeconds(_tongueCd);
        _isCasting = false;
        _awaitingJump = false;
    }

    private IEnumerator PullPlayer(Vector2 contact)
    {
        _waterMeterScr.DrainWater(_waterCost);

        Vector2 initialSize = _collider.size;
        _collider.size = new(.5f, .5f);

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
        //_tongue.SetActive(false);

        transform.up = rayCastHit.normal;
        _isCasting = false;

        if(rayCastHit.normal == Vector2.up)
        {
            _rb.gravityScale = _gScale;
            _movementControllerScr.enabled = true;
            _awaitingJump = false;
            //transform.right = Vector2.right;
        }
        else
        {
            _awaitingJump = true;
        }

        _collider.size = initialSize;
    }

    private void OnDrawGizmos()
    {

    }
}
