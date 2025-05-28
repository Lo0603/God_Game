using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    [Header("Zoom Settings")]
    public Camera cam;
    public float zoomInSize = 3f;      // 拡大時目標大きさ
    public float zoomSpeed = 2f;       // 拡大速度
    public float moveSpeed = 2f;       // カメラの移動速度

    private float originalSize;
    private Vector3 originalPosition;
    private Transform target;          // Player参照

    private float targetSize;
    private Vector3 targetPosition;
    private bool isZooming = false;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        originalSize = cam.orthographicSize;
        originalPosition = cam.transform.position;
        targetSize = originalSize;
    }

    void Update()
    {
        if (isZooming && target != null)
        {
            // 1. カメラの大きさ調整
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);

            // 2. カメラの位置をプレイヤーの方向に移動
            targetPosition = new Vector3(target.position.x, target.position.y, originalPosition.z); // Z 고정
            cam.transform.position = Vector3.Lerp(cam.transform.position, targetPosition, Time.deltaTime * moveSpeed);

            // 3. 目標到達確認
            if (Mathf.Abs(cam.orthographicSize - targetSize) < 0.01f)
            {
                cam.orthographicSize = targetSize;
                isZooming = false;
            }
        }
    }

    //  拡大開始(プレイヤーにTransform渡す)
    public void ZoomIn(Transform playerTransform)
    {
        target = playerTransform;
        targetSize = zoomInSize;
        isZooming = true;
    }

    //  拡大復旧
    public void ZoomOut()
    {
        target = null;
        targetSize = originalSize;
        isZooming = true;
        cam.transform.position = originalPosition; // 位置復旧
    }
}
