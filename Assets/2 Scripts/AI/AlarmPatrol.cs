using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmPatrol : MonoBehaviour
{
    [SerializeField] float _timeAwake;
    [SerializeField] float _timeAsleep;
    [SerializeField] Collider2D _alarmCollider;

    Animator _animator;

    bool _isAwake;
    bool _soundedAlarm = false;

    // Start is called before the first frame update

    private void OnEnable()
    {
        StealthController.OnAlarmSound += ActivateAlarm;
    }

    private void OnDisable()
    {
        StealthController.OnAlarmSound -= ActivateAlarm;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _isAwake = false;
        _alarmCollider.enabled = false;
        StartCoroutine(AwakeCycle());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _soundedAlarm = true;
            collision.GetComponent<StealthController>().SoundAlarm();
        }
    }

    private void GoToSleep()
    {
        _isAwake = false;
        _alarmCollider.enabled = false;
        _animator.SetBool("Awake", false);
    }

    private void ActivateAlarm()
    {
        StopAllCoroutines();

        if (!_soundedAlarm)
        {
            GoToSleep();
        }
        else
        {
            _animator.SetTrigger("Alarm");
            StartCoroutine(SleepAfterDelay());
        }
    }

    private IEnumerator WakeUp(float wakeUpTime)
    {
        _isAwake = true;
        _animator.SetTrigger("Awakening");

        yield return new WaitForSeconds(wakeUpTime); // Leave time for the player to react before getting caught inside the alarm range;

        _alarmCollider.enabled = true;
        _animator.SetBool("Awake", true);
    }

    private IEnumerator AwakeCycle()
    {
        float wakeUpTime = 2.5f;
        StartCoroutine(WakeUp(wakeUpTime));

        yield return new WaitForSeconds(wakeUpTime + _timeAwake);

        StartCoroutine(AsleepCycle());
    }

    private IEnumerator AsleepCycle()
    {
        GoToSleep();

        yield return new WaitForSeconds(_timeAsleep);

        StartCoroutine(AwakeCycle());
    }

    private IEnumerator SleepAfterDelay()
    {
        yield return new WaitForSeconds(7);

        GoToSleep();
    }
}
