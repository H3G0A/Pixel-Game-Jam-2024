using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterMeter : MonoBehaviour
{
    [SerializeField] int _maxWater;
    [SerializeField] int _drainingRate;
    [SerializeField] Image _meterUI;

    [SerializeField] float _currentWater;

    // Start is called before the first frame update
    void Start()
    {
        _currentWater = _maxWater;
    }

    // Update is called once per frame
    void Update()
    {
        ManagerWater();
    }

    private void ManagerWater()
    {
        if (_currentWater == 0) return;

        DrainWater(Time.deltaTime * _drainingRate);
    }

    public void DrainWater(float amount)
    {
        _currentWater -= amount;
        if (_currentWater < 0) _currentWater = 0;
        float normalizedValue = _currentWater / _maxWater;
        _meterUI.fillAmount = normalizedValue;
    }
}
