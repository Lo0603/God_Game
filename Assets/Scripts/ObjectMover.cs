using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public Vector2 gridMove = new Vector2(1.0f, 1.0f); // この値はタイルの大きさに合わせて調整
    private GameObject mouseObject;
    private CameraFollow cameraFollow;
    // Start is called before the first frame update
    void Start()
    {
        // Sceneでマウスオブジェクトとカメラフォローコンポーネントを検索して割り当て
        mouseObject = GameObject.Find("HandCursor"); // 「HandCursor」は、マウス オブジェクトの名前と一致する必要があります。
        cameraFollow = GameObject.FindObjectOfType<CameraFollow>(); //シーンでCameraFollowコンポーネントを探します。
    }

    void Update()
    {
        MoveObject();
        HandleBackspace();
        Debug.Log("Moving " + gridMove + " units.");
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
            // 四角形の最後の位置を保存し、各座標を四捨五入してタイルマップに合わせる
            Vector3 lastPosition = new Vector3(Mathf.Round(transform.position.x), 
                Mathf.Round(transform.position.y), transform.position.z);
            Destroy(gameObject); // 現在のオブジェクト削除
            if (mouseObject)
            {
                mouseObject.SetActive(true); // マウスオブジェクトの有効化
                mouseObject.transform.position = lastPosition;
            }
            if (cameraFollow && mouseObject)
            {
                cameraFollow.SetTarget(mouseObject.transform); // カメラターゲットをマウスオブジェクトに設定
            }
        }
    }
}
