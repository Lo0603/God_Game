using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public Vector2 moveDirection = Vector2.right; // 로컬 기준 기본 방향
    public float moveSpeed = 3f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.attachedRigidbody == null) return;
        SoundManager.Instance.PlayLoopSE("ConveyorSound");
        // 월드 기준 이동 방향
        Vector2 worldDir = transform.rotation * new Vector3(moveDirection.x, moveDirection.y, 0f);

        // 충돌한 오브젝트의 상대 위치 판단
        Vector2 relativePos = other.transform.position - transform.position;
        float side = Mathf.Sign(Vector2.Dot(relativePos, transform.up)); // 위면: +1, 아래면: -1
        side = 1;

        // 상면/하면 방향으로 이동 방향 조정
        Vector2 finalMoveDir = worldDir * side;

        // 위치 이동
        other.attachedRigidbody.MovePosition(other.attachedRigidbody.position + finalMoveDir * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        SoundManager.Instance.StopLoopSE("ConveyorSound");
    }

    // 회전/반전 대응 함수
    public void FlipXAxis() => moveDirection.y *= -1; // X축 반전 시 상하 뒤집힘
    public void FlipYAxis() => moveDirection.x *= -1; // Y축 반전 시 좌우 뒤집힘
    public void Rotate90Clockwise() => moveDirection = new Vector2(moveDirection.y, -moveDirection.x);
    public void Rotate90CounterClockwise() => moveDirection = new Vector2(-moveDirection.y, moveDirection.x);
}
