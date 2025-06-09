using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReset : MonoBehaviour, IResettable
{
    private Vector3 startPos;
    private Quaternion startRot;
    private float startGravity;

    private Rigidbody2D rb;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if(rb != null )
        {
            startGravity = rb.gravityScale;
        }
    }

    public void ResetState()
    {
        transform.position = startPos;
        transform.rotation = startRot;

        anim.SetBool("isWalking", false);
        if (rb != null )
        {
            rb.gravityScale = startGravity;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }

}
