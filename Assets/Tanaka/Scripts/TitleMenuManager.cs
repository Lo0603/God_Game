using UnityEngine.SceneManagement;

public class TitleMenuManager : BaseMenuManager
{
	protected override void SetupMenu()
	{
		CreateButton("はじめから", () => SceneManager.LoadScene("Stage1"));
		CreateButton("セレクト", () => SceneManager.LoadScene("Select"));
		CreateButton("終了", ShowExitConfirm);
	}

	private void ShowExitConfirm()
	{
		// 確認UI表示など
	}
}