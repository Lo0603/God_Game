using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectCutter : MonoBehaviour
{
    [Header("Cutting Settings")]
    public string cuttableTag = "Cuttable";
    public GameObject rectangleReference;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CutSpritesPreciselyWithoutDestroy();
        }
    }

    void CutSpritesPreciselyWithoutDestroy()
    {
        if (rectangleReference == null)
        {
            Debug.LogWarning("Rectangle Referenceが設定されていません。");
            return;
        }

        Vector2 rectCenter = rectangleReference.transform.position;
        Vector2 rectSize = rectangleReference.transform.localScale;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(rectCenter, rectSize, 0f);

        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag(cuttableTag))
            {
                SplitObjectAndKeepOriginal(col.gameObject, rectCenter, rectSize);
            }
        }
    }

    void SplitObjectAndKeepOriginal(GameObject obj, Vector2 rectCenter, Vector2 rectSize)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        Sprite originalSprite = sr.sprite;
        Texture2D originalTexture = originalSprite.texture;

        if (!originalTexture.isReadable)
        {
            Debug.LogError("Texture2DのRead/Writeが有効になっていません。");
            return;
        }

        Rect worldCutRect = new Rect(rectCenter - rectSize / 2f, rectSize);

        Bounds objBounds = sr.bounds;
        Rect objWorldRect = new Rect(
            objBounds.min.x,
            objBounds.min.y,
            objBounds.size.x,
            objBounds.size.y);

        if (!objWorldRect.Overlaps(worldCutRect)) return; // 重ならなければカットしない

        // カットされた領域を計算した後、ピースを生成
        CreateCutPiece(obj, new Rect(objWorldRect.xMin, objWorldRect.yMin, worldCutRect.xMin - objWorldRect.xMin, objWorldRect.height)); // Left
        CreateCutPiece(obj, new Rect(worldCutRect.xMax, objWorldRect.yMin, objWorldRect.xMax - worldCutRect.xMax, objWorldRect.height)); // Right
        CreateCutPiece(obj, new Rect(worldCutRect.xMin, worldCutRect.yMax, worldCutRect.width, objWorldRect.yMax - worldCutRect.yMax)); // Top
        CreateCutPiece(obj, new Rect(worldCutRect.xMin, objWorldRect.yMin, worldCutRect.width, worldCutRect.yMin - objWorldRect.yMin)); // Bottom

        // 元のオブジェクトも切りっぱなしにする
        ApplyCutToOriginal(obj, worldCutRect);
    }

    void ApplyCutToOriginal(GameObject obj, Rect worldCutRect)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        Sprite sprite = sr.sprite;
        Texture2D texture = sprite.texture;
        float ppu = sprite.pixelsPerUnit;

        Rect cutTextureRect = WorldRectToTextureRect(sprite, worldCutRect, obj.transform);

        if (cutTextureRect.width <= 0 || cutTextureRect.height <= 0) return;

        Texture2D cutTexture = new Texture2D((int)cutTextureRect.width, (int)cutTextureRect.height);
        cutTexture.SetPixels(texture.GetPixels(
            (int)cutTextureRect.x, (int)cutTextureRect.y,
            (int)cutTextureRect.width, (int)cutTextureRect.height));
        cutTexture.Apply();

        sr.sprite = Sprite.Create(cutTexture, new Rect(0, 0, cutTexture.width, cutTexture.height), new Vector2(0.5f, 0.5f), ppu);
        BoxCollider2D bc = obj.GetComponent<BoxCollider2D>();
        if (bc != null)
        {
            bc.size = worldCutRect.size;
        }
    }

    void CreateCutPiece(GameObject original, Rect worldRect)
    {
        if (worldRect.width <= 0 || worldRect.height <= 0) return;

        SpriteRenderer sr = original.GetComponent<SpriteRenderer>();
        Sprite sprite = sr.sprite;
        Texture2D texture = sprite.texture;
        float ppu = sprite.pixelsPerUnit;

        Rect textureRect = WorldRectToTextureRect(sprite, worldRect, original.transform);

        if (textureRect.width <= 0 || textureRect.height <= 0) return;

        Texture2D cutTexture = new Texture2D((int)textureRect.width, (int)textureRect.height);
        cutTexture.SetPixels(texture.GetPixels(
            (int)textureRect.x, (int)textureRect.y,
            (int)textureRect.width, (int)textureRect.height));
        cutTexture.Apply();

        Sprite newSprite = Sprite.Create(cutTexture, new Rect(0, 0, cutTexture.width, cutTexture.height), new Vector2(0.5f, 0.5f), ppu);

        GameObject newObj = new GameObject("CutPiece");
        newObj.transform.position = worldRect.center;
        SpriteRenderer newSr = newObj.AddComponent<SpriteRenderer>();
        newSr.sprite = newSprite;
        newSr.sortingLayerID = sr.sortingLayerID;
        newSr.sortingOrder = sr.sortingOrder;

        BoxCollider2D bc = newObj.AddComponent<BoxCollider2D>();
        bc.size = worldRect.size;
    }

    Rect WorldRectToTextureRect(Sprite sprite, Rect worldRect, Transform objTransform)
    {
        Vector2 pivot = sprite.pivot;
        float ppu = sprite.pixelsPerUnit;
        Texture2D tex = sprite.texture;

        Vector2 localCenter = objTransform.InverseTransformPoint(worldRect.center);
        Vector2 textureCenter = pivot + new Vector2(localCenter.x * ppu, localCenter.y * ppu);

        Rect textureRect = new Rect(
            textureCenter.x - worldRect.width * ppu / 2f,
            textureCenter.y - worldRect.height * ppu / 2f,
            worldRect.width * ppu,
            worldRect.height * ppu);

        textureRect.x = Mathf.Clamp(textureRect.x, 0, tex.width);
        textureRect.y = Mathf.Clamp(textureRect.y, 0, tex.height);
        textureRect.width = Mathf.Clamp(textureRect.width, 0, tex.width - textureRect.x);
        textureRect.height = Mathf.Clamp(textureRect.height, 0, tex.height - textureRect.y);

        return textureRect;
    }

    void OnDrawGizmos()
    {
        if (rectangleReference != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(rectangleReference.transform.position, rectangleReference.transform.localScale);
        }
    }
}
