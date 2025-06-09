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
			PlayerPrefs.Save(); // 念のため保存
		}

		InitializePagePositions();
		CreateStageButtons();
		UpdateNavigation();
		SetupNavigation();

		// 最初の選択をセット
		if (firstSelectable != null)
			EventSystem.current.SetSelectedGameObject(firstSelectable);
	}

	private void InitializePagePositions()
	{
		// PageSlider と合わせてページ位置を設定
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
			if (pageIndex >= pageParents.Length)
			{
				Debug.LogWarning($"pageParents[{pageIndex}] が存在しません。");
				continue;
			}

			Transform parent = pageParents[pageIndex];

			// ステージ1は常に表示、それ以降は開放されてるか
			bool isCleared = (i <= maxUnlockedStage);
			PlayerPrefs.SetInt($"StageCleared_{i - 1}", isCleared ? 1 : 0);

			// まずボタンを生成（SetActiveでON/OFF）
			GameObject btn = Instantiate(stageButtonPrefab, parent);
			btn.name = $"Stage_{i}";
			btn.SetActive(isCleared);

			// テキスト表示
			var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
			if (tmp != null) tmp.text = i.ToString();

			// ボタン有効化とイベント設定
			var button = btn.GetComponent<Button>();
			if (isCleared && button != null)
			{
				button.interactable = true;
				int sn = i;
				button.onClick.AddListener(() => Debug.Log($"Stage {sn} selected!"));

				button.onClick.AddListener(() =>
				{
					string stageName = $"Stage{sn}";
					LoadingManager.LoadScene(stageName);
				});


			}

			// 最初の選択対象として記録
			if (firstSelectable == null && isCleared)
				firstSelectable = btn;

			// プレースホルダー追加（未解放時のみ）
			if (!isCleared && stagePlaceholderPrefab != null)
			{
				GameObject placeholder = Instantiate(stagePlaceholderPrefab, parent);
				placeholder.name = $"Placeholder_{i}";
			}
		}

		PlayerPrefs.Save();
	}


	private void UpdateNavigation()
	{
		// ステージ9がクリアされていたらページ2有効
		bool page2Enabled = maxUnlockedStage > 9;

		if (pageParents.Length > 1)
			pageParents[1].gameObject.SetActive(page2Enabled);

		// 矢印の表示切替
		rightArrowPrefab.SetActive(page2Enabled && pageSlider.CurrentPage == 0);
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
		// スライド中は矢印非表示
		leftArrowPrefab.SetActive(false);
		rightArrowPrefab.SetActive(false);

		// スライド終了待ち
		yield return new WaitUntil(() => !pageSlider.IsSliding);

		// スライド後のナビゲーション更新
		UpdateNavigation();

		// フォーカスを次の矢印に移す
		if (pageSlider.CurrentPage == 0)
		{
			// ページ0なら右矢印を選択
			EventSystem.current.SetSelectedGameObject(rightArrowPrefab);
		}
		else
		{
			// ページ1なら左矢印を選択
			EventSystem.current.SetSelectedGameObject(leftArrowPrefab);
		}
	}

	private void SetupNavigation()
	{
		// 各ページ内のボタン取得
		foreach (Transform page in pageParents)
		{
			var buttons = page.GetComponentsInChildren<Button>(includeInactive: false);

			for (int i = 0; i < buttons.Length; i++)
			{
				Navigation nav = new Navigation
				{
					mode = Navigation.Mode.Explicit
				};

				int col = i % 3;
				int row = i / 3;

				// 上下左右のボタンを探す
				if (row > 0) nav.selectOnUp = buttons[i - 3];
				if (row < 2 && i + 3 < buttons.Length) nav.selectOnDown = buttons[i + 3];
				if (col > 0) nav.selectOnLeft = buttons[i - 1];
				if (col < 2 && i + 1 < buttons.Length) nav.selectOnRight = buttons[i + 1];

				buttons[i].navigation = nav;
			}
		}

		// ページ矢印との連携（Page1: ステージ6→右矢印、Page2: ステージ10→左矢印）
		if (pageParents.Length > 1)
		{
			// Page1: ステージ6 → 右矢印
			Button stage6 = GameObject.Find("Stage_6")?.GetComponent<Button>();
			if (stage6 != null && rightArrowPrefab != null)
			{
				var nav = stage6.navigation;
				nav.selectOnRight = rightArrowPrefab.GetComponent<Button>();
				stage6.navigation = nav;

				var arrowNav = rightArrowPrefab.GetComponent<Button>().navigation;
				arrowNav.selectOnLeft = stage6;
				rightArrowPrefab.GetComponent<Button>().navigation = arrowNav;
			}

			// Page2: ステージ10 → 左矢印
			Button stage10 = GameObject.Find("Stage_10")?.GetComponent<Button>();
			if (stage10 != null && leftArrowPrefab != null)
			{
				var nav = stage10.navigation;
				nav.selectOnLeft = leftArrowPrefab.GetComponent<Button>();
				stage10.navigation = nav;

				var arrowNav = leftArrowPrefab.GetComponent<Button>().navigation;
				arrowNav.selectOnRight = stage10;
				leftArrowPrefab.GetComponent<Button>().navigation = arrowNav;
			}
		}
	}
}
