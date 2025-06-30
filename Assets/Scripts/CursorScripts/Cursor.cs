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

        if (Input.GetKeyDown(KeyCode.W))
        {
            move.y += gridMove.y; 
            SoundManager.Instance.PlaySE("カーソル移動");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            move.y -= gridMove.y; 
            SoundManager.Instance.PlaySE("カーソル移動");
        } 
        else if (Input.GetKeyDown(KeyCode.A))
        {
            move.x -= gridMove.x;
            SoundManager.Instance.PlaySE("カーソル移動");
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            move.x += gridMove.x; 
            SoundManager.Instance.PlaySE("カーソル移動");
        }

        if (move != Vector3.zero)
        {
            Vector3 targetPos = transform.position + move;

            // カメラ境界計算
            float vertExtent = mainCam.orthographicSize;
            float horzExtent = vertExtent * Screen.width / Screen.height;

            Vector3 camPos = mainCam.transform.position;

            float minX = camPos.x - horzExtent;
            float maxX = camPos.x + horzExtent;
            float minY = camPos.y - vertExtent;
            float maxY = camPos.y + vertExtent;

            // カーソル サイズを考慮しない場合:タイル中央まで制御
            if (targetPos.x >= minX && targetPos.x <= maxX &&
                targetPos.y >= minY && targetPos.y <= maxY)
            {
                transform.position = targetPos;
            }
        }
    }
}
