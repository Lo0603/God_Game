using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraViewAdjuster : MonoBehaviour
{
    public Camera cam;

    // 타일 하나의 크기 (예: 5x5)
    public float tileWidth = 5f;
    public float tileHeight = 5f;

    // 맵 크기
    public int mapWidthInTiles = 18;  // 예: 18 또는 32
    public int mapHeightInTiles = 10; // 예: 10 또는 18

    void Start()
    {
        UpdateCameraSize();
    }

    public void UpdateCameraSize()
    {
        float screenAspect = (float)Screen.width / Screen.height;

        // 전체 맵 크기 (유닛 기준)
        float mapWidth = mapWidthInTiles * tileWidth;
        float mapHeight = mapHeightInTiles * tileHeight;

        // 카메라가 맵 전체를 담으려면
        float cameraSizeByWidth = mapWidth / (2f * screenAspect);
        float cameraSizeByHeight = mapHeight / 2f;

        // 둘 중 더 큰 쪽을 사용해야 전체가 화면에 들어옴
        cam.orthographicSize = Mathf.Max(cameraSizeByWidth, cameraSizeByHeight);
    }
}


//// 스테이지1 (18 x 10)
//scaler.mapWidthInTiles = 18;
//scaler.mapHeightInTiles = 10;
//scaler.UpdateCameraSize();

//// 스테이지2 (32 x 18)
//scaler.mapWidthInTiles = 32;
//scaler.mapHeightInTiles = 18;
//scaler.UpdateCameraSize();