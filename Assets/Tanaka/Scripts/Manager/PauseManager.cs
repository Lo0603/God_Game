using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;  // テキスト非表示用

public class PauseManager : MonoBehaviour
{
	[Header("UI Elements")]
	[SerializeField] private GameObject pauseCanvas;        // ポーズ全体の Canvas
	[SerializeField] private Button resumeButton;           // 再開ボタン
	[SerializeField] private Button restartButton;          // リスタートボタン
	[SerializeField] private Button returnToTitleButton;    // タイトルへ戻るボタン

	[Header("Button Images (optional)")]
	[SerializeField] private Sprite resumeSprite;           // Resume 用画像
	[SerializeField] private Sprite restartSprite;          // Restart 用画像
	[SerializeField] private Sprite returnSprite;           // Return 用画像

	[Header("Key Bindings")]
	[SerializeField] private KeyCode toggleKey = KeyCode.Escape;   // 開閉キー
	[SerializeField] private KeyCode upKey = KeyCode.UpArrow;  // 上移動キー
	[SerializeField] private KeyCode downKey = KeyCode.DownArrow;// 下移動キー
	[SerializeField] private KeyCode confirmKey = KeyCode.Return;   // 決定キー

	private Button[] buttons;    // Resume, Restart, Return の配列
	private int selectedButtonIndex = 0;
	private bool isPauseOpen = false;

	private void Start()
	{
		// 1. 最初は非表示
		pauseCanvas.SetActive(false);
		Time.timeScale = 1f;

		// 2. onClick 登録
		resumeButton.onClick.AddListener(() => TogglePause(false));
		restartButton.onClick.AddListener(() =>
		{
			Time.timeScale = 1f;
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		});
		returnToTitleButton.onClick.AddListener(() =>
		{
			Time.timeScale = 1f;
			PlayerPrefs.Save();
			SceneManager.LoadScene("Title");
		});

		// 3. ボタン配列を作り、各種エフェクト／設定を適用
		buttons = new[] { resumeButton, restartButton, returnToTitleButton };
		ApplyButtonScaleEffects();
		HideButtonText(resumeButton);
		HideButtonText(restartButton);
		HideButtonText(returnToTitleButton);
		ApplyButtonImages();
	}

	private void Update()
	{
		// ポーズ開閉
		if (Input.GetKeyDown(toggleKey))
			TogglePause(!isPauseOpen);

		if (!isPauseOpen) return;

		// 選択移動
		if (Input.GetKeyDown(upKey))
			ChangeSelection(-1);
		else if (Input.GetKeyDown(downKey))
			ChangeSelection(+1);

		// 決定
		if (Input.GetKeyDown(confirmKey))
			buttons[selectedButtonIndex].onClick.Invoke();
	}

	private void TogglePause(bool open)
	{
		isPauseOpen = open;
		pauseCanvas.SetActive(open);
		Time.timeScale = open ? 0f : 1f;
		if (open)
		{
			EventSystem.current.SetSelectedGameObject(null);
			selectedButtonIndex = 0;
			var btn = resumeButton;
			btn.Select();
			ExecuteEvents.Execute(
				btn.gameObject,
				new BaseEventData(EventSystem.current),
				ExecuteEvents.selectHandler
			);
		}
		else
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	private void ChangeSelection(int direction)
	{
		// インデックス更新
		selectedButtonIndex = (selectedButtonIndex + direction + buttons.Length) % buttons.Length;

		// Select() でフォーカス移動＆拡大
		var btn = buttons[selectedButtonIndex];
		btn.Select();
		ExecuteEvents.Execute(
			btn.gameObject,
			new BaseEventData(EventSystem.current),
			ExecuteEvents.selectHandler
		);
	}

	private void ApplyButtonScaleEffects()
	{
		foreach (var btn in buttons)
		{
			// 選択時拡大コンポーネント
			if (btn.gameObject.GetComponent<ButtonScaleOnSelect>() == null)
				btn.gameObject.AddComponent<ButtonScaleOnSelect>();

			// 自動ナビゲーション（上下キーで隣を探す）
			var nav = btn.navigation;
			nav.mode = Navigation.Mode.Automatic;
			btn.navigation = nav;
		}
	}

	private void HideButtonText(Button btn)
	{
		// TextMeshProUGUI がある場合は非表示
		var tmp = btn.GetComponentInChildren<TMP_Text>();
		if (tmp != null)
			tmp.gameObject.SetActive(false);
	}

	private void ApplyButtonImages()
	{
		// Inspector で画像をセットしている場合のみ差し替え
		if (resumeSprite != null) resumeButton.image.sprite = resumeSprite;
		if (restartSprite != null) restartButton.image.sprite = restartSprite;
		if (returnSprite != null) returnToTitleButton.image.sprite = returnSprite;
	}
}
