using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class StealthController : MonoBehaviour
{
    [SerializeField] Transform _spawnPoint;

    bool _isCaught = false;
    Rigidbody2D _rb;
    Collider2D _collider;

    public static event Action OnAlarmSound;
        
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        transform.position = _spawnPoint.position;
    }

    public void HideInWater()
    {
        _rb.isKinematic = true;
        _collider.enabled = false;
    }

    public void ReemergeFromWater()
    {
        _rb.isKinematic = false;
        _collider.enabled = true;
    }

    public void GotCaught()
    {
        if (_isCaught) return;
        _isCaught = true;

        StartCoroutine(Restart());
    }

    public void SoundAlarm()
    {
        OnAlarmSound?.Invoke();
    }

    private IEnumerator Restart()
    {
        yield return null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
