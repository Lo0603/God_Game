using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultMenuLoader : MonoBehaviour
{
	[Header("Menu Builder")]
	[SerializeField] private MenuBuilder menuBuilder;

	[Header("Next Swap States")]
	[SerializeField] private SpriteState next;

	[Header("Play Again Swap States")]
	[SerializeField] private SpriteState again;

	[Header("Top Swap States")]
	[SerializeField] private SpriteState top;

	[Header("Stage Settings")]
	[SerializeField] private int totalStages = 10;  // 全ステージ数

	private void OnEnable() => BuildMenu();
	private void Start() => BuildMenu();

	private void BuildMenu()
	{
		menuBuilder.Clear();

		// 今のステージ番号をシーン名から取得
		string sceneName = SceneManager.GetActiveScene().name;
		int currentStage = 0;
		if (sceneName.StartsWith("Stage") &&
			int.TryParse(sceneName.Substring("Stage".Length), out var num))
		{
			currentStage = num;
		}

		var items = new List<MenuItemData>();

		// Next ボタン：10ステージ未満なら表示
		if (currentStage > 0 && currentStage < totalStages)
		{
			int nextStage = currentStage + 1;
			items.Add(new MenuItemData(
				"Next",
				() => LoadingManager.LoadScene($"Stage{nextStage}")/*,
				next*/
			));
		}

		items.Add(new MenuItemData(
			"Play Again",
			() => LoadingManager.LoadScene(sceneName)/*,
			again*/
		));

		items.Add(new MenuItemData(
			"Top",
			() => SceneManager.LoadSceneAsync("TitleScene")/*,
			top*/
		));

		menuBuilder.BuildMenu(items);
	}
}