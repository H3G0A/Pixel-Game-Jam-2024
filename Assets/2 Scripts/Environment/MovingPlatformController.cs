using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformController : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _restTime;
    [SerializeField] Transform _leftLimit;
    [SerializeField] Transform _rightLimit;

    bool _isTurning;
    bool _movingRight = true;
    Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();        
    }

    private void Start()
    {
        _rb.velocity = transform.right * _speed;
    }

    void Update()
    {
        ManageMovement();
    }

    private void ManageMovement()
    {
        if (_isTurning) return;

        if ((transform.position.x <= _leftLimit.position.x && !_movingRight) || (transform.position.x >= _rightLimit.position.x && _movingRight))
        {
            StartCoroutine(TurnAround());
        }
    }

    private IEnumerator TurnAround()
    {
        _rb.velocity = Vector2.zero;
        _isTurning = true;

        yield return new WaitForSeconds(_restTime);

        _rb.velocity = transform.right * (-1 * _speed);
        _isTurning = false;
        _movingRight = !_movingRight;
    }
}
