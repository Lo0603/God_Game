using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 중력 설정 및 반전, 저장 기능을 담당하는 스크립트
/// Rigidbody2D를 통해 gravityScale 제어
/// </summary>
public class PlayerGravity : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerMover mover;
    private float storedGravity;
    private float originalGravity;

    void Awake()
    {
        mover = gameObject.AddComponent<PlayerMover>();
    }

    public void Init(Rigidbody2D rigidbody)
    {
        rb = rigidbody;
        storedGravity = rb.gravityScale;
        originalGravity = rb.gravityScale;
    }

    /// <summary>
    /// 중력 활성화/비활성화 (enable=true면 복원, false면 저장 후 0으로 설정)
    /// </summary>
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

    /// <summary>
    /// 중력 방향 반전 및 저장값 반전
    /// </summary>
    public void FlipGravity()
    {
        if (rb != null)
        {
            storedGravity *= -1f;
            if (mover.IsMoving)
            {
                rb.gravityScale *= -1f;
            }
        }
    }

    /// <summary>
    /// 외부에서 중력값 갱신할 때 사용 (절대값 저장)
    /// </summary>
    public void UpdateOriginalGravity(float current)
    {
        originalGravity = Mathf.Abs(current);
    }
}
