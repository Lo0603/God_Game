using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColor : MonoBehaviour
{
    public float transparency = 0.5f; // �����x�ݒ�

    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = transparency; // Alpha �l�ύX
            spriteRenderer.color = color; // �F�K�p
        }
    }
}
