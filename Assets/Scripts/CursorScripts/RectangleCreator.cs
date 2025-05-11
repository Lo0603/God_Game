using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleCreator : MonoBehaviour
{
    public GameObject rectanglePrefab;
    public GameObject mouseObject;
    private Vector3 initialPosition;
    private GameObject currentRectangle;
    private Vector3 gridCellSize;
    private PlayerMoving playerScript;
    private bool isCreating = false;

    // 四角を生成した時点で範囲中にあるオブジェクトをセーブするリスト
    private List<GameObject> containedObjects = new List<GameObject>();

    void Start()
    {
        gridCellSize = FindObjectOfType<Grid>().cellSize;  // Gridコンポーネントからセルサイズを取得

        // player script 探索
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null ) 
        {
            playerScript = player.GetComponent<PlayerMoving>();
        }
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
            //cameraFollow.SetTarget(mouseObject.transform); // カメラターゲットをマウスオブジェクトに変更

        }
    }


    void StartCreatingRectangle()
    {
        isCreating = true;

        if (playerScript != null)
        {
            playerScript.SetMoving(false);
            //playerScript.SetGravity(false);
        }

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

        // 四角形の中にあるオブジェクトを探す
        FindObjectsInRectangle();

        // 見つけたオブジェクトに対して望む処理をここで
        foreach (GameObject obj in containedObjects)
        {
            if (obj.CompareTag("Player"))
            {
                PlayerMoving moveScript = obj.GetComponent<PlayerMoving>();
                if (moveScript != null)
                {
                    moveScript.SetMoving(false);
                    moveScript.SetGravity(false);
                }
            }

            // 今後、他のオブジェクトも追加可能
            // if (obj.CompareTag("Enemy")) { ... }
            // if (obj.CompareTag("Item")) { ... }
        }

    }


    void FindObjectsInRectangle()    //  四角形の中のオブジェクトを探す関数
    {
        containedObjects.Clear();    // リスト初期化

        Vector2 center = currentRectangle.transform.position;
        Vector2 size = currentRectangle.transform.localScale;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(center, size, 0f);

        foreach (Collider2D collider in colliders)
        {
            containedObjects.Add(collider.gameObject);
        }
    }


    public bool IsCreating() { return isCreating; } 

    public List<GameObject> GetContainedObjects() { return containedObjects; }
}
