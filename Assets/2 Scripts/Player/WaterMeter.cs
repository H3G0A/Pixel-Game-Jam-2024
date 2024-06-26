using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterMeter : MonoBehaviour
{
    [SerializeField] int _maxWater;
    [Space(10)]
    [SerializeField] float _normalDrainingRate;
    [SerializeField] float _hotZoneDrainingRate;
    [SerializeField] float _heatTrapDrainingRate;
    [Space(10)]
    [SerializeField] float _currentDrainingRate;
    [Space(10)]
    [SerializeField] Image _meterUI;

    float _currentWater;
    Animator _animator;
    StealthController _stealthControllerScr;


    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _stealthControllerScr = GetComponent<StealthController>();
    }

    void Start()
    {
        _currentWater = _maxWater;
        _currentDrainingRate = _normalDrainingRate;
    }

    // Update is called once per frame
    void Update()
    {
        ManageWater();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HeatTrap")) _currentDrainingRate += _heatTrapDrainingRate;
        if (collision.CompareTag("HotZone")) _currentDrainingRate += _hotZoneDrainingRate;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("HeatTrap")) _currentDrainingRate -= _heatTrapDrainingRate;
        if (collision.CompareTag("HotZone")) _currentDrainingRate -= _hotZoneDrainingRate;
    }

    private void ManageWater()
    {
        if (_stealthControllerScr.HiddenInWater) return;

        if(_currentWater <= 0)
        {
            _currentDrainingRate = 0;
            _animator.SetTrigger("Death");
            _stealthControllerScr.GotCaught();
        }

        DrainWater(Time.deltaTime * _currentDrainingRate);
    }

    public void DrainWater(float amount)
    {
        _currentWater -= amount;
        if (_currentWater < 0) _currentWater = 0;
        float normalizedValue = _currentWater / _maxWater;
        _meterUI.fillAmount = normalizedValue;
    }
}
