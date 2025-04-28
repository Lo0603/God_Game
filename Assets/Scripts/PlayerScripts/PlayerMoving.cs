using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoving : MonoBehaviour
{
    public float moveSpeed = 2f;
    private bool isMoving = true;
    private int moveDirection = 1; // 1 = 右 , -1 = 左
    Animator anim;
    private Rigidbody2D rb;
    private float originalGravity;
    private float storedGravity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        storedGravity = rb.gravityScale;
        originalGravity = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        if (isMoving)
        {
            anim.SetBool("isWalking", true);
            transform.position += Vector3.right * moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    void HandleInput()
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            isMoving = !isMoving;
            rb.gravityScale = storedGravity;
        }

        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    moveDirection *= -1;. z
        //    FlipSprite();  // Sprite反転
        //}
    }

    public void ReversDirection()
    {
        moveDirection *= -1;
    }

    void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * moveDirection ; // 方向によって +/-
        transform.localScale = scale;
    }


    public void SetMoving(bool value)
    {
        isMoving=value;
    }

    public void SetGravity(bool enable)
    {
        if (rb != null)
        {
            if (enable)
            {
                rb.gravityScale = storedGravity;  // セーブされた値復旧
            }
            else
            {
                storedGravity = rb.gravityScale;  // セーブ
                rb.gravityScale = 0f;             // 重力削除
            }
        }
    }

    public void FlipGravity()
    {
        if (rb != null)
        {
            storedGravity *= -1f;
            if(isMoving)
            {
                rb.gravityScale *= -1f;
            }
            //rb.gravityScale = storedGravity;
        }
    }

    public void UpdateOriginalGravity(float current)
    {
        originalGravity = Mathf.Abs(current); // 方向関係なく絶対値だけsave
    }
}
