using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StageResetManager : MonoBehaviour
{
    private IResettable[] resettableObjects;

    public Transform player;
    public float maxX = 5f;
    public float maxY = 5;

    private Vector3 startPlayerPos;

    void Start()
    {
        resettableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IResettable>().ToArray();

        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
        }

        if (player != null)
        {
            startPlayerPos = player.position;
        }
    }

    void Update()
    {
        if (player != null && IsOutOfBounds(player.position))
        {
            ResetStage();
        }
    }
    public void ResetStage()
    {
        Debug.Log("ResetStage sussces!");
        foreach (var obj in resettableObjects)
        {
            obj.ResetState();
        }
    }

    private bool IsOutOfBounds(Vector3 pos)
    {
        return Mathf.Abs(pos.x) > maxX || Mathf.Abs(pos.y) > maxY;
    }
}
