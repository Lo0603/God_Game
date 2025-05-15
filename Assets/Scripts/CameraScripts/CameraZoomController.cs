using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    [Header("Zoom Settings")]
    public Camera cam;
    public float zoomInSize = 3f;      // 확대 시 목표 크기
    public float zoomSpeed = 2f;       // 확대 속도
    public float moveSpeed = 2f;       // 카메라 이동 속도

    private float originalSize;
    private Vector3 originalPosition;
    private Transform target;          // 플레이어 참조

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
            // 1. 카메라 크기 부드럽게 조정
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);

            // 2. 카메라 위치 부드럽게 플레이어 쪽으로 이동
            targetPosition = new Vector3(target.position.x, target.position.y, originalPosition.z); // Z 고정
            cam.transform.position = Vector3.Lerp(cam.transform.position, targetPosition, Time.deltaTime * moveSpeed);

            // 3. 목표 도달 체크
            if (Mathf.Abs(cam.orthographicSize - targetSize) < 0.01f)
            {
                cam.orthographicSize = targetSize;
                isZooming = false;
            }
        }
    }

    // ✅ 확대 시작 (플레이어 트랜스폼 넘겨주기)
    public void ZoomIn(Transform playerTransform)
    {
        target = playerTransform;
        targetSize = zoomInSize;
        isZooming = true;
    }

    // ✅ 확대 복구
    public void ZoomOut()
    {
        target = null;
        targetSize = originalSize;
        isZooming = true;
        cam.transform.position = originalPosition; // 위치 복구
    }
}
