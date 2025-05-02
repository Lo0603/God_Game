using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
	public static StageManager Instance { get; private set; }

	[Header("ステージクリア条件")]
	public GameObject player;

	[Header("UI表示")]
	public GameObject clearConditionWindow;
	public GameObject stageClearWindow;


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
	}

	public void StartStage()
	{
		Time.timeScale = 1f;
		clearConditionWindow.SetActive(false);
	}


	void ClearStage()
	{
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
