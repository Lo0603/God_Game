using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public Vector2 gridMove = new Vector2(5.0f, 5.0f); // タイルサイズ
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W)) move.y += gridMove.y;
        if (Input.GetKeyDown(KeyCode.S)) move.y -= gridMove.y;
        if (Input.GetKeyDown(KeyCode.A)) move.x -= gridMove.x;
        if (Input.GetKeyDown(KeyCode.D)) move.x += gridMove.x;

        if (move != Vector3.zero)
        {
            Vector3 targetPos = transform.position + move;

            // 카메라 경계 계산
            float vertExtent = mainCam.orthographicSize;
            float horzExtent = vertExtent * Screen.width / Screen.height;

            Vector3 camPos = mainCam.transform.position;

            float minX = camPos.x - horzExtent;
            float maxX = camPos.x + horzExtent;
            float minY = camPos.y - vertExtent;
            float maxY = camPos.y + vertExtent;

            // 커서 크기를 고려하지 않는 경우: 타일 중앙까지만 제어
            if (targetPos.x >= minX && targetPos.x <= maxX &&
                targetPos.y >= minY && targetPos.y <= maxY)
            {
                transform.position = targetPos;
            }
        }
    }
}
