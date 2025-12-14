using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    public float speed = 5.0f;
    private Rigidbody _rb;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Move(Direction dir)
    {
        Vector3 moveDir = dir switch
        {
            Direction.UP => Vector3.forward,
            Direction.DOWN => Vector3.back,
            Direction.LEFT => Vector3.left,
            Direction.RIGHT => Vector3.right,
            _ => Vector3.zero
        };

        _rb.position += moveDir * (speed * Time.deltaTime);
    }
}

public enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}
