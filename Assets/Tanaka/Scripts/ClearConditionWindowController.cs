using UnityEngine;

public enum ConditionMenu
{
	StartGame = 0,
	Return,
	Num // €–Ú”
}

public class ClearConditionWindowController : MonoBehaviour
{
	public Animator[] menuList;
	private ConditionMenu currentMenu = ConditionMenu.StartGame;

	private bool isWaitingForInput = false;

	public void ShowWindow()
	{
		gameObject.SetActive(true);
		isWaitingForInput = true;
		menuList[(int)currentMenu].SetTrigger("OnCursor");
	}

	void Update()
	{
		if (!isWaitingForInput) return;

		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			menuList[(int)currentMenu].SetTrigger("Reset");
			currentMenu--;
			if ((int)currentMenu < 0)
				currentMenu = ConditionMenu.Num - 1;
			menuList[(int)currentMenu].SetTrigger("OnCursor");
		}

		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			menuList[(int)currentMenu].SetTrigger("Reset");
			currentMenu++;
			if ((int)currentMenu >= (int)ConditionMenu.Num)
				currentMenu = ConditionMenu.StartGame;
			menuList[(int)currentMenu].SetTrigger("OnCursor");
		}

		if (Input.GetKeyDown(KeyCode.Return))
		{
			switch (currentMenu)
			{
				case ConditionMenu.StartGame:
					StageManager.Instance.StartStage();
					break;
				case ConditionMenu.Return:
					Debug.Log("ƒ^ƒCƒgƒ‹‚Ö–ß‚é‚È‚Ç‚Ìˆ—");
					break;
			}

			gameObject.SetActive(false);
			isWaitingForInput = false;
		}
	}
}
