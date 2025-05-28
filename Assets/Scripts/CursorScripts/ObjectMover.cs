using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public Vector2 gridMove = new Vector2(5.0f, 5.0f); // この値はタイルの大きさに合わせて調整
    private GameObject mouseObject;
    private CameraFollow cameraFollow;
    private RectangleCreator rectangleCreator;
    private RotateInArea rotateInArea;
    // Start is called before the first frame update
    void Start()
    {
        // Sceneでマウスオブジェクトとカメラフォローコンポーネントを検索して割り当て
        mouseObject = GameObject.Find("HandCursor"); // 「HandCursor」は、マウス オブジェクトの名前と一致する必要があります。
        cameraFollow = GameObject.FindObjectOfType<CameraFollow>(); //シーンでCameraFollowコンポーネントを探します。

        rectangleCreator = GameObject.FindObjectOfType<RectangleCreator>();
        GameObject Square = GameObject.FindGameObjectWithTag("Square");
        if(Square != null)
        {
            rotateInArea = Square.GetComponent<RotateInArea>();
        }
    }

    void Update()
    {
        //MoveObject();
        HandleBackspace();
        //Debug.Log("Moving " + gridMove + " units.");
    }

    void MoveObject()
    {
        if (Input.GetKeyDown(KeyCode.W))
            transform.position += new Vector3(0, gridMove.y, 0);
        if (Input.GetKeyDown(KeyCode.S))
            transform.position -= new Vector3(0, gridMove.y, 0);
        if (Input.GetKeyDown(KeyCode.A))
            transform.position -= new Vector3(gridMove.x, 0, 0);
        if (Input.GetKeyDown(KeyCode.D))
            transform.position += new Vector3(gridMove.x, 0, 0);
    }

    void HandleBackspace()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (rotateInArea.IsFading()) { return; }
            RestoreObjectsInRectangle();

            GravityBlockApplyGravity();

            // 四角形の最後の位置を保存し、各座標を四捨五入してタイルマップに合わせる
            // tile単位でsnap(例：5単位)
            float snappedX = Mathf.Round(transform.position.x / 5) * 5;
            float snappedY = Mathf.Round(transform.position.y / 5) * 5;
            Vector3 snappedPosition = new Vector3(snappedX, snappedY, transform.position.z);
            Destroy(gameObject); // 現在のオブジェクト削除
            if (mouseObject)
            {
                mouseObject.SetActive(true); // マウスオブジェクトの有効化
                mouseObject.transform.position = snappedPosition;
            }
            if (cameraFollow && mouseObject)
            {
                cameraFollow.SetTarget(mouseObject.transform); // カメラターゲットをマウスオブジェクトに設定
            }
        }
    }

    void RestoreObjectsInRectangle() // 四角が閉じた後処理
    {
        if (rectangleCreator == null) return;

        foreach (GameObject obj in rectangleCreator.GetContainedObjects())
        {
            if (obj == null) continue;

            if (obj.CompareTag("Player"))
            {
                PlayerMoving moveScript = obj.GetComponent<PlayerMoving>();
                if (moveScript != null)
                {
                    moveScript.SetMoving(false); 
                    moveScript.SetGravity(true); 
                }
            }

            // 必要ならここに追加
            // if (obj.CompareTag("Enemy")) { ... }
        }
    }


    void GravityBlockApplyGravity()
    {
        Vector2 center = rectangleCreator.GetRectangleCenter();
        Vector2 size = rectangleCreator.GetRectangleSize();
        Collider2D[] colliders = Physics2D.OverlapBoxAll(center, size, 0f);

        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("GravityBlock"))
            {
                if (IsFullyInside(col, 0.2f))
                {
                    col.GetComponent<GravityBlock>()?.ApplyFakeGravity();
                }
            }
        }
    }

    bool IsFullyInside(Collider2D collider, float margin = 0.1f)
    {
        Bounds bounds = collider.bounds;

        float left = bounds.min.x;
        float right = bounds.max.x;
        float bottom = bounds.min.y;
        float top = bounds.max.y;

        float areaLeft = rectangleCreator.GetRectangleCenter().x - rectangleCreator.GetRectangleSize().x / 2f - margin;
        float areaRight = rectangleCreator.GetRectangleCenter().x + rectangleCreator.GetRectangleSize().x / 2f + margin;
        float areaBottom = rectangleCreator.GetRectangleCenter().y - rectangleCreator.GetRectangleSize().y / 2f - margin;
        float areaTop = rectangleCreator.GetRectangleCenter().y + rectangleCreator.GetRectangleSize().y / 2f + margin;

        return left >= areaLeft && right <= areaRight && bottom >= areaBottom && top <= areaTop;
    }
}
