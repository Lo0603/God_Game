using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]  // エディタでも動作
[RequireComponent(typeof(SpriteRenderer))]
public class FitSpriteToObjectSize : MonoBehaviour
{
    public Vector2 targetSize = new Vector2(1, 1);  // オブジェクトの目標サイズ

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (sr.sprite == null) return;

        Vector2 spriteSize = sr.sprite.bounds.size;

        // 現在のオブジェクトのサイズ = local Scale * sprite Size
        Vector3 newScale = transform.localScale;
        newScale.x = targetSize.x / spriteSize.x;
        newScale.y = targetSize.y / spriteSize.y;

        transform.localScale = newScale;
    }

    // エディターでもアップデートを反映
    void OnValidate()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        Update();
    }
}
