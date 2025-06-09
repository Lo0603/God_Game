using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StageResetManager : MonoBehaviour
{
    private IResettable[] resettableObjects;

    void Start()
    {
        resettableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IResettable>().ToArray();
    }

    public void ResetStage()
    {
        foreach (var obj in resettableObjects)
        {
            obj.ResetState();
        }
    }
}
