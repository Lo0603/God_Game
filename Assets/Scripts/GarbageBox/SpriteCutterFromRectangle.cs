using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCutterFromRectangle : MonoBehaviour
{
    [Header("カット対象オブジェクトのタグ")]
    public string targetTag = "Cuttable";

    [Header("切断基準 四角形 (RectanglePrefab)")]
    public GameObject rectangleObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            TrySmartSlice();
        }
    }

    void TrySmartSlice()
    {
        if (rectangleObject == null)
        {
            Debug.LogError("rectangleObjectが空！");
            return;
        }

        Bounds cutBounds = rectangleObject.GetComponent<Renderer>().bounds;
        float cutX1 = cutBounds.min.x;
        float cutX2 = cutBounds.max.x;
        float cutY1 = cutBounds.min.y;
        float cutY2 = cutBounds.max.y;

        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject target in targets)
        {
            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
            if (sr == null || sr.sprite == null) continue;

            Texture2D originalTex = sr.sprite.texture;
            if (!originalTex.isReadable)
            {
                Debug.LogWarning($"{target.name}のテクスチャがRead/Writeを無効にする");
                continue;
            }

            Sprite sprite = sr.sprite;
            float ppu = sprite.pixelsPerUnit;
            Rect spriteRect = sprite.rect;
            Bounds spriteBounds = sr.bounds;

            float spriteLeft = spriteBounds.min.x;
            float spriteRight = spriteBounds.max.x;
            float spriteBottom = spriteBounds.min.y;
            float spriteTop = spriteBounds.max.y;

            float unitPerPixel = (spriteBounds.size.x / spriteRect.width);

            // カットライン保存
            List<float> xCuts = new List<float> { spriteLeft };
            List<float> yCuts = new List<float> { spriteBottom };

            if (spriteLeft < cutX1 && cutX1 < spriteRight) xCuts.Add(cutX1);
            if (spriteLeft < cutX2 && cutX2 < spriteRight) xCuts.Add(cutX2);
            xCuts.Add(spriteRight);
            xCuts.Sort();

            if (spriteBottom < cutY1 && cutY1 < spriteTop) yCuts.Add(cutY1);
            if (spriteBottom < cutY2 && cutY2 < spriteTop) yCuts.Add(cutY2);
            yCuts.Add(spriteTop);
            yCuts.Sort();

            int texLeft = (int)spriteRect.x;
            int texBottom = (int)spriteRect.y;
            int texWidth = (int)spriteRect.width;
            int texHeight = (int)spriteRect.height;

            for (int xi = 0; xi < xCuts.Count - 1; xi++)
            {
                for (int yi = 0; yi < yCuts.Count - 1; yi++)
                {
                    float partLeft = xCuts[xi];
                    float partRight = xCuts[xi + 1];
                    float partBottom = yCuts[yi];
                    float partTop = yCuts[yi + 1];

                    float partWidthWorld = partRight - partLeft;
                    float partHeightWorld = partTop - partBottom;
                    if (partWidthWorld <= 0 || partHeightWorld <= 0) continue;

                    int px = Mathf.RoundToInt((partLeft - spriteLeft) / unitPerPixel);
                    int py = Mathf.RoundToInt((partBottom - spriteBottom) / unitPerPixel);
                    int pw = Mathf.RoundToInt(partWidthWorld / unitPerPixel);
                    int ph = Mathf.RoundToInt(partHeightWorld / unitPerPixel);

                    if (px + pw > texWidth || py + ph > texHeight) continue;

                    Color[] pixels = originalTex.GetPixels(texLeft + px, texBottom + py, pw, ph);
                    Texture2D partTex = new Texture2D(pw, ph, TextureFormat.RGBA32, false);
                    partTex.filterMode = FilterMode.Point;
                    partTex.wrapMode = TextureWrapMode.Clamp;
                    partTex.SetPixels(pixels);
                    partTex.Apply();

                    Sprite partSprite = Sprite.Create(partTex, new Rect(0, 0, pw, ph), new Vector2(0.5f, 0.5f), ppu);

                    GameObject piece = new GameObject(target.name + $"_piece_{xi}_{yi}");
                    piece.transform.position = new Vector3(partLeft + partWidthWorld / 2, partBottom + partHeightWorld / 2, 0);

                    var srNew = piece.AddComponent<SpriteRenderer>();
                    srNew.sprite = partSprite;
                    srNew.sortingOrder = sr.sortingOrder;

                    var col = piece.AddComponent<BoxCollider2D>();
                    col.size = new Vector2(partWidthWorld, partHeightWorld);

                    var rb = piece.AddComponent<Rigidbody2D>();
                    rb.gravityScale = 0f;
                }
            }

            target.SetActive(false);
        }
    }
}
