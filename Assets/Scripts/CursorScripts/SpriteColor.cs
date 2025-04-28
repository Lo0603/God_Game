using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColor : MonoBehaviour
{
    //public float transparency = 0.5f; // 透明度設定

    //void Start()
    //{
    //    SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    //    if (spriteRenderer != null)
    //    {
    //        Color color = spriteRenderer.color;
    //        color.a = transparency; // Alpha 値変更
    //        spriteRenderer.color = color; // 色適用
    //    }
    //}


    public float transparency = 0.5f;       // 基本透明度
    public float darkenTime = 1.0f;         // 暗くなるまでかかる時間
    public float brightenTime = 1.0f;       // 明るくなるまでかかる時間
    public Color targetColor = Color.black; // 暗くなる色

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = transparency;
            spriteRenderer.color = color;
        }
    }

    public void StartFade(System.Action onDarkComplete, System.Action onBrightComplete = null)
    {
        StartCoroutine(FadeRoutine(onDarkComplete, onBrightComplete));
    }

    private IEnumerator FadeRoutine(System.Action onDarkComplete, System.Action onBrightComplete)
    {
        // 1. 暗くなる
        float timer = 0;
        Color startColor = spriteRenderer.color;
        while (timer < darkenTime)
        {
            timer += Time.deltaTime;
            float t = timer / darkenTime;
            spriteRenderer.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        // 2. 暗くなった後->回転／反転 処理
        onDarkComplete?.Invoke();

        // 3. 明るく
        timer = 0;
        while (timer < brightenTime)
        {
            timer += Time.deltaTime;
            float t = timer / brightenTime;
            spriteRenderer.color = Color.Lerp(targetColor, startColor, t);
            yield return null;
        }

        // 明るくなったあと処理
        onBrightComplete?.Invoke();
    }


    public void StartBlinkRed(System.Action onComplete = null)
    {
        StartCoroutine(BlinkRedRoutine(onComplete));
    }

    private IEnumerator BlinkRedRoutine(System.Action onComplete)
    {
        if (spriteRenderer == null)
            yield break;

        Color originalColor = spriteRenderer.color;
        Color redColor = Color.red;
        redColor.a = originalColor.a; // 透明度はそのまま

        float blinkSpeed = 0.3f; // 点滅速度
        int blinkCount = 2; //   赤色->元色->赤色

        for (int i = 0; i < blinkCount; i++)
        {
            // 赤色変更
            float timer = 0;
            while (timer < blinkSpeed)
            {
                timer += Time.deltaTime;
                float t = timer / blinkSpeed;
                spriteRenderer.color = Color.Lerp(originalColor, redColor, t);
                yield return null;
            }

            // 元の色に変更
            timer = 0;
            while (timer < blinkSpeed)
            {
                timer += Time.deltaTime;
                float t = timer / blinkSpeed;
                spriteRenderer.color = Color.Lerp(redColor, originalColor, t);
                yield return null;
            }
        }

        // 元の色に
        spriteRenderer.color = originalColor;

        // 終わった後処理
        onComplete?.Invoke();
    }
}
