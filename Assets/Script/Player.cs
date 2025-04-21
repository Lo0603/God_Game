using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField, Header("移動速度")]
    private float _moveSpeed;

    [SerializeField, Header("デバッグモード（速度変更できるようにする）")]
    private bool _isDebugMode = false;


    private Vector2 _inputDirection;
    private Rigidbody2D _rigid;

    private bool _isStopped = false;

    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _inputDirection = Vector2.right; // 最初から右に進む
    }

    void Update()
    {
        // 速度調整はデバッグモードのときだけ
        if (_isDebugMode)
        {
            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                _moveSpeed += 1f;
            }

            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                _moveSpeed = Mathf.Max(1f, _moveSpeed - 1f);
            }

            _moveSpeed = Mathf.Clamp(_moveSpeed, 1f, 10f);

            Debug.Log("現在の速度: " + _moveSpeed);
        }

        // Sキーで停止/再開（これは通常プレイヤーも可）
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            _isStopped = !_isStopped;
        }

        _Move();
    }



    private void _Move()
    {
        if (_isStopped)
        {
            _rigid.velocity = new Vector2(0f, _rigid.velocity.y);
        }
        else
        {
            _rigid.velocity = new Vector2(_inputDirection.x * _moveSpeed, _rigid.velocity.y);
        }
    }
}
