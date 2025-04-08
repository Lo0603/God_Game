using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleCreator : MonoBehaviour
{
    public GameObject rectanglePrefab;
    public GameObject mouseObject;
    public CameraFollow cameraFollow;
    private Vector3 initialPosition;
    private GameObject currentRectangle;
    private bool isCreating = false;
    private Vector3 gridCellSize;

    void Start()
    {
        gridCellSize = FindObjectOfType<Grid>().cellSize;  // Gridコンポーネントからセルサイズを取得
    }

    void Update()
    {
        HandleInput();
        if (isCreating && currentRectangle != null)
        {
            UpdateRectangleSize();
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isCreating)
            {
                StartCreatingRectangle();
            }
            else
            {
                FinishCreatingRectangle();
            }
        }

        // バックスペースを押して戻る
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Destroy(currentRectangle);  // 現在の四角形オブジェクトの削除
            currentRectangle = null;    // 参照除去
            isCreating = false;         // 生成モード終了
            mouseObject.SetActive(true); // マウス オブジェクトの再アクティブ化
            cameraFollow.SetTarget(mouseObject.transform); // カメラターゲットをマウスオブジェクトに変更
        }
    }


    void StartCreatingRectangle()
    {
        isCreating = true;
        initialPosition = transform.position;  // 現在のオブジェクトの位置を使用する
        currentRectangle = Instantiate(rectanglePrefab, initialPosition, Quaternion.identity);
        currentRectangle.AddComponent<ObjectMover>(); // ObjectMover コンポーネント追加
        //mouseObject.SetActive(false); 
    }

    void UpdateRectangleSize()
    {
        Vector3 currentPosition = transform.position;
        Vector3 direction = currentPosition - initialPosition;
        Sprite sprite = currentRectangle.GetComponent<SpriteRenderer>().sprite;
        float ppu = sprite.pixelsPerUnit;

        Vector3 size = new Vector3(
            Mathf.Ceil(Mathf.Abs(direction.x) / gridCellSize.x) * gridCellSize.x,
            Mathf.Ceil(Mathf.Abs(direction.y) / gridCellSize.y) * gridCellSize.y,
            1);

        // スプライトサイズを考慮したスケール調整
        size.x = size.x / (sprite.rect.width / ppu);
        size.y = size.y / (sprite.rect.height / ppu);

        Vector3 centerPosition = initialPosition + direction / 2;

        currentRectangle.transform.position = centerPosition;
        currentRectangle.transform.localScale = size;
    }

    void FinishCreatingRectangle()
    {
        isCreating = false;
        mouseObject.SetActive(false);
        cameraFollow.SetTarget(currentRectangle.transform); // カメラ保持
        //currentRectangle = null;  
    }
}
