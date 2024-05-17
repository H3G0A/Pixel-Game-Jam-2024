using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] GameObject _target;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new(_target.transform.position.x, _target.transform.position.y);

    }
}
