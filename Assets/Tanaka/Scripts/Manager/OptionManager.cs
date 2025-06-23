using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
	private enum OptionItem { BGM = 0, SE = 1, }

	private OptionItem selectedItem = OptionItem.BGM;
	private bool isButtonMode = false;      // false:項目　true:ボタン
	private int selectedButtonIndex = 0;    // ボタン選択位置
	private GameObject _previousSelected;   // タイトル側の直前選択を覚えておく

	[Header("UI Elements")]
	[SerializeField] private GameObject optionCanvas;
	[SerializeField] private TextMeshProUGUI[] itemLabels;
	[SerializeField] private TextMeshProUGUI[] valueLabels;
	[SerializeField] private Button returnToTitleButton;
	[SerializeField] private Button backButton;

	[Header("Button Labels")]
	[SerializeField] private TextMeshProUGUI[] buttonLabels;

	[Header("Key Bindings")]
	[SerializeField] private KeyCode[] openKeys = { KeyCode.Tab };
	[SerializeField] private KeyCode[] upKeys = { KeyCode.UpArrow, KeyCode.W };
	[SerializeField] private KeyCode[] downKeys = { KeyCode.DownArrow, KeyCode.A };
	[SerializeField] private KeyCode[] leftKeys = { KeyCode.LeftArrow, KeyCode.S };
	[SerializeField] private KeyCode[] rightKeys = { KeyCode.RightArrow, KeyCode.D };
	[SerializeField] private KeyCode[] confirmKeys = { KeyCode.Return, KeyCode.KeypadEnter };

	private int[] bgmLevels = { 0, 25, 50, 75, 100 };
	private int[] seLevels = { 0, 25, 50, 75, 100 };
	private int bgmValue = 100;
	private int seValue = 100;
	private bool isOptionOpen = false;

	public static OptionManager Instance { get; private set; }
	public bool IsOptionOpen => isOptionOpen;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		if (optionCanvas != null)
			optionCanvas.SetActive(false);

		LoadAudioSettings();

		if (returnToTitleButton != null)
			returnToTitleButton.onClick.AddListener(ReturnToTitle);

		if (backButton != null)
			backButton.onClick.AddListener(() => ToggleOption(false));

		UpdateUI();
	}

	private void Update()
	{
		// ── Tab で Option 開閉 ──
		if (IsAnyKeyDown(openKeys))
		{
			if (!isOptionOpen)
			{
				// (1) 開く前の選択を覚えておく
				_previousSelected = EventSystem.current.currentSelectedGameObject;

				// (2) Option を開く
				ToggleOption(true);

				// (3) Option メニュー内「タイトルに戻る」ボタンを選択
				EventSystem.current.SetSelectedGameObject(returnToTitleButton.gameObject);
				var ed = new BaseEventData(EventSystem.current);
				ExecuteEvents.Execute(returnToTitleButton.gameObject, ed, ExecuteEvents.selectHandler);
			}
			else
			{
				// Option を閉じる
				ToggleOption(false);

				// (4) 覚えておいた Title メニューの選択を復元
				if (_previousSelected != null)
				{
					EventSystem.current.SetSelectedGameObject(_previousSelected);
					var ed = new BaseEventData(EventSystem.current);
					ExecuteEvents.Execute(_previousSelected, ed, ExecuteEvents.selectHandler);
				}
			}
			return;
		}

		if (!isOptionOpen) return;

		// （以下、既存の BGM/SE 項目モード ⇔ ボタンモード の処理はそのまま）
		if (!isButtonMode)
		{
			// ↓キーで項目モード→ボタンモードに切り替え
			if (selectedItem == OptionItem.SE && IsAnyKeyDown(downKeys))
			{
				isButtonMode = true;
				selectedButtonIndex = 0;
				UpdateUI();
				return;
			}
			// Up/Down：BGM ⇔ SE 移動
			if (IsAnyKeyDown(upKeys)) { selectedItem = OptionItem.BGM; UpdateUI(); }
			else if (IsAnyKeyDown(downKeys)) { selectedItem = OptionItem.SE; UpdateUI(); }
			// Left/Right：音量変更
			else if (IsAnyKeyDown(leftKeys)) { AdjustValue(-1); }
			else if (IsAnyKeyDown(rightKeys)) { AdjustValue(1); }
		}
		else
		{
			// ボタンモード：Up キーで項目モードに戻す
			if (IsAnyKeyDown(upKeys))
			{
				isButtonMode = false;
				UpdateUI();
				return;
			}
			// Left/Right で Return ⇔ Back
			if (IsAnyKeyDown(leftKeys) || IsAnyKeyDown(rightKeys))
			{
				selectedButtonIndex = (selectedButtonIndex + 1) % 2;
				UpdateUI();
				// フォーカス移動も行う
				var target = (selectedButtonIndex == 0 ? returnToTitleButton : backButton).gameObject;
				EventSystem.current.SetSelectedGameObject(target);
				var ed = new BaseEventData(EventSystem.current);
				ExecuteEvents.Execute(target, ed, ExecuteEvents.selectHandler);
				return;
			}
			// Enter で実行
			if (IsAnyKeyDown(confirmKeys))
			{
				if (selectedButtonIndex == 0) ReturnToTitle();
				else ToggleOption(false);
			}
		}
	}

	//  全キー配列をループして、いずれかが押されていれば true を返す
	private bool IsAnyKeyDown(KeyCode[] keys)
	{
		foreach (var k in keys)
		{
			if (Input.GetKeyDown(k))
				return true;
		}
		return false;
	}

	private void ToggleOption(bool open)
	{
		isOptionOpen = open;
		optionCanvas.SetActive(open);
		Time.timeScale = open ? 0f : 1f;
		UpdateUI();
	}

	private void AdjustValue(int delta)
	{
		switch (selectedItem)
		{
			case OptionItem.BGM:
				{
					int idx = System.Array.IndexOf(bgmLevels, bgmValue);
					idx = Mathf.Clamp(idx + delta, 0, bgmLevels.Length - 1);
					bgmValue = bgmLevels[idx];
					PlayerPrefs.SetInt("BGMVolume", bgmValue);
					AudioListener.volume = bgmValue / 100f;
				}
				break;

			case OptionItem.SE:
				{
					int idx = System.Array.IndexOf(seLevels, seValue);
					idx = Mathf.Clamp(idx + delta, 0, seLevels.Length - 1);
					seValue = seLevels[idx];
					PlayerPrefs.SetInt("SEVolume", seValue);
					// SE音量適用は別途
				}
				break;
		}

		UpdateUI();
	}

	private void LoadAudioSettings()
	{
		bgmValue = PlayerPrefs.GetInt("BGMVolume", 100);
		seValue = PlayerPrefs.GetInt("SEVolume", 100);
	}

	private void UpdateUI()
	{
		if (!isButtonMode)
		{
			// BGM/SE のラベル色を切り替え
			for (int i = 0; i < itemLabels.Length; i++)
			{
				bool isSelected = (i == (int)selectedItem);
				itemLabels[i].color = isSelected ? Color.yellow : Color.white;
			}
			// 数値表示
			valueLabels[0].text = bgmValue.ToString();
			valueLabels[1].text = seValue.ToString();

			// ボタンラベルは常に通常色にする
			for (int i = 0; i < buttonLabels.Length; i++)
			{
				buttonLabels[i].color = Color.white;
			}
		}
		else
		{
			for (int i = 0; i < itemLabels.Length; i++)
			{
				itemLabels[i].color = Color.black;
			}
			// 数値も通常色
			valueLabels[0].color = Color.white;
			valueLabels[1].color = Color.white;

			// 「Return」「Back」のラベル色を切り替え
			for (int i = 0; i < buttonLabels.Length; i++)
			{
				bool isSel = (i == selectedButtonIndex);
				buttonLabels[i].color = isSel ? Color.yellow : Color.black;
			}
		}
	}


	private void ReturnToTitle()
	{
		Time.timeScale = 1f;
		PlayerPrefs.Save();
		SceneManager.LoadScene("Title");
	}
}