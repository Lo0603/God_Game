using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RotateTiles : MonoBehaviour
{
    public Tilemap tilemap; // 타일맵 컴포넌트
    public TileBase[] rotatedTiles; // 회전된 타일 에셋 배열
    public Transform areaCenter; // 회전을 적용할 영역의 중심
    public Vector2Int areaSize; // 회전을 적용할 영역의 크기

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // R 키를 눌러 회전 실행
        {
            RotateinTiles();
        }
    }

    void RotateinTiles()
    {
        Vector3Int basePosition = tilemap.WorldToCell(areaCenter.position);
        int halfWidth = areaSize.x / 2;
        int halfHeight = areaSize.y / 2;

        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            for (int y = -halfHeight; y <= halfHeight; y++)
            {
                Vector3Int tilePosition = new Vector3Int(basePosition.x + x, basePosition.y + y, 0);
                TileBase currentTile = tilemap.GetTile(tilePosition);
                if (currentTile != null)
                {
                    int tileIndex = System.Array.IndexOf(rotatedTiles, currentTile);
                    tileIndex = (tileIndex + 1) % rotatedTiles.Length;
                    tilemap.SetTile(tilePosition, rotatedTiles[tileIndex]); // 다음 회전된 타일로 교체
                }
            }
        }
    }
}
