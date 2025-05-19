using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player 전체 제어용 메인 스크립트
/// 하위 모듈: Mover, Animator, Gravity, Input을 초기화 및 연결함
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerManager : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    private PlayerMover mover;
    private PlayerAnimator playerAnimator;
    private PlayerGravity gravityController;
    private PlayerInput input;

    void Awake()
    {
        // 필수 컴포넌트 가져오기
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // 필요한 컴포넌트 붙이기
        mover = gameObject.AddComponent<PlayerMover>();
        playerAnimator = gameObject.AddComponent<PlayerAnimator>();
        gravityController = gameObject.AddComponent<PlayerGravity>();
        input = gameObject.AddComponent<PlayerInput>();

        // 초기화 연결
        mover.Init(rb);
        playerAnimator.Init(animator);
        gravityController.Init(rb);

        // 입력 이벤트 등록
        input.OnMoveToggle += ToggleMove;
        input.OnReverseDirection += ReverseDirection;
    }

    void Update()
    {
        mover.ManualUpdate();
        playerAnimator.SetWalking(mover.IsMoving);
    }

    void ToggleMove()
    {
        if (mover.IsMoving)
        {
            mover.StopMovingSmoothly();
        }
        else
        {
            mover.StartMoving();
        }
    }

    public void ReverseDirection()
    {
        int dir = mover.GetDirection();
        mover.SetDirection(-dir);

        // 좌우 반전 시 스프라이트도 뒤집기
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * -dir;
        transform.localScale = scale;
    }

    public void SetMoving(bool moving)
    {
        mover.SetMoving(moving);
    }

    public void SetGravity(bool enable)
    {
        gravityController.SetGravity(enable);
    }

    public void FlipGravity()
    {
        gravityController.FlipGravity();
    }

    public void UpdateOriginalGravity(float val)
    {
        gravityController.UpdateOriginalGravity(val);
    }
}