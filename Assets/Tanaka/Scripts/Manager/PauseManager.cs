using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ポーズメニューの開閉と操作を管理します。
/// キー操作でボタンを選択し、選択状態で拡大エフェクトをかけます。
/// </summary>
public class PauseManager : MonoBehaviour
{
	[Header("UI Elements")]
	[SerializeField] private GameObject pauseCanvas;       // ポーズメニュー全体の Canvas
	[SerializeField] private Button resumeButton;          // 「Resume」ボタン
	[SerializeField] private Button returnToTitleButton;   // 「Return To Title」ボタン

	[Header("Button Images")]
	[SerializeField] private Sprite resumeSprite;
	[SerializeField] private Sprite returnSprite;

	[Header("Key Bindings")]
	[SerializeField] private KeyCode toggleKey = KeyCode.Escape; // メニュー開閉キー
	[SerializeField] private KeyCode upKey = KeyCode.W;      // 上移動キー
	[SerializeField] private KeyCode downKey = KeyCode.S;      // 下移動キー
	[SerializeField] private KeyCode confirmKey = KeyCode.Return; // 決定キー

	private bool isPauseOpen = false;          // ポーズメニューが開いているか
	private int selectedButtonIndex = 0;      // 現在選択中のボタンインデックス
	private Button[] buttons;                  // 操作対象ボタン配列

	private void Start()
	{
		// 初期状態
		pauseCanvas.SetActive(false);
		Time.timeScale = 1f;

		// ボタンの onClick 登録
		resumeButton.onClick.AddListener(() => TogglePause(false));
		returnToTitleButton.onClick.AddListener(ReturnToTitle);

		// ボタン配列と拡大エフェクト＋ナビゲーション適用
		buttons = new[] { resumeButton, returnToTitleButton };
		ApplyButtonScaleEffects();

		HideButtonText(resumeButton);
		HideButtonText(returnToTitleButton);

		if (resumeButton != null && resumeSprite != null)
			resumeButton.image.sprite = resumeSprite;

		if (returnToTitleButton != null && returnSprite != null)
			returnToTitleButton.image.sprite = returnSprite;
	}

	private void Update()
	{
		// トグルキーでポーズメニュー開閉
		if (Input.GetKeyDown(toggleKey))
			TogglePause(!isPauseOpen);

		if (!isPauseOpen) return;

		// 選択移動
		if (Input.GetKeyDown(upKey)) ChangeSelection(-1);
		else if (Input.GetKeyDown(downKey)) ChangeSelection(+1);

		// 決定キーで選択中のボタンを実行
		if (Input.GetKeyDown(confirmKey))
			buttons[selectedButtonIndex].onClick.Invoke();
	}

	private void HideButtonText(Button btn)
	{
		var tmp = btn.GetComponentInChildren<TMP_Text>();
		if (tmp != null)
			tmp.gameObject.SetActive(false);
	}

	private void ChangeSelection(int direction)
	{
		int count = buttons.Length;
		selectedButtonIndex = (selectedButtonIndex + direction + count) % count;

		var btnGO = buttons[selectedButtonIndex].gameObject;
		// フォーカス移動
		EventSystem.current.SetSelectedGameObject(btnGO);
		// OnSelect を強制発火して拡大させる
		var ev = new BaseEventData(EventSystem.current);
		ExecuteEvents.Execute(btnGO, ev, ExecuteEvents.selectHandler);
	}

	private void TogglePause(bool open)
	{
		isPauseOpen = open;
		pauseCanvas.SetActive(open);
		Time.timeScale = open ? 0f : 1f;

		if (open)
		{
			// 開いた時は最初のボタンを選択
			selectedButtonIndex = 0;
			ChangeSelection(0); // direction 0 で初回選択
		}
		else
		{
			// 閉じた時はフォーカス解除
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	private void ReturnToTitle()
	{
		Time.timeScale = 1f;
		PlayerPrefs.Save();
		SceneManager.LoadScene("Title");
	}

	private void ApplyButtonScaleEffects()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			var btn = buttons[i];
			// 選択拡大／非選択縮小コンポーネントを追加
			btn.gameObject.AddComponent<ButtonScaleOnSelect>();

			// Explicit ナビゲーションを設定
			var nav = new Navigation { mode = Navigation.Mode.Explicit };
			if (i > 0) nav.selectOnUp = buttons[i - 1];
			if (i < buttons.Length - 1) nav.selectOnDown = buttons[i + 1];
			btn.navigation = nav;
		}
	}
}
