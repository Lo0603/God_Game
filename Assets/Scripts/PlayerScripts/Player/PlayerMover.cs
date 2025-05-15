using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 타일 기반 이동 및 벽 감지 기능을 담당하는 스크립트
/// Player.cs에서 호출되어 이동 제어 전담
/// </summary>
public class PlayerMover : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float tileSize = 5f; // 타일 한 칸의 크기

    private bool isMoving = true;
    private bool stoppingSoon = false; // 다음 타일까지 이동 후 멈출지 여부
    private float targetX;
    private int moveDirection = 1; // 1 = 오른쪽, -1 = 왼쪽

    private Rigidbody2D rb;

    public bool IsMoving => isMoving;

    public void Init(Rigidbody2D rigid)
    {
        rb = rigid;
    }

    public void ManualUpdate()
    {
        if (isMoving || stoppingSoon)
        {
            if (!stoppingSoon && CheckWallAhead())
            {
                stoppingSoon = true;
                targetX = CalculateNextTilePosition();
            }

            transform.position += Vector3.right * moveDirection * moveSpeed * Time.deltaTime;

            if (stoppingSoon && HasReachedTargetX())
            {
                SnapToTargetX();
                StopMovingImmediately();
            }
        }
    }

    public void SetMoving(bool value)
    {
        if (value)
        {
            if (CheckWallAhead())
            {
                isMoving = false;
                stoppingSoon = false;
                return;
            }

            StartMoving();
        }
        else if (isMoving)
        {
            stoppingSoon = true;
            targetX = CalculateNextTilePosition();
        }
    }


    public void StartMoving()
    {
        if (CheckWallAhead()) return;

        isMoving = true;
        stoppingSoon = false;
        if (rb != null) rb.gravityScale = Mathf.Abs(rb.gravityScale);
    }

    public void StopMovingSmoothly()
    {
        if (isMoving)
        {
            stoppingSoon = true;
            targetX = CalculateNextTilePosition();
        }
    }

    public void StopMovingImmediately()
    {
        isMoving = false;
        stoppingSoon = false;
    }

    public void SetDirection(int dir)
    {
        moveDirection = dir;
    }

    public int GetDirection() => moveDirection;

    float CalculateNextTilePosition()
    {
        float currentX = transform.position.x;
        return moveDirection == 1 ? Mathf.Ceil(currentX / tileSize) * tileSize
                                  : Mathf.Floor(currentX / tileSize) * tileSize;
    }

    bool HasReachedTargetX()
    {
        return (moveDirection == 1 && transform.position.x >= targetX) ||
               (moveDirection == -1 && transform.position.x <= targetX);
    }

    void SnapToTargetX()
    {
        Vector3 pos = transform.position;
        pos.x = targetX;
        transform.position = pos;
    }

    bool CheckWallAhead()
    {
        Vector2 direction = moveDirection == 1 ? Vector2.right : Vector2.left;
        float checkDistance = tileSize * 2f; // 예: 10

        Debug.DrawLine(transform.position, transform.position + (Vector3)(direction * checkDistance), Color.green);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, checkDistance, LayerMask.GetMask("Platform", "Blocked"));
        return hit.collider != null;
    }
}
