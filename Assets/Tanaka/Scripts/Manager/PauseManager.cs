using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseManager : MonoBehaviour
{
	[Header("UI Elements")]
	[SerializeField] private GameObject pauseCanvas;
	[SerializeField] private Button resumeButton;
	[SerializeField] private Button restartButton;
	[SerializeField] private Button returnToTitleButton;

	[Header("Button Images (optional)")]
	[SerializeField] private Sprite resumeSprite;
	[SerializeField] private Sprite restartSprite;
	[SerializeField] private Sprite returnSprite;


	[SerializeField] private Vector3 normalScale = Vector3.one;
	[SerializeField] private Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1f);

	[Header("Key Bindings")]
	[SerializeField] private KeyCode toggleKey = KeyCode.Escape;
	[SerializeField] private KeyCode upKey = KeyCode.UpArrow;
	[SerializeField] private KeyCode downKey = KeyCode.DownArrow;
	[SerializeField] private KeyCode confirmKey = KeyCode.Return;

	private Button[] buttons;
	private int selectedButtonIndex = 0;
	private bool isPauseOpen = false;
	private float _defaultFixedDeltaTime;

	private void Start()
	{
		_defaultFixedDeltaTime = Time.fixedDeltaTime;

		// 初期セットアップ
		pauseCanvas.SetActive(false);
		Time.timeScale = 1f;

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

		buttons = new[] { resumeButton, restartButton, returnToTitleButton };

		// テキストを非表示＆画像をセット
		HideButtonText(resumeButton);
		HideButtonText(restartButton);
		HideButtonText(returnToTitleButton);
		ApplyButtonImages();

		// ナビゲーション設定（Automatic で上下キーを自動でつなぐ）
		foreach (var btn in buttons)
		{
			var nav = btn.navigation;
			nav.mode = Navigation.Mode.Automatic;
			btn.navigation = nav;
		}

		// 全ボタンを通常サイズに
		foreach (var btn in buttons)
			btn.GetComponent<RectTransform>().localScale = normalScale;
	}

	private void Update()
	{
		if (Input.GetKeyDown(toggleKey))
			TogglePause(!isPauseOpen);

		if (!isPauseOpen) return;

		if (Input.GetKeyDown(upKey))
			ChangeSelection(-1);
		else if (Input.GetKeyDown(downKey))
			ChangeSelection(+1);

		if (Input.GetKeyDown(confirmKey))
			buttons[selectedButtonIndex].onClick.Invoke();
	}

	private void TogglePause(bool open)
	{
		isPauseOpen = open;
		pauseCanvas.SetActive(open);

		if (open)
		{
			// ポーズ開始：時間を完全に止める
			Time.timeScale = 0f;
			Time.fixedDeltaTime = 0f;
			EventSystem.current.SetSelectedGameObject(null);
			selectedButtonIndex = 0;
			resumeButton.Select();
			UpdateButtonScales();
		}
		else
		{
			// ポーズ解除：元に戻す
			Time.timeScale = 1f;
			Time.fixedDeltaTime = _defaultFixedDeltaTime;
			EventSystem.current.SetSelectedGameObject(null);
		}
	}

	private void ChangeSelection(int direction)
	{
		// インデックスを回す
		selectedButtonIndex = (selectedButtonIndex + direction + buttons.Length) % buttons.Length;

		// 選択＆スケール更新
		var btn = buttons[selectedButtonIndex];
		btn.Select();
		UpdateButtonScales();

		Debug.Log($"Selected: {btn.gameObject.name} (Index {selectedButtonIndex})");
	}

	private void UpdateButtonScales()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			var rt = buttons[i].GetComponent<RectTransform>();
			rt.localScale = (i == selectedButtonIndex) ? selectedScale : normalScale;
		}
	}

	private void HideButtonText(Button btn)
	{
		var tmp = btn.GetComponentInChildren<TMP_Text>();
		if (tmp != null)
			tmp.gameObject.SetActive(false);
	}

	private void ApplyButtonImages()
	{
		if (resumeSprite != null) resumeButton.image.sprite = resumeSprite;
		if (restartSprite != null) restartButton.image.sprite = restartSprite;
		if (returnSprite != null) returnToTitleButton.image.sprite = returnSprite;
	}
}
