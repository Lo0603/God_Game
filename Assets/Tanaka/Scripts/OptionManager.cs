using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour
{
	[Header("UI Elements")]
	[SerializeField] private GameObject optionCanvas;
	[SerializeField] private Slider bgmSlider;
	[SerializeField] private Slider seSlider;
	[SerializeField] private Button returnToTitleButton;

	private bool isOptionOpen = false;

	private void Start()
	{
		if (optionCanvas != null)
			optionCanvas.SetActive(false); // ‰Šú”ñ•\¦

		LoadAudioSettings();

		if (returnToTitleButton != null)
			returnToTitleButton.onClick.AddListener(ReturnToTitle);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			ToggleOption();
		}
	}

	private void ToggleOption()
	{
		if (optionCanvas == null)
		{
			Debug.LogWarning("Option Canvas ‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñ");
			return;
		}

		isOptionOpen = !isOptionOpen;
		optionCanvas.SetActive(isOptionOpen);

		// ˆê’â~§Œä
		Time.timeScale = isOptionOpen ? 0f : 1f;
	}

	private void LoadAudioSettings()
	{
		int bgmVolume = PlayerPrefs.GetInt("BGMVolume", 100);
		int seVolume = PlayerPrefs.GetInt("SEVolume", 100);

		if (bgmSlider != null)
		{
			bgmSlider.value = bgmVolume / 25;
			bgmSlider.onValueChanged.AddListener((value) =>
			{
				int volume = (int)(value * 25);
				PlayerPrefs.SetInt("BGMVolume", volume);
				AudioListener.volume = volume / 100f; // ‰¼‚ÌBGM’²®
			});
		}

		if (seSlider != null)
		{
			seSlider.value = seVolume / 25;
			seSlider.onValueChanged.AddListener((value) =>
			{
				int volume = (int)(value * 25);
				PlayerPrefs.SetInt("SEVolume", volume);
				// SE‚Ì‰¹—Ê‚ğ‚±‚±‚Å“K—p
			});
		}
	}

	private void ReturnToTitle()
	{
		// ˆê’â~‚ğ‰ğœ
		Time.timeScale = 1f;
		PlayerPrefs.Save();

		// LoadingManager.LoadScene("TitleScene");
		SceneManager.LoadScene("TitleScene");
	}
}
