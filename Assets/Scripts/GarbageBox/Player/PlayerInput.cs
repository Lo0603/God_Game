using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 입력 감지를 전담하는 스크립트
/// 외부에서 등록한 콜백을 통해 움직임 제어 트리거
/// </summary>
public class PlayerInput : MonoBehaviour
{
    public System.Action OnMoveToggle;  // B 키 입력
    public System.Action OnReverseDirection; // V 키 입력

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            OnMoveToggle?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            OnReverseDirection?.Invoke();
        }
    }
}
