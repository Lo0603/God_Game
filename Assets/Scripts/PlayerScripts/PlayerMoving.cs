using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerMoving : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float tileSize = 5f; // 추가: 타일 크기

    [Header("Internal States")]
    private bool isMoving = false;
    private int moveDirection = 1; // 1 = 右 , -1 = 左
    private bool stoppingSoon = false; // 추가: 이동 중 멈출 준비
    private float targetX; // 추가: 목표 위치

    [Header("Components")]
    private Animator anim;
    private Rigidbody2D rb;
    private float originalGravity;
    private float storedGravity;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        storedGravity = rb.gravityScale;
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        HandleInput();

        //CheckGroundAndAdjustGravity(); // 

        if (isMoving || stoppingSoon)
        {
            anim.SetBool("isWalking", true);

            // ⭐ 이동 중에도 벽 체크
            if (!stoppingSoon && CheckWallAhead())
            {
                stoppingSoon = true;
                targetX = CalculateNextTilePosition();
            }

            transform.position += Vector3.right * moveDirection * moveSpeed * Time.deltaTime;

            // もしstopping Soon中なら目標地点チェック
            if (stoppingSoon && HasReachedTargetX())
            {
                SnapToTargetX();
                StopMoving();
            }

        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (isMoving)
            {
                stoppingSoon = true;
                targetX = CalculateNextTilePosition();
            }
            else
            {
                if (CheckWallAhead())
                {
                    isMoving = false;
                    stoppingSoon = false;
                    anim.SetBool("isWalking", false);
                    return;
                }

                StartMoving();
            }
        }

        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    moveDirection *= -1;
        //    FlipSprite();  // Sprite反転
        //}
    }



    void CheckGroundAndAdjustGravity()
    {
        // 1. 重力方向基準
        Vector2 baseDirection = storedGravity > 0 ? Vector2.down : Vector2.up;

        // 2.Z回転を反映した方向に回転
        Vector2 rayDirection = Quaternion.Euler(0, 0, transform.eulerAngles.z) * baseDirection;

        float rayDistance = 2.6f;
        int groundLayer = LayerMask.GetMask("Platform", "Blocked","Ground", "GravityBlock");

        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, rayDistance, groundLayer);

        Color debugColor = hit.collider != null ? Color.green : Color.red;
        Debug.DrawLine(transform.position, transform.position + (Vector3)(rayDirection * rayDistance), debugColor, 0f, false);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject != this.gameObject)
            {
                if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Platform"))
                {
                    rb.gravityScale = 0f;
                    return;
                }
            }
        }

        rb.gravityScale = storedGravity;
    }



    public void SetMoving(bool value)
    {
        if (value)
        {
            if (CheckWallAhead())
            {
                isMoving = false;
                stoppingSoon = false;
                anim.SetBool("isWalking", false);
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

    void StartMoving()
    {
        isMoving = true;
        stoppingSoon = false;
        anim.SetBool("isWalking", true);
        rb.gravityScale = storedGravity;
    }

    public void StopMoving()
    {
        isMoving = false;
        stoppingSoon = false;
        anim.SetBool("isWalking", false);
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

    float CalculateNextTilePosition()
    {
        float currentX = transform.position.x;
        return moveDirection == 1 ? Mathf.Ceil(currentX / tileSize) * tileSize
                                  : Mathf.Floor(currentX / tileSize) * tileSize;
    }

    bool CheckWallAhead()
    {
        Vector2 direction = moveDirection == 1 ? Vector2.right : Vector2.left;
        float checkDistance = 9f; // プレイヤー1マス(5)+4=9

        Debug.DrawLine(transform.position, transform.position + (Vector3)(direction * checkDistance), Color.green);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, checkDistance, 
            LayerMask.GetMask("Platform", "Blocked","Ground","GravityBlock"));
        return hit.collider != null;
    }

    public void SetGravity(bool enable)
    {
        if (rb == null) return;

        if (enable)
        {
            rb.gravityScale = storedGravity;
        }
        else
        {
            storedGravity = rb.gravityScale;
            rb.gravityScale = 0f;
        }
    }

    public void FlipGravity()
    {
        if (rb != null)
        {
            storedGravity *= -1f;
            if (isMoving)
            {
                rb.gravityScale *= -1f;
            }
        }
    }

    public void UpdateOriginalGravity(float current)
    {
        originalGravity = Mathf.Abs(current); // 方向関係なく絶対値だけsave
    }

    public void ReversDirection()
    {
        moveDirection *= -1;
    }

    void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * moveDirection;
        transform.localScale = scale;
    }
}
