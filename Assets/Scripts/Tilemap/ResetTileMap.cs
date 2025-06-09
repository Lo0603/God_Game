using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResetTileMap : MonoBehaviour, IResettable
{
    private Tilemap tilemap;
    private Vector3 initialPos;
    private Quaternion initialRot;
    private Vector3 initialScale;
    private bool initialActive;

    private Dictionary<Vector3Int, (TileBase tile, Matrix4x4 matrix)> originalTiles = new();

    void Start()
    {
        tilemap = GetComponent<Tilemap>();

        initialPos = transform.position;
        initialRot = transform.rotation;
        initialScale = transform.localScale;
        initialActive = gameObject.activeSelf;

        SaveOriginalTiles();
    }

    void SaveOriginalTiles()
    {
        originalTiles.Clear();
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                originalTiles[pos] = (tilemap.GetTile(pos), tilemap.GetTransformMatrix(pos));
            }
        }
    }

    public void ResetState()
    {
        transform.position = initialPos;
        transform.rotation = initialRot;
        transform.localScale = initialScale;
        gameObject.SetActive(initialActive);

        tilemap.ClearAllTiles();
        foreach (var kv in originalTiles)
        {
            tilemap.SetTile(kv.Key, kv.Value.tile);
            tilemap.SetTransformMatrix(kv.Key, kv.Value.matrix);
        }
    }
}
