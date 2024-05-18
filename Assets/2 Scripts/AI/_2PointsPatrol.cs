using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _2PointsPatrol : MonoBehaviour
{
    [SerializeField] Transform _leftLimit;
    [SerializeField] Transform _rightLimit;
    [Space(10)]
    [SerializeField] float _speed;
    [SerializeField] float _alertedSpeed;
    [SerializeField] float _restTime;

    bool _isTurning = false;
    Rigidbody2D _rb;

    private void OnEnable()
    {
        StealthController.OnAlarmSound += RaiseAlarm;
    }

    private void OnDisable()
    {
        StealthController.OnAlarmSound -= RaiseAlarm;
    }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            _rb.velocity = Vector2.zero;
            collision.GetComponent<StealthController>().GotCaught();
        }
    }

    private void ManageMovement()
    {
        if (_isTurning) return;

        if (transform.position.x <= _leftLimit.position.x && (Vector2)transform.right == Vector2.left)
        {
            StartCoroutine(TurnAround(Vector2.right));
        }
        else if (transform.position.x >= _rightLimit.position.x && (Vector2)transform.right == Vector2.right)
        {
            StartCoroutine(TurnAround(Vector2.left));
        }
    }

    public void RaiseAlarm()
    {
        if(!_isTurning) _rb.velocity = transform.right * _alertedSpeed;
        _speed = _alertedSpeed;
    }

    private IEnumerator TurnAround(Vector2 direction)
    {
        _rb.velocity = Vector2.zero;
        _isTurning = true;

        yield return new WaitForSeconds(_restTime);

        transform.right = direction;

        yield return new WaitForSeconds(.3f);

        _rb.velocity = transform.right * _speed;
        _isTurning = false;
    }
}
