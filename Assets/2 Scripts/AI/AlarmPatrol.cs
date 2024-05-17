using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmPatrol : MonoBehaviour
{
    [SerializeField] float _timeAwake;
    [SerializeField] float _timeAsleep;
    [SerializeField] Collider2D _alarmCollider;

    bool _isAwake = true;

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
        StartCoroutine(AwakeCycle());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision with: " + collision.name);
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player");
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

    private IEnumerator WakeUp()
    {
        _isAwake = true;

        yield return new WaitForSeconds(1); // Leave time for the player to react before getting caught inside the alarm range;

        _alarmCollider.enabled = true;
    }

    private IEnumerator AwakeCycle()
    {
        StartCoroutine(WakeUp());

        yield return new WaitForSeconds(_timeAwake + 1);

        StartCoroutine(AsleepCycle());
    }

    private IEnumerator AsleepCycle()
    {
        GoToSleep();

        yield return new WaitForSeconds(_timeAsleep);

        StartCoroutine(AwakeCycle());
    }
}
