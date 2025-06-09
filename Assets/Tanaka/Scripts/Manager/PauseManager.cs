using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
	[Header("UI Elements")]
	[SerializeField] private GameObject pauseCanvas;
	[SerializeField] private Button resume;
	[SerializeField] private Button returnToTitle;

	[Header("Button Labels")]
	[SerializeField] private TextMeshProUGUI[] buttonLabels;

	private bool isResumeOpen = false;
	private int selectedButtonIndex = 0;    // ボタン選択位置

	private void Start()
	{
		if (pauseCanvas != null)
			pauseCanvas.SetActive(false);

		if (resume != null)
			resume.onClick.AddListener(() => ToggleOption(false));

		if (returnToTitle != null)
			returnToTitle.onClick.AddListener(ReturnToTitle);
	}


	private void Update()
	{
		// Tab キーでオプション開閉
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ToggleOption(!isResumeOpen);
		}
		if (!isResumeOpen) return;

		// 下キー：Return → Back に移動、Back → Return へループ
		if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
		{
			selectedButtonIndex = (selectedButtonIndex + 1) % buttonLabels.Length;
			return;
		}

		// 確定キーでそれぞれのボタン処理を呼び出す
		if (Input.GetKeyDown(KeyCode.Return))
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

	private void ToggleOption(bool open)
	{
		isResumeOpen = open;
		pauseCanvas.SetActive(open);
		Time.timeScale = open ? 0f : 1f;
	}

	private void ReturnToTitle()
	{
		Time.timeScale = 1f;
		PlayerPrefs.Save();
		SceneManager.LoadScene("Title");
	}
}
