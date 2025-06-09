using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionManager : MonoBehaviour
{
	private enum OptionItem { BGM = 0, SE = 1, }

	private OptionItem selectedItem = OptionItem.BGM;

	private bool isButtonMode = false;      // false:項目　true:ボタン
	private int selectedButtonIndex = 0;    // ボタン選択位置

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
		// Tab キーでオプション開閉
		if (IsAnyKeyDown(openKeys))
		{
			ToggleOption(!isOptionOpen);
		}
		if (!isOptionOpen) return;

		if (!isButtonMode)
		{
			// 下キーで「ボタンモード」へ切り替える
			if ((int)selectedItem == (int)OptionItem.SE && IsAnyKeyDown(downKeys))
			{
				isButtonMode = true;     // ボタン（Return/Back）を選ぶ状態へ
				selectedButtonIndex = 0; // 「Return」を初期選択
				UpdateUI();
				return;
			}

			// 上キー：BGM ←→ SE 閑散移動
			if (IsAnyKeyDown(upKeys))
			{
				int idx = (int)selectedItem;
				idx = Mathf.Max(0, idx - 1);
				selectedItem = (OptionItem)idx;
				UpdateUI();
			}
			else if (IsAnyKeyDown(downKeys))
			{
				int idx = (int)selectedItem;
				idx = Mathf.Min(System.Enum.GetValues(typeof(OptionItem)).Length - 1, idx + 1);
				selectedItem = (OptionItem)idx;
				UpdateUI();
			}

			// 左右キー：BGM/SE の音量変更
			if (IsAnyKeyDown(leftKeys))
			{
				AdjustValue(-1);
			}
			else if (IsAnyKeyDown(rightKeys))
			{
				AdjustValue(1);
			}
		}
		else
		{
			// 上キーで「項目モード（SE に戻る）」に切り替える
			if (IsAnyKeyDown(upKeys))
			{
				isButtonMode = false;
				UpdateUI();
				return;
			}

			// 下キー：Return → Back に移動、Back → Return へループ
			if (IsAnyKeyDown(leftKeys) || IsAnyKeyDown(rightKeys))
			{
				selectedButtonIndex = (selectedButtonIndex + 1) % buttonLabels.Length;
				UpdateUI();
				return;
			}

			// 確定キーでそれぞれのボタン処理を呼び出す
			if (IsAnyKeyDown(confirmKeys))
			{
				if (selectedButtonIndex == 0)
				{
					// 「タイトルに戻る」処理
					ReturnToTitle();
				}
				else if (selectedButtonIndex == 1)
				{
					// 「オプションを閉じる」処理
					ToggleOption(false);
				}
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