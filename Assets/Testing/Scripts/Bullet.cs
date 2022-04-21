using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private float _lifetime = 5f;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.velocity += transform.forward * 20f;

        StartCoroutine(DespawnHandler());
    }

    private IEnumerator DespawnHandler()
    {
        yield return new WaitForSeconds(_lifetime);

        Destroy();
    }

    public void Destroy() 
    {
        Destroy(gameObject);
    }

    void Update()
    {
        transform.Rotate(1,1,0);
    }
}
