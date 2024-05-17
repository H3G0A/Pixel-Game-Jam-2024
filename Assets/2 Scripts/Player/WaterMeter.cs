using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterMeter : MonoBehaviour
{
    [SerializeField] int _maxWater;
    [SerializeField] float _normalDrainingRate;
    [SerializeField] float _hotZoneDrainingRate;
    [SerializeField] Image _meterUI;

    float _currentDrainingRate;
    float _currentWater;

    // Start is called before the first frame update
    void Start()
    {
        _currentWater = _maxWater;
        _currentDrainingRate = _normalDrainingRate;
    }

    // Update is called once per frame
    void Update()
    {
        ManagerWater();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HotZone")) _currentDrainingRate = _hotZoneDrainingRate;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("HotZone")) _currentDrainingRate = _normalDrainingRate;
    }

    private void ManagerWater()
    {
        if (_currentWater == 0) return;

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
