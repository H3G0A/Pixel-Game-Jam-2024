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
    Animator _animator;

    TongueHook _tongueHookScr;
    PlayerMovementController _movementControllerScr;
    TransformationController _transformControllerScr;

    public static event Action OnAlarmSound;
        
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _playerSprite = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();

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
        _animator.SetFloat("XSpeed", _rb.velocity.x);
        _animator.SetFloat("YSpeed", _rb.velocity.y);
        _rb.isKinematic = true;
        _collider.enabled = false;
        HiddenInWater = true;
        _animator.SetTrigger("Hide");
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
        _animator.SetTrigger("Emerge");

        yield return new WaitForSeconds(.5f);

        _movementControllerScr.enabled = true;
        _tongueHookScr.enabled = true;
        _transformControllerScr.enabled = true;

        _rb.isKinematic = false;
        _collider.enabled = true;
        HiddenInWater = false;
        transform.right = _movementControllerScr.CurrentDirection;
    }
}
