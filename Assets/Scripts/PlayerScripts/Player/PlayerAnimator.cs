using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 플레이어 애니메이션을 제어하는 스크립트
/// 이동/정지 등의 상태에 따라 애니메이터 상태를 변경함
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    private Animator anim;

    public void Init(Animator animator)
    {
        anim = animator;
    }

    /// <summary>
    /// 이동 중 애니메이션 설정
    /// </summary>
    public void SetWalking(bool isWalking)
    {
        if (anim != null)
        {
            anim.SetBool("isWalking", isWalking);
        }
    }
}
