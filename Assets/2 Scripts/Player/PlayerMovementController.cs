using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] float _maxSpeed;
    [SerializeField] float _jumpForce;
    [SerializeField] CapsuleCollider2D _capsuleCollider;
    [SerializeField] LayerMask _groundedMask;

    float _currentSpeed = 0;
    float _direction;

    Rigidbody2D _rb;


    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        //_movement = new(transform.position.x, transform.position.y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ManageMovement();
    }

    private void ManageMovement()
    {
        _rb.velocity = new Vector2(_direction * _maxSpeed, _rb.velocity.y);
    }

    public void OnRun(InputValue value)
    {
        _direction = value.Get<float>();
    }

    public void OnJump()
    {
        if(IsGrounded() && this.enabled) _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
    }

    private Boolean IsGrounded()
    {
        Vector2 origin = new(_capsuleCollider.bounds.center.x, _capsuleCollider.bounds.center.y - _capsuleCollider.size.x + .4f);
        RaycastHit2D rayCastHit = Physics2D.CircleCast(origin, _capsuleCollider.size.x * .5f, Vector2.down, .1f, _groundedMask);
        //RaycastHit2D rayCastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, groundCheckDirection, extraHeight, groundLayer);

        //Debug.Log(rayCastHit.collider);
        bool isGrounded = rayCastHit.collider != null;

        return isGrounded;
    }

    private void OnDrawGizmosSelected()
    {
        Color red = new Color(1, 0, 0, .3f);
        Color green = new Color(0, 1, 0, .3f);

        Vector2 origin = new(_capsuleCollider.bounds.center.x, _capsuleCollider.bounds.center.y - _capsuleCollider.size.x + .3f);

        if (IsGrounded()) Gizmos.color = green;
        else              Gizmos.color = red;
        Gizmos.DrawSphere(origin, _capsuleCollider.size.x * .5f);

    }
}
