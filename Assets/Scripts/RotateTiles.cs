using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RotateTiles : MonoBehaviour
{
    public Tilemap tilemap; // Tilemapコンポーネント参照

    // タイルを移動させる関数
    public void MoveTile(Vector3Int originalPosition, Vector3Int newPosition)
    {
        TileBase tileToMove = tilemap.GetTile(originalPosition); // 元の位置からタイルを取り込む
        if (tileToMove != null)
        {
            tilemap.SetTile(originalPosition, null); // 元の位置のタイル除去
            tilemap.SetTile(newPosition, tileToMove); // 新しい位置にタイル設定
        }
    }
}
