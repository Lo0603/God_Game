using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RotateInArea : MonoBehaviour
{
    public Transform areaCenter;  // 四角形の中心を動的に更新する予定
    public Vector2 areaSize;  // 四角形のサイズを動的に更新する予定
    public GameObject rectanglePrefab;  // 四角形 プリファブ 参照
    public float rotationAngle = 90f;  // 回転角度
    public List<Tilemap> tilemaps = new List<Tilemap>();

    private ParticleSpawner particleSpawner;
    private RectangleCreator rectangleCreator;
    private bool canOperate = true;  // 操作可能かどうか
    private bool isFading = false;   // 演出中か確認
    void Start()
    {
    

        if (tilemaps.Count == 0)
        {
            tilemaps.AddRange(GameObject.FindObjectsOfType<Tilemap>());
            //tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();    　　　　　　　// 名前で探す方法
            //tilemap = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<Tilemap>(); // tagで探す方法
        }

        rectangleCreator = FindObjectOfType<RectangleCreator>();
        particleSpawner = FindObjectOfType<ParticleSpawner>();
    }

    void Update()
    {
        // 四角生成中なら操作禁止
        canOperate = rectangleCreator == null ? true : rectangleCreator.IsCreating() == false;

        if (!canOperate || isFading) return;

        // tile 削除処理
        if (Input.GetKeyDown(KeyCode.X))
        {
            UpdateAreaProperties();
            EraseTiles();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            UpdateAreaProperties();

            if (HasBlockedTagInArea(new string[] { "Blocked" , "Goal"}))
            {
                isFading = true;
                SpriteColor spriteColor = rectanglePrefab.GetComponent<SpriteColor>();
                spriteColor.StartBlinkRed(() => {
                    // 点滅後処理

                    isFading = false;
                    Debug.Log("禁止");
                });
                SoundManager.Instance.PlaySE("反転できない時２");
                Debug.Log("回転不可");
                return;
            }
            SoundManager.Instance.PlaySE("反転１");
            if (particleSpawner != null) // particle
                particleSpawner.PlayParticlesAround(rectanglePrefab.transform.position, rectanglePrefab.transform.localScale);
            StartFadeAndRotate(isXAxis: false);  // Y軸回転
        }

        if (Input.GetKeyDown(KeyCode.E))
        {

            UpdateAreaProperties();

            if (HasBlockedTagInArea(new string[] { "Blocked", "Goal" }))
            {
                isFading = true;
                SpriteColor spriteColor = rectanglePrefab.GetComponent<SpriteColor>();
                spriteColor.StartBlinkRed(() => {
                    // 点滅後処理

                    isFading = false;
                    Debug.Log("禁止");
                });
                SoundManager.Instance.PlaySE("反転できない時２");
                Debug.Log("回転不可");
                return;
            }
            SoundManager.Instance.PlaySE("反転１");
            if (particleSpawner != null) // particle
                particleSpawner.PlayParticlesAround(rectanglePrefab.transform.position, rectanglePrefab.transform.localScale);
            StartFadeAndRotate(isXAxis: true);   // X軸回転
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

            // 四角形の中心位置設定
            areaCenter.position = rectanglePrefab.transform.position;
        }
    }


    void RotateObjectsXAxis()
    {
        bool playerFound = false;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(areaCenter.position, areaSize, 0f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Platform"))
                continue;

            //  완전히 안에 들어와야 회전
            if (!IsFullyInside(collider, 0.2f))
                continue;

            if (collider.CompareTag("GravityBlock"))
            {
                collider.GetComponent<GravityBlock>()?.ApplyXFlipGravity();
            }


            collider.transform.Rotate(180, 0, 0);

            Vector3 pos = collider.transform.position;
            pos.y = 2 * areaCenter.position.y - pos.y;
            collider.transform.position = pos;

            // 플레이어인지 체크
            if (collider.CompareTag("Player"))
            {
                playerFound = true;
            }
        }

        // 루프 끝나고 나서 한번만
        if (playerFound)
        {
            FlipPlayerGravity();
        }
    }

    void RotateObjectsYAxis()
    {
        bool playerFound = false;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(areaCenter.position, areaSize, 0f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Platform"))
                continue;

            //  完全に中に入ってこそ回転
            if (!IsFullyInside(collider, 0.2f))
                continue;

            if (collider.CompareTag("GravityBlock"))
            {
                collider.GetComponent<GravityBlock>()?.ApplyYFlipGravity();
            }


            collider.transform.Rotate(0, 180, 0);

            Vector3 pos = collider.transform.position;
            pos.x = 2 * areaCenter.position.x - pos.x;
            collider.transform.position = pos;

            if (collider.CompareTag("Player"))
            {
                playerFound = true;
            }
        }
        if (playerFound)
        {
            FlipPlayerDirection();
        }
    }

    //// 指定された範囲内のオブジェクトを回転させる関数
    //void RotateObjectsXAxis()
    //{
    //    Collider2D[] colliders = Physics2D.OverlapBoxAll(areaCenter.position, new Vector2(areaSize.x, areaSize.y), 0);
    //    foreach (Collider2D collider in colliders)
    //    {
    //        if (collider.CompareTag("Platform")) // 回転から除外するタグの確認
    //            continue;

    //        //各オブジェクトを180度回転させる
    //        collider.transform.Rotate(180, 0, 0);

    //        // 位置反転ロジック追加 (上下位置反転)
    //        Vector3 pos = collider.transform.position;
    //        pos.y = 2 * areaCenter.position.y - pos.y; // 中心を基準にY位置を反転
    //        collider.transform.position = pos;
    //        FlipPlayerGravity();  // 重力反転
    //    }
    //}


    //void RotateObjectsYAxis()
    //{
    //    Collider2D[] colliders = Physics2D.OverlapBoxAll(areaCenter.position, new Vector2(areaSize.x, areaSize.y), 0);
    //    foreach (Collider2D collider in colliders)
    //    {
    //        if (collider.CompareTag("Platform")) // 回転から除外するタグの確認
    //            continue;


    //        // 各オブジェクトをY軸を基準に180度回転させる
    //        collider.transform.Rotate(0, 180, 0);

    //        // 位置反転ロジック追加(左右位置反転)
    //        Vector3 pos = collider.transform.position;
    //        pos.x = 2 * areaCenter.position.x - pos.x; // 中心を基準にX位置を反転
    //        collider.transform.position = pos;
    //        FlipPlayerDirection(); // 移動方向反転
    //    }
    //}

    void EraseTiles()
    {
        foreach (var map in tilemaps)
        {
            // 四角形範囲の左下隅計算（areaSizeは中心基準であるため）
            Vector3 bottomLeft = areaCenter.position - (Vector3)(areaSize / 2);

            // セル座標範囲計算
            Vector3Int minCell = map.WorldToCell(bottomLeft);
            Vector3Int maxCell = map.WorldToCell(areaCenter.position + (Vector3)(areaSize / 2));

            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x, y, 0);
                    if (map.HasTile(tilePos))
                    {
                        map.SetTile(tilePos, null); // タイル除去
                    }
                }
            }
        }
    }

    void RotateTilesYAxis()
    {
        foreach (var map in tilemaps)
        {
            Vector3 bottomLeft = areaCenter.position - (Vector3)(areaSize / 2);
            Vector3Int minCell = map.WorldToCell(bottomLeft);
            Vector3Int maxCell = map.WorldToCell(areaCenter.position + (Vector3)(areaSize / 2));

            List<(Vector3Int oldPos, Vector3Int newPos, TileBase tile, Matrix4x4 matrix)> tilesToMove = new();

            // 1. 保存:移動するタイル位置計算
            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    Vector3Int oldPos = new Vector3Int(x, y, 0);
                    if (map.HasTile(oldPos))
                    {
                        TileBase tile = map.GetTile(oldPos);
                        Matrix4x4 matrix = map.GetTransformMatrix(oldPos);

                        Vector3 worldPos = map.CellToWorld(oldPos) + map.cellSize / 2f;
                        float mirroredX = 2 * areaCenter.position.x - worldPos.x;
                        Vector3 mirroredWorld = new Vector3(mirroredX, worldPos.y, worldPos.z);
                        Vector3Int newPos = map.WorldToCell(mirroredWorld);

                        tilesToMove.Add((oldPos, newPos, tile, matrix));
                    }
                }
            }

            // 2. 削除: 既存 タイル 全て 除去 (衝突 防止)
            foreach (var (oldPos, _, _, _) in tilesToMove)
            {
                map.SetTile(oldPos, null);
                map.SetTransformMatrix(oldPos, Matrix4x4.identity);
            }

            // 3. コピー:新しい位置に回転した後、再配置
            foreach (var (_, newPos, tile, matrix) in tilesToMove)
            {
                Quaternion newRotation = matrix.rotation * Quaternion.Euler(0, 180, 0);
                Matrix4x4 newMatrix = Matrix4x4.TRS(Vector3.zero, newRotation, Vector3.one);

                map.SetTile(newPos, tile);
                map.SetTransformMatrix(newPos, newMatrix);
            }
        }
    }


    void RotateTilesXAxis()
    {
        foreach (var map in tilemaps)
        {
            Vector3 bottomLeft = areaCenter.position - (Vector3)(areaSize / 2);
            Vector3Int minCell = map.WorldToCell(bottomLeft);
            Vector3Int maxCell = map.WorldToCell(areaCenter.position + (Vector3)(areaSize / 2));

            List<(Vector3Int oldPos, Vector3Int newPos, TileBase tile, Matrix4x4 matrix)> tilesToMove = new();

            // 1. 保存:移動するタイル位置計算
            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    Vector3Int oldPos = new Vector3Int(x, y, 0);
                    if (map.HasTile(oldPos))
                    {
                        TileBase tile = map.GetTile(oldPos);
                        Matrix4x4 matrix = map.GetTransformMatrix(oldPos);

                        Vector3 worldPos = map.CellToWorld(oldPos) + map.cellSize / 2f;
                        float mirroredY = 2 * areaCenter.position.y - worldPos.y;
                        Vector3 mirroredWorld = new Vector3(worldPos.x, mirroredY, worldPos.z);
                        Vector3Int newPos = map.WorldToCell(mirroredWorld);

                        tilesToMove.Add((oldPos, newPos, tile, matrix));
                    }
                }
            }

            // 2. 削除: 既存 タイル 全て 除去 (衝突 防止)
            foreach (var (oldPos, _, _, _) in tilesToMove)
            {
                map.SetTile(oldPos, null);
                map.SetTransformMatrix(oldPos, Matrix4x4.identity);
            }

            // 3. コピー:新しい位置に回転した後、再配置
            foreach (var (_, newPos, tile, matrix) in tilesToMove)
            {
                Quaternion newRotation = matrix.rotation * Quaternion.Euler(180, 0, 0);
                Matrix4x4 newMatrix = Matrix4x4.TRS(Vector3.zero, newRotation, Vector3.one);

                map.SetTile(newPos, tile);
                map.SetTransformMatrix(newPos, newMatrix);
            }
        }
    }



    void StartFadeAndRotate(bool isXAxis)
    {
        if (rectanglePrefab != null)
        {
            SpriteColor spriteColor = rectanglePrefab.GetComponent<SpriteColor>();
            if (spriteColor != null)
            {
                isFading = true; // 始まったときtrue

                spriteColor.StartFade(
                    () => {
                        // 暗くなった後->回転／反転 処理
                        if (isXAxis)
                        {
                            RotateTilesXAxis();
                            RotateObjectsXAxis();
                        }
                        else
                        {
                            RotateTilesYAxis();
                            RotateObjectsYAxis();
                        }
                    },
                    () => {
                        // 明るくなった後処理
                        isFading = false;
                    }
                );
            }
            else
            {
                // SpriteColorがないとすぐに回転
                if (isXAxis)
                {
                    RotateTilesXAxis();
                    RotateObjectsXAxis();
                }
                else
                {
                    RotateTilesYAxis();
                    RotateObjectsYAxis();
                }
            }
        }
    }




    void FlipPlayerGravity()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
       
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            PlayerMoving playerScript = player.GetComponent<PlayerMoving>();
            if (rb != null && playerScript != null)
            {
                playerScript.FlipGravity();  // セーブされた重力値を反転
            }
        }
    }

    void FlipPlayerDirection()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerMoving move = player.GetComponent<PlayerMoving>();
            if (move != null)
            {
                move.ReversDirection();
            }
        }
    }

    // tilemapCollider更新
    //void ForceRefreshTilemapCollider()
    //{
    //    TilemapCollider2D collider = tilemap.GetComponent<TilemapCollider2D>();
    //    if (collider != null)
    //    {
    //        collider.enabled = false;
    //        collider.enabled = true;
    //    }
    //}


    // 四角の中に禁止TAGオブジェクトがあるか確認
    bool HasBlockedTagInArea(string[] blockedTags)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(areaCenter.position, new Vector2(areaSize.x, areaSize.y), 0);

        foreach (Collider2D collider in colliders)
        {
            foreach (string tag in blockedTags)
            {
                if (collider.CompareTag(tag))
                {
                    return true; // 止されたTAG発見
                }
            }
        }
        return false;
    }



    bool IsFullyInside(Collider2D collider, float margin = 0.1f)
    {
        Bounds bounds = collider.bounds;

        float left = bounds.min.x;
        float right = bounds.max.x;
        float bottom = bounds.min.y;
        float top = bounds.max.y;

        float areaLeft = areaCenter.position.x - areaSize.x / 2f - margin;
        float areaRight = areaCenter.position.x + areaSize.x / 2f + margin;
        float areaBottom = areaCenter.position.y - areaSize.y / 2f - margin;
        float areaTop = areaCenter.position.y + areaSize.y / 2f + margin;

        return left >= areaLeft && right <= areaRight && bottom >= areaBottom && top <= areaTop;
    }


    public bool IsFading() { return isFading; }


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
