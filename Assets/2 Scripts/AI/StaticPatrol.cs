using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticPatrol : MonoBehaviour
{
    Rigidbody2D _rb;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            collision.GetComponent<StealthController>().GotCaught();
        }
    }
}
