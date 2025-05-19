using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBlock : MonoBehaviour
{
    [Header("重力設定")]
    public float fakeGravity = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            fakeGravity = 3f;
            rb.gravityScale = 0f; // 最初は重力なし
        }
    }

    /// 左右反転は重力変化なし
    public void ApplyYFlipGravity()
    {
        // 実際の重力は一時削除
        if (rb != null)
        {
            rb.gravityScale = 0f;
        }
        //fakeGravity *= 1f; // 変化なし
    }

    /// 上下反転とき重力反転
    public void ApplyXFlipGravity()
    {
        if (rb != null)
        {
            rb.gravityScale = 0f;
        }
        fakeGravity *= -1f; // 重力反転
    }

    //　backspaceのとき重力適用
    public void ApplyFakeGravity()
    {
        if (rb != null)
        {
            rb.gravityScale = fakeGravity;
        }
    }

    /// 外部でfakeGravityを設定できる関数
    public void SetFakeGravity(float value)
    {
        fakeGravity = value;
    }

    public float GetFakeGravity()
    {
        return fakeGravity;
    }
}
