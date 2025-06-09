using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTileMap : MonoBehaviour, IResettable
{
    private Vector3 initialPos;
    private Quaternion initialRot;
    private Vector3 initialScale;
    private bool initialActive;
    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;
        initialScale = transform.localScale;
        initialActive = gameObject.activeSelf;
    }

 
    public void ResetState()
    {
        transform.position = initialPos;
        transform.rotation = initialRot;
        transform.localScale = initialScale;
        gameObject.SetActive(initialActive);
    }
}
