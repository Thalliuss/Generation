using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 0;
    [SerializeField] private float _jumpForce = 0;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        Walk();
    }

    private void Update()
    {
        Rotate();
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    private void Jump()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2))
        {
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
        else return;

    }

    private void Rotate()
    {
        float yRot = Input.GetAxis("Mouse X") * 2;

        transform.rotation *= Quaternion.Euler(0f, yRot, 0f);
    }

    private void Walk()
    {
        float t_horizontal = Input.GetAxisRaw("Horizontal") * _speed;
        float t_vertical = Input.GetAxisRaw("Vertical") * _speed;

        t_horizontal *= Time.deltaTime;
        t_vertical *= Time.deltaTime;

        if (t_horizontal != 0 && t_vertical != 0)
        {
            t_horizontal *= .7f;
            t_vertical *= .7f;
        }

        Vector3 t_move = new (t_horizontal, 0, t_vertical);

        transform.Translate(t_move);
    }
}
