using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraViewAdjuster : MonoBehaviour
{
    public Camera cam;

    // tile大きさ (例: 5x5)
    public float tileWidth = 5f;
    public float tileHeight = 5f;

    // MAP大きさ
    public int mapWidthInTiles = 18;  
    public int mapHeightInTiles = 10; 

    void Start()
    {
        UpdateCameraSize();
    }

    public void UpdateCameraSize()
    {
        float screenAspect = (float)Screen.width / Screen.height;

        // 全体MAP大きさ
        float mapWidth = mapWidthInTiles * tileWidth;
        float mapHeight = mapHeightInTiles * tileHeight;

        // カメラ調整(MAP全体映るように)
        float cameraSizeByWidth = mapWidth / (2f * screenAspect);
        float cameraSizeByHeight = mapHeight / 2f;

        // 2つのうち、より大きい方を使用すると全体が画面に表示される
        cam.orthographicSize = Mathf.Max(cameraSizeByWidth, cameraSizeByHeight);
    }
}


//// STAGE1 (18 x 10)
//scaler.mapWidthInTiles = 18;
//scaler.mapHeightInTiles = 10;
//scaler.UpdateCameraSize();

//// STAGE2 (32 x 18)
//scaler.mapWidthInTiles = 32;
//scaler.mapHeightInTiles = 18;
//scaler.UpdateCameraSize();