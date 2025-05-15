using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class StageSelectManager : MonoBehaviour
{
	[Header("ステージボタン")]
	[SerializeField] private GameObject stageButtonPrefab;
	[SerializeField] private GameObject stagePlaceholderPrefab;
	[SerializeField] private Transform[] pageParents;

	[Header("ページ切替用矢印")]
	[SerializeField] public GameObject leftArrowPrefab;
	[SerializeField] public GameObject rightArrowPrefab;

	[Header("PageSlider 参照")]
	[SerializeField] private PageSlider pageSlider;

	[Header("デバッグ用")]
	[SerializeField] private bool useDebug = true;
	[SerializeField, Range(1, 10)] private int maxUnlockedStage = 1;


	private const int totalStages = 10;
	private const int stagesPerPage = 9;

	private GameObject firstSelectable;

	void Start()
	{
		// デバッグモード：ステージクリア情報を一時的にセット
		if (useDebug)
		{
			PlayerPrefs.DeleteAll();


			for (int i = 1; i <= totalStages; i++)
			{
				bool isCleared = (i <= maxUnlockedStage);
				PlayerPrefs.SetInt($"StageCleared_{i}", isCleared ? 1 : 0);
			}
			PlayerPrefs.Save(); // ※不要だけど明示的に
		}

		InitializePagePositions();
		CreateStageButtons();
		UpdateNavigation();

		// 最初の選択をセット
		if (firstSelectable != null)
			EventSystem.current.SetSelectedGameObject(firstSelectable);
	}

	private void InitializePagePositions()
	{
		// PageSlider と合わせておく
		pageSlider.Pages[0].anchoredPosition = Vector2.zero;
		float w = Screen.width;
		for (int i = 1; i < pageSlider.Pages.Length; i++)
			pageSlider.Pages[i].anchoredPosition = new Vector2(w, 0);
	}

	private void CreateStageButtons()
	{
		for (int i = 1; i <= totalStages; i++)
		{
			int pageIndex = (i - 1) / stagesPerPage;
			Transform parent = pageParents[pageIndex];

			// 前ステージ未クリアなら非表示に skip（ステージ1は常に表示）
			bool isCleared = (i <= maxUnlockedStage);
			PlayerPrefs.SetInt($"StageCleared_{i - 1}", isCleared ? 1 : 0);

			GameObject btn = Instantiate(stageButtonPrefab, parent);
			btn.name = $"Stage_{i}";

			// テキスト表示
			var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
			if (tmp != null) tmp.text = i.ToString();

			// ボタン
			var button = btn.GetComponent<Button>();
			btn.SetActive(isCleared);

			if (isCleared && button != null)
			{
				button.interactable = true;
				int sn = i;
				button.onClick.AddListener(() => Debug.Log($"Stage {sn} selected!"));
				if (firstSelectable == null)
					firstSelectable = btn;
			}
		}
		PlayerPrefs.Save();
		Debug.Log("StageCleared_9 = " + PlayerPrefs.GetInt("StageCleared_9", 0));
	}

	private void UpdateNavigation()
	{
		bool page2Enabled = PlayerPrefs.GetInt("StageCleared_9", 0) == 1;

		// ページ2 全体を表示／非表示
		if (pageParents.Length > 1)
			pageParents[1].gameObject.SetActive(page2Enabled);

		// 右矢印：ステージ9クリア済みで、現在ページが0のときのみ表示
		rightArrowPrefab.SetActive(page2Enabled && pageSlider.CurrentPage == 0);

		// 左矢印：ステージ9クリア済みで、現在ページが1のときのみ表示
		leftArrowPrefab.SetActive(page2Enabled && pageSlider.CurrentPage == 1);
	}

	public void OnClick_Left()
	{
		pageSlider.SlideToPage(0);
		StartCoroutine(WaitForSlideAndUpdate());
	}

	public void OnClick_Right()
	{
		pageSlider.SlideToPage(1);
		StartCoroutine(WaitForSlideAndUpdate());
	}

	private IEnumerator WaitForSlideAndUpdate()
	{
		// スライド中は矢印を非表示
		leftArrowPrefab.SetActive(false);
		rightArrowPrefab.SetActive(false);

		// スライド完了まで待つ
		yield return new WaitUntil(() => !pageSlider.IsSliding);

		// スライド後に矢印の表示状態を更新
		UpdateNavigation();
	}
}
