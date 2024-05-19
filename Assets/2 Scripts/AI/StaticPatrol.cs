using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticPatrol : MonoBehaviour
{
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<StealthController>().GotCaught();
            _animator.SetTrigger("Surprised");
        }
    }
}
