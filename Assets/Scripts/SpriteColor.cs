using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColor : MonoBehaviour
{
    public float transparency = 0.5f; // 透明度設定

    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = transparency; // Alpha 値変更
            spriteRenderer.color = color; // 色適用
        }
    }
}
