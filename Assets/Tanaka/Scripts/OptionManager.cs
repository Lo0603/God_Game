using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionManager : MonoBehaviour
{
	[Header("UI Elements")]
	[SerializeField] private GameObject optionCanvas;
	[SerializeField] private TextMeshProUGUI[] itemLabels; // 項目名表示
	[SerializeField] private TextMeshProUGUI[] valueLabels; // 値表示
	[SerializeField] private Button returnToTitleButton;
	[SerializeField] private Button backButton; // オプション閉じる

	private int selectedIndex = 0;
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
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			ToggleOption(!isOptionOpen);
		}

		if (!isOptionOpen) return;

		// 上下選択
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			selectedIndex = Mathf.Max(0, selectedIndex - 1);
			UpdateUI();
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			selectedIndex = Mathf.Min(itemLabels.Length - 1, selectedIndex + 1);
			UpdateUI();
		}

		// 左右で音量変更
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			AdjustValue(-1);
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			AdjustValue(1);
		}
	}

	private void ToggleOption(bool open)
	{
		isOptionOpen = open;
		optionCanvas.SetActive(open);
		Time.timeScale = open ? 0f : 1f;
	}

	private void AdjustValue(int delta)
	{
		if (selectedIndex == 0) // BGM
		{
			int index = Mathf.Clamp(System.Array.IndexOf(bgmLevels, bgmValue) + delta, 0, bgmLevels.Length - 1);
			bgmValue = bgmLevels[index];
			PlayerPrefs.SetInt("BGMVolume", bgmValue);
			AudioListener.volume = bgmValue / 100f; // 仮の適用
		}
		else if (selectedIndex == 1) // SE
		{
			int index = Mathf.Clamp(System.Array.IndexOf(seLevels, seValue) + delta, 0, seLevels.Length - 1);
			seValue = seLevels[index];
			PlayerPrefs.SetInt("SEVolume", seValue);
			// SE 音量適用（任意）
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
		for (int i = 0; i < itemLabels.Length; i++)
		{
			bool isSelected = (i == selectedIndex);
			itemLabels[i].color = isSelected ? Color.yellow : Color.white;
		}

		valueLabels[0].text = bgmValue.ToString();
		valueLabels[1].text = seValue.ToString();
	}

	private void ReturnToTitle()
	{
		Time.timeScale = 1f;
		PlayerPrefs.Save();
		LoadingManager.LoadScene("Title");
	}
}
