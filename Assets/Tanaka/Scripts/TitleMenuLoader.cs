using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenuLoader : MonoBehaviour
{
	[SerializeField] private MenuBuilder menuBuilder;

	[Header("Start Sprite Swap")]
	[SerializeField] private SpriteState start;

	[Header("Select Sprite Swap")]
	[SerializeField] private SpriteState select;

	[Header("Quit Sprite Swap")]
	[SerializeField] private SpriteState quit;

	[Header("Button Icon")]
	[SerializeField] private Image startIcon;
	[SerializeField] private Image selectIcon;
	[SerializeField] private Image quitIcon;

	private void OnEnable() => BuildMenu();
	private void Start() => BuildMenu();

	private void BuildMenu()
	{
		var items = new List<MenuItemData>();

		items.Add(new MenuItemData(
			"Start",
			() => LoadingManager.LoadScene("Stage1"),
			/*start*/
			startIcon
		));

		items.Add(new MenuItemData(
			"Select",
			() => SceneManager.LoadSceneAsync("Select"),
			/*select*/
			selectIcon
		));

		items.Add(new MenuItemData(
			"Quit",
			() => GameEnd(),
			/*quit*/
			quitIcon
		));

		menuBuilder.BuildMenu(items);
	}

	private void GameEnd()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}
}