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
    public Tilemap tilemap;


    void Start()
    {
        if (tilemap == null)
        {
            // スタート時のタイルマップを探す
            tilemap = GameObject.FindObjectOfType<Tilemap>();
            //tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();    　　　　　　　// 名前で探す方法
            //tilemap = GameObject.FindGameObjectWithTag("Tilemap").GetComponent<Tilemap>(); // tagで探す方法
        }
    }

    void Update()
    {
        // 'L'キーを押す時、四角形内のオブジェクトを回転
        if (Input.GetKeyDown(KeyCode.L))
        {
            // 四角形プリファブのTransformから中心位置と大きさを取得
            UpdateAreaProperties();
            RotateObjectsXAxis();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            UpdateAreaProperties();
            RotateObjectsYAxis();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            UpdateAreaProperties();
            EraseTiles();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            UpdateAreaProperties();
            RotateTilesYAxis();
            RotateObjectsYAxis();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UpdateAreaProperties();
            RotateTilesXAxis();
            RotateObjectsXAxis();
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
    void RotateObjectsXAxis()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(areaCenter.position, new Vector2(areaSize.x, areaSize.y), 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Platform")) // 回転から除外するタグの確認
                continue;

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
            if (collider.CompareTag("Platform")) // 回転から除外するタグの確認
                continue;


            // 各オブジェクトをY軸を基準に180度回転させる
            collider.transform.Rotate(0, 180, 0);

            // 位置反転ロジック追加(左右位置反転)
            Vector3 pos = collider.transform.position;
            pos.x = 2 * areaCenter.position.x - pos.x; // 中心を基準にX位置を反転
            collider.transform.position = pos;
        }
    }

    void EraseTiles()
    {
        // 四角形範囲の左下隅計算（areaSizeは中心基準であるため）
        Vector3 bottomLeft = areaCenter.position - (Vector3)(areaSize / 2);

        // セル座標範囲計算
        Vector3Int minCell = tilemap.WorldToCell(bottomLeft);
        Vector3Int maxCell = tilemap.WorldToCell(areaCenter.position + (Vector3)(areaSize / 2));

        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(tilePos))
                {
                    tilemap.SetTile(tilePos, null); // タイル除去
                }
            }
        }
    }

    void RotateTilesYAxis()
    {
        Vector3 bottomLeft = areaCenter.position - (Vector3)(areaSize / 2);
        Vector3Int minCell = tilemap.WorldToCell(bottomLeft);
        Vector3Int maxCell = tilemap.WorldToCell(areaCenter.position + (Vector3)(areaSize / 2));

        List<(Vector3Int oldPos, Vector3Int newPos, TileBase tile, Matrix4x4 matrix)> tilesToMove = new();

        // 1. 保存:移動するタイル位置計算
        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector3Int oldPos = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(oldPos))
                {
                    TileBase tile = tilemap.GetTile(oldPos);
                    Matrix4x4 matrix = tilemap.GetTransformMatrix(oldPos);

                    Vector3 worldPos = tilemap.CellToWorld(oldPos) + tilemap.cellSize / 2f;
                    float mirroredX = 2 * areaCenter.position.x - worldPos.x;
                    Vector3 mirroredWorld = new Vector3(mirroredX, worldPos.y, worldPos.z);
                    Vector3Int newPos = tilemap.WorldToCell(mirroredWorld);

                    tilesToMove.Add((oldPos, newPos, tile, matrix));
                }
            }
        }

        // 2. 削除: 既存 タイル 全て 除去 (衝突 防止)
        foreach (var (oldPos, _, _, _) in tilesToMove)
        {
            tilemap.SetTile(oldPos, null);
            tilemap.SetTransformMatrix(oldPos, Matrix4x4.identity); // 초기화
        }

        // 3. コピー:新しい位置に回転した後、再配置
        foreach (var (_, newPos, tile, matrix) in tilesToMove)
        {
            Quaternion newRotation = matrix.rotation * Quaternion.Euler(0, 180, 0);
            Matrix4x4 newMatrix = Matrix4x4.TRS(Vector3.zero, newRotation, Vector3.one);

            tilemap.SetTile(newPos, tile);
            tilemap.SetTransformMatrix(newPos, newMatrix);
        }
    }


    void RotateTilesXAxis()
    {
        Vector3 bottomLeft = areaCenter.position - (Vector3)(areaSize / 2);
        Vector3Int minCell = tilemap.WorldToCell(bottomLeft);
        Vector3Int maxCell = tilemap.WorldToCell(areaCenter.position + (Vector3)(areaSize / 2));

        List<(Vector3Int oldPos, Vector3Int newPos, TileBase tile, Matrix4x4 matrix)> tilesToMove = new();

        //  1. 保存:移動するタイル位置計算
        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector3Int oldPos = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(oldPos))
                {
                    TileBase tile = tilemap.GetTile(oldPos);
                    Matrix4x4 matrix = tilemap.GetTransformMatrix(oldPos);

                    // セルの中心位置で上下反転
                    Vector3 worldPos = tilemap.CellToWorld(oldPos) + tilemap.cellSize / 2f;
                    float mirroredY = 2 * areaCenter.position.y - worldPos.y;
                    Vector3 mirroredWorld = new Vector3(worldPos.x, mirroredY, worldPos.z);
                    Vector3Int newPos = tilemap.WorldToCell(mirroredWorld);

                    tilesToMove.Add((oldPos, newPos, tile, matrix));
                }
            }
        }

        // 2. 削除: 既存 タイル 全て 除去 (衝突 防止)
        foreach (var (oldPos, _, _, _) in tilesToMove)
        {
            tilemap.SetTile(oldPos, null);
            tilemap.SetTransformMatrix(oldPos, Matrix4x4.identity);
        }

        // 3. コピー:新しい位置に回転した後、再配置
        foreach (var (_, newPos, tile, matrix) in tilesToMove)
        {
            Quaternion newRotation = matrix.rotation * Quaternion.Euler(180, 0, 0); // X축 기준 회전
            Matrix4x4 newMatrix = Matrix4x4.TRS(Vector3.zero, newRotation, Vector3.one);

            tilemap.SetTile(newPos, tile);
            tilemap.SetTransformMatrix(newPos, newMatrix);
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
