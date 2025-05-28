using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingManager : MonoBehaviour
{
	public static LoadingManager Instance { get; private set; }

	[Header("UI リファレンス")]
	[SerializeField] private GameObject loadingUI;
	[SerializeField] private TextMeshProUGUI loadingText;
	[SerializeField] private TextMeshProUGUI hintText;

	[Header("ヒント ＆ タイミング")]
	[SerializeField] private string[] hintMessages;
	[SerializeField] private float minDisplayTime = 3f;
	[SerializeField] private float dotInterval = 0.5f;

	private Coroutine loadingAnimationCoroutine;
	private readonly string[] loadingDots = { "Loading.", "Loading..", "Loading..." };

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public static void LoadScene(string sceneName)
	{
		if (Instance == null)
		{
			var prefab = Resources.Load<LoadingManager>("LoadingManager");
			if (prefab != null)
			{
				Instantiate(prefab);
			}
			else
			{
				Debug.LogError("Resources に LoadingManager プレハブがありません！");
				return;
			}
		}

		Instance.StartCoroutine(Instance.LoadSceneAsync(sceneName));
	}

	private IEnumerator LoadSceneAsync(string sceneName)
	{
		SceneManager.LoadScene("LoadingScene");
		yield return null;

		// ローディングシーン内の UI を動的に取得
		loadingUI = GameObject.Find("LoadingUI");
		loadingText = GameObject.Find("LoadingText")?.GetComponent<TextMeshProUGUI>();
		hintText = GameObject.Find("HintText")?.GetComponent<TextMeshProUGUI>();

		if (loadingUI != null)
			loadingUI.SetActive(true);

		if (loadingUI != null)
			loadingUI.SetActive(true);

		// ヒント表示
		if (hintText != null && hintMessages != null && hintMessages.Length > 0)
		{
			hintText.text = hintMessages[Random.Range(0, hintMessages.Length)];
		}

		// アニメーション開始
		loadingAnimationCoroutine = StartCoroutine(AnimateLoadingDots());

		// 非同期ロード準備
		AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
		async.allowSceneActivation = false;

		// 指定時間待機
		yield return new WaitForSeconds(minDisplayTime);

		// アクティベーション
		async.allowSceneActivation = true;

		// ロード終了を待ってから UI を非表示
		while (!async.isDone)
		{
			yield return null;
		}

		if (loadingUI != null)
			loadingUI.SetActive(false);

		StopLoadingAnimation();
	}

	private IEnumerator AnimateLoadingDots()
	{
		int index = 0;
		while (true)
		{
			if (loadingText != null)
				loadingText.text = loadingDots[index % loadingDots.Length];
			index++;
			yield return new WaitForSeconds(dotInterval);
		}
	}

	private void StopLoadingAnimation()
	{
		if (loadingAnimationCoroutine != null)
		{
			StopCoroutine(loadingAnimationCoroutine);
			loadingAnimationCoroutine = null;
		}
	}
}
