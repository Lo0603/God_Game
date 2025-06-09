using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGravityBlock : MonoBehaviour, IResettable
{
    private Vector3 initialPos;
    private Quaternion initialRot;
    private Vector3 initialScale;
    private float startGravity;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;
        initialScale = transform.localScale;

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            startGravity = rb.gravityScale;
        }
    }


    public void ResetState()
    {
        transform.position = initialPos;
        transform.rotation = initialRot;
        transform.localScale = initialScale;

        if (rb != null)
        {
            rb.gravityScale = startGravity;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }
}
