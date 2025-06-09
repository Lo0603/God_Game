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
			resume.onClick.AddListener(() => TogglePause(false));

		if (returnToTitle != null)
			returnToTitle.onClick.AddListener(ReturnToTitle);
	}


	private void Update()
	{
		// Tab キーでオプション開閉
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePause(!isResumeOpen);
		}
		if (!isResumeOpen) return;

		if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
		{
			selectedButtonIndex = (selectedButtonIndex + 1) % buttonLabels.Length;
			return;
		}

		// 確定キーでそれぞれのボタン処理を呼び出す
		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (selectedButtonIndex == 0)
			{
				TogglePause(false);
			}
			else if (selectedButtonIndex == 1)
			{
				ReturnToTitle();
			}
		}
	}

	private void TogglePause(bool open)
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
