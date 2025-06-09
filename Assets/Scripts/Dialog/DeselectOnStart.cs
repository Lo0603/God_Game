using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeselectOnStart : MonoBehaviour
{
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    void Update()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
