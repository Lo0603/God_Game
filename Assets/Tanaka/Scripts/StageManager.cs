using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
	public static StageManager Instance { get; private set; }

	[Header("ステージクリア条件")]
	public Transform goalPoint;
	public GameObject player;

	[Header("UI表示")]
	public GameObject clearConditionWindow;
	public GameObject stageClearWindow;

	[Header("タイマー")]
	public bool useCountdownTimer = false;
	public float timeLimit = 60f; // 秒
	private float timer;

	private bool stageStarted = false;
	private bool stageCleared = false;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	void Start()
	{
		Time.timeScale = 0f; // ゲーム停止
		clearConditionWindow.SetActive(true);
		timer = timeLimit;
	}

	public void StartStage()
	{
		Time.timeScale = 1f;
		stageStarted = true;
		clearConditionWindow.SetActive(false);
	}

	void Update()
	{
		if (!stageStarted || stageCleared) return;

		UpdateTimer();

		if (CheckClearCondition())
		{
			ClearStage();
		}
	}

	void UpdateTimer()
	{
		if (useCountdownTimer)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				timer = 0f;
				// タイムアップ時の処理もここで書ける
			}
		}
		else
		{
			timer += Time.deltaTime;
		}

		// ここでUIに表示するタイマー更新処理も追加可能
	}

	bool CheckClearCondition()
	{
		// 仮の条件：プレイヤーがゴール地点に近づいたらクリア
		return Vector2.Distance(player.transform.position, goalPoint.position) < 0.5f;
	}

	void ClearStage()
	{
		stageCleared = true;
		Time.timeScale = 0f;
		stageClearWindow.SetActive(true);
	}

	public void LoadNextStage(string nextSceneName)
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(nextSceneName);
	}

	public void RetryStage()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
