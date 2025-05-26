using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuLoader : MonoBehaviour
{
	[SerializeField] private MenuBuilder menuBuilder;

	[Header("Resume Sprite Swap")]
	[SerializeField] private SpriteState resume;

	[Header("Title Sprite Swap")]
	[SerializeField] private SpriteState title;

	private void Start()
	{
		var items = new List<MenuItemData>()
		{
			new MenuItemData("Resume Game", () => ResumeGame()/*, resume*/),
			new MenuItemData("Return To Title", () => {
				Time.timeScale = 1f;
				SceneManager.LoadSceneAsync("TitleScene");
			}/*, title*/)
		};

		menuBuilder.BuildMenu(items);
	}

	private void ResumeGame()
	{
		Time.timeScale = 1f;
		gameObject.SetActive(false);
	}
}