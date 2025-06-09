using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuLoader : MonoBehaviour
{
	[SerializeField] private MenuBuilder menuBuilder;

	// メニュー項目の定義をメンバ変数化
	private List<MenuItemData> items;

	private void OnEnable() => BuildMenu();
	private void Start() => BuildMenu();

	private void BuildMenu()
	{
		items = new List<MenuItemData>();

		items.Add(new MenuItemData(
			"Resume",
			() => ResumeGame()/*,
			Resume*/
		));

		items.Add(new MenuItemData(
			"Title",
			() => SceneManager.LoadScene("Title")/*,
			Title*/
		));

		menuBuilder.BuildMenu(items);
	}

	private void Update()
	{
		// Escape キーでトグル
		if (Input.GetKeyDown(KeyCode.T))
		{
			if (!gameObject.activeSelf)
			{
				// 開くとき：時間停止・メニュー生成・表示
				Time.timeScale = 0f;
				menuBuilder.BuildMenu(items);
				gameObject.SetActive(true);
			}
			else
			{
				// 閉じるとき：ResumeGame() と同じ処理
				ResumeGame();
			}

			Debug.Log("唯一王");
		}
	}

	private void ResumeGame()
	{
		Time.timeScale = 1f;
		gameObject.SetActive(false);
	}
}
