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
        if(transform.position.x <= _leftLimit.position.x)
        {
            StartCoroutine(TurnAround(Vector2.right));
        } 
        else if(transform.position.x >= _rightLimit.position.x)
        {
            StartCoroutine(TurnAround(Vector2.left));
        }
    }

    public void RaiseAlarm()
    {
        _rb.velocity = transform.right * _alertedSpeed;
        _speed = _alertedSpeed;
    }

    private IEnumerator TurnAround(Vector2 direction)
    {
        _rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(_restTime);

        transform.right = direction;

        yield return new WaitForSeconds(.5f);

        _rb.velocity = transform.right * _speed;
    }
}