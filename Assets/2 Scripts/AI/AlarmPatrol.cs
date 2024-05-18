using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmPatrol : MonoBehaviour
{
    [SerializeField] float _timeAwake;
    [SerializeField] float _timeAsleep;
    [SerializeField] Collider2D _alarmCollider;

    bool _isAwake;

    // Start is called before the first frame update

    private void OnEnable()
    {
        StealthController.OnAlarmSound += SleepForever;
    }

    private void OnDisable()
    {
        StealthController.OnAlarmSound -= SleepForever;
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

            collision.GetComponent<StealthController>().SoundAlarm();
        }
    }

    private void GoToSleep()
    {

        _isAwake = false;
        _alarmCollider.enabled = false;
    }

    private void SleepForever()
    {
        StopAllCoroutines();

        GoToSleep();
    }

    private IEnumerator WakeUp(float wakeUpTime)
    {
        _isAwake = true;

        yield return new WaitForSeconds(wakeUpTime); // Leave time for the player to react before getting caught inside the alarm range;

        _alarmCollider.enabled = true;
    }

    private IEnumerator AwakeCycle()
    {
        float wakeUpTime = 1;
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
}
