using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ManuManager : MonoBehaviour
{
	[Header("Main Buttons")]
	[SerializeField] private GameObject play;
	[SerializeField] private GameObject select;
	[SerializeField] private GameObject quit;

	[Header("SubMenus")]
	[SerializeField] private GameObject playSubMenu;
	[SerializeField] private GameObject exitConfirmMenu;

	[Header("SubMenus Default CursorPos")]
	[SerializeField] private GameObject playDefaultSelect;
	[SerializeField] private GameObject exitDefaultSelect;

	private void Start()
	{
		playSubMenu.SetActive(false);
		exitConfirmMenu.SetActive(false);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Backspace))
		{
			if (playSubMenu.activeSelf)
			{
				playSubMenu.SetActive(false);
				StartCoroutine(SelectAfterFrame(play));
			}
			else if (exitConfirmMenu.activeSelf)
			{
				exitConfirmMenu.SetActive(false);
				StartCoroutine(SelectAfterFrame(quit));
			}
		}
	}

	private System.Collections.IEnumerator SelectAfterFrame(GameObject target)
	{
		EventSystem.current.SetSelectedGameObject(null);
		yield return null;  // 1フレーム待つ
		EventSystem.current.SetSelectedGameObject(target);
	}


	public void OnPlay()
	{
		exitConfirmMenu.SetActive(false);
		playSubMenu.SetActive(true);

		// はじめからボタンにカーソル移動
		EventSystem.current.SetSelectedGameObject(playDefaultSelect);
	}


	public void OnExit()
	{
		playSubMenu.SetActive(false);
		exitConfirmMenu.SetActive(true);

		// Yesボタンにカーソル移動
		EventSystem.current.SetSelectedGameObject(exitDefaultSelect);
	}

	// Optionに遷移する
	public void OnOption()
	{
		SceneManager.LoadScene("Option");
	}

	// ゲームを終了する
	public void OnExitYes()
	{
#if UNITY_EDITOR
		// 再生モードを解除
		UnityEditor.EditorApplication.isPlaying = false;
#else
		// アプリケーションを終了
		Application.Quit();
#endif
	}

	// ExitConfirmMenuを閉じる
	public void OnExitNo()
	{
		exitConfirmMenu.SetActive(false);
		StartCoroutine(SelectAfterFrame(quit));
	}

	// Stage1に遷移する
	public void OnStartGame()
	{
		SceneManager.LoadScene("Stage1");
	}

	// StageSelectに遷移する
	public void OnStageSelect()
	{
		SceneManager.LoadScene("StageSelect");
	}
}
