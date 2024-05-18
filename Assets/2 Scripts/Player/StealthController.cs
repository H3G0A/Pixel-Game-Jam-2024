using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class StealthController : MonoBehaviour
{
    [SerializeField] Transform _spawnPoint;

    [HideInInspector] public bool HiddenInWater = false;
    bool _isCaught = false;

    Rigidbody2D _rb;
    Collider2D _collider;
    SpriteRenderer _playerSprite;

    TongueHook _tongueHookScr;
    PlayerMovementController _movementControllerScr;
    TransformationController _transformControllerScr;

    public static event Action OnAlarmSound;
        
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _playerSprite = GetComponentInChildren<SpriteRenderer>();

        _tongueHookScr = GetComponent<TongueHook>();
        _movementControllerScr = GetComponent<PlayerMovementController>();
        _transformControllerScr = GetComponent<TransformationController>();
    }

    private void Start()
    {
        transform.position = _spawnPoint.position;
    }

    public void HideInWater()
    {
        if (HiddenInWater) return;

        _movementControllerScr.enabled = false;
        _transformControllerScr.enabled = false;
        _tongueHookScr.enabled = false;

        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;
        _collider.enabled = false;
        HiddenInWater = true;

        _playerSprite.color = new Color32(100, 101, 231, 155);
    }

    public void ReemergeFromWater()
    {
        StartCoroutine(Reemerge());
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

    public void OnJump()
    {
        if (HiddenInWater)
        {
            ReemergeFromWater();
        }
    }

    private IEnumerator Restart()
    {
        yield return null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator Reemerge()
    {

        yield return null;

        _movementControllerScr.enabled = true;
        _tongueHookScr.enabled = true;
        _transformControllerScr.enabled = true;

        _rb.isKinematic = false;
        _collider.enabled = true;
        HiddenInWater = false;
        transform.right = _movementControllerScr.CurrentDirection;

        _playerSprite.color = new Color32(100, 101, 231, 255);
    }
}
