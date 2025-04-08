using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public Vector2 gridMove = new Vector2(1.0f, 1.0f); // ���̒l�̓^�C���̑傫���ɍ��킹�Ē���

    void Update()
    {
        Vector3 move = new Vector3(0, 0, 0);
        if (Input.GetKeyDown(KeyCode.W))
        {
            move.y += gridMove.y;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            move.y -= gridMove.y;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            move.x -= gridMove.x;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            move.x += gridMove.x;
        }

        if (move != Vector3.zero)
        {
            transform.position += move; 
        } // �^�C���T�C�Y�P�ʂňړ�
    }
}
