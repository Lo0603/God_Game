using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateInArea : MonoBehaviour
{
    public Transform areaCenter;  // 四角形の中心を動的に更新する予定
    public Vector2 areaSize;  // 四角形のサイズを動的に更新する予定
    public GameObject rectanglePrefab;  // 四角形 プリファブ 参照
    public float rotationAngle = 90f;  // 回転角度

    void Update()
    {
        // 'L'キーを押す時、四角形内のオブジェクトを回転
        if (Input.GetKeyDown(KeyCode.L))
        {
            // 四角形プリファブのTransformから中心位置と大きさを取得
            UpdateAreaProperties();
            RotateObjects();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            UpdateAreaProperties();
            RotateObjectsYAxis();
        }
    }

    // 四角形の位置と大きさを更新する関数
    void UpdateAreaProperties()
    {
        if (rectanglePrefab != null)
        {

            SpriteRenderer spriteRenderer = rectanglePrefab.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                areaSize = rectanglePrefab.transform.localScale;
            }
            else
            {
                // SpriteRendererがない場合、TransformのlocalScaleを使用
                areaSize = rectanglePrefab.transform.localScale;
            }

            // 各次元から0.1を引くロジックを追加
            areaSize.x = Mathf.Max(0, areaSize.x - 0.1f); // 0より小さくならないよう最小値設定
            areaSize.y = Mathf.Max(0, areaSize.y - 0.1f); // 0より小さくならないよう最小値設定

            // 사각형의 중심 위치 설정
            areaCenter.position = rectanglePrefab.transform.position;
        }
    }

    // 指定された範囲内のオブジェクトを回転させる関数
    void RotateObjects()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(areaCenter.position, new Vector2(areaSize.x, areaSize.y), 0);
        foreach (Collider2D collider in colliders)
        {
            //各オブジェクトを180度回転させる
            collider.transform.Rotate(180, 0, 0);

            // 位置反転ロジック追加 (上下位置反転)
            Vector3 pos = collider.transform.position;
            pos.y = 2 * areaCenter.position.y - pos.y; // 中心を基準にY位置を反転
            collider.transform.position = pos;
        }
    }


    void RotateObjectsYAxis()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(areaCenter.position, new Vector2(areaSize.x, areaSize.y), 0);
        foreach (Collider2D collider in colliders)
        {
            // 各オブジェクトをY軸を基準に180度回転させる
            collider.transform.Rotate(0, 180, 0);

            // 位置反転ロジック追加(左右位置反転)
            Vector3 pos = collider.transform.position;
            pos.x = 2 * areaCenter.position.x - pos.x; // 中心を基準にX位置を反転
            collider.transform.position = pos;
        }
    }

    // 四角形の領域を視覚的に確認するために使用 (デバッグ目的)
    void OnDrawGizmosSelected()
    {
        if (areaCenter != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(areaCenter.position, new Vector3(areaSize.x, areaSize.y, 1));
        }
    }
}
