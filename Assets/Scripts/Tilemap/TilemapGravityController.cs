using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGravityController : MonoBehaviour
{
    public float fallSpeed = 5f;
    public string[] groundTags = { "Ground","Blocked","Player" };

    private Tilemap tilemap;
    private List<FallingTile> activeFallingTiles = new List<FallingTile>();

    private struct FallingTile
    {
        public Vector3Int cell;
        public TileBase tile;
        public GameObject visual;
    }

    public GameObject fallingTilePrefab;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        StartCoroutine(ApplyGravityCoroutine());
    }

    IEnumerator ApplyGravityCoroutine()
    {
        while (true)
        {
            CheckForNewFallingTiles();
            UpdateFallingTiles();
            yield return null;
        }
    }

    void CheckForNewFallingTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3 tileOffset = new Vector3(2.5f, 2.5f, 0);

        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(cell);

                if (tile != null)
                {
                    Vector3Int belowCell = cell + Vector3Int.down;

                    if (tilemap.GetTile(belowCell) == null)
                    {
                        tilemap.SetTile(cell, null);

                        Vector3 worldPos = tilemap.CellToWorld(cell) + tileOffset;

                        GameObject visual = Instantiate(fallingTilePrefab, worldPos, Quaternion.identity);
                        visual.GetComponent<SpriteRenderer>().sprite = ((Tile)tile).sprite;

                        activeFallingTiles.Add(new FallingTile
                        {
                            cell = cell,
                            tile = tile,
                            visual = visual
                        });
                    }
                }
            }
        }
    }

    void UpdateFallingTiles()
    {
        for (int i = activeFallingTiles.Count - 1; i >= 0; i--)
        {
            FallingTile ft = activeFallingTiles[i];

            if (ft.visual == null) continue;

            Vector3 moveDir = Vector3.down;
            Vector3 nextPos = ft.visual.transform.position + moveDir * fallSpeed * Time.deltaTime;

            if (HasGroundAtPosition(ft.visual.transform.position))
            {
                Vector3Int targetCell = tilemap.WorldToCell(ft.visual.transform.position + Vector3.down * 2.5f);
                tilemap.SetTile(targetCell, ft.tile);
                Destroy(ft.visual);
                activeFallingTiles.RemoveAt(i);
            }
            else
            {
                ft.visual.transform.position = nextPos;
                activeFallingTiles[i] = ft;
            }
        }
    }

    bool HasGroundAtPosition(Vector3 worldPos)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(worldPos + Vector3.down * 2.5f, new Vector2(2.5f, 0.1f), 0f);

        foreach (var hit in hits)
        {
            foreach (string tag in groundTags)
            {
                if (hit.CompareTag(tag))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
