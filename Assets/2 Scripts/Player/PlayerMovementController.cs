using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    public float MaxSpeed;
    public float JumpForce;
    [SerializeField] CapsuleCollider2D _capsuleCollider;
    [SerializeField] LayerMask _groundedMask;

    [HideInInspector] public Vector2 CurrentDirection;
    float _currentSpeed = 0;
    float _direction;

    Rigidbody2D _rb;


    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ManageMovement();
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
        if(IsGrounded() && this.enabled) _rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    public bool IsGrounded()
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
