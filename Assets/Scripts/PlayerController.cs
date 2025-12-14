using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMove _playerMove;

    private void Awake()
    {
        if (!TryGetComponent(out _playerMove))
        {
            Debug.LogError("PlayerMove component not found!");
        }
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _playerMove.Move(Direction.UP);
        }
        if (Input.GetKey(KeyCode.S))
        {
            _playerMove.Move(Direction.DOWN);
        } 
        if (Input.GetKey(KeyCode.A))
        {
            _playerMove.Move(Direction.LEFT);
        }
        if (Input.GetKey(KeyCode.D))
        {
            _playerMove.Move(Direction.RIGHT);
        }
    }
}
