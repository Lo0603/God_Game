using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuLoader : MonoBehaviour
{
	[SerializeField] private MenuBuilder menuBuilder; // メニュー生成を行うビルダー

	private List<MenuItemData> items; // メニュー項目を保持するリスト


	private void Start()
	{
		// メニュー項目の定義
		items = new List<MenuItemData>()
		{
			new MenuItemData(
				"Resume",
				() => ResumeGame()
			),
			new MenuItemData(
				"Title",
				() => SceneManager.LoadScene("Title")
			)
		};

		// メニュー生成
		menuBuilder.BuildMenu(items);

		// ボタン選択時にスケール変更する
		ApplyButtonScaleEffects();

		// 最初のボタンにフォーカス
		var firstBtn = menuBuilder.panelParent.GetChild(0).gameObject;
		EventSystem.current.SetSelectedGameObject(firstBtn);
		var eventData = new BaseEventData(EventSystem.current);
		ExecuteEvents.Execute(firstBtn, eventData, ExecuteEvents.selectHandler);
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
		}
	}

	private void ResumeGame()
	{
		Time.timeScale = 1f;
		gameObject.SetActive(false);
	}

	private void ApplyButtonScaleEffects()
	{
		var parent = menuBuilder.panelParent;
		int count = parent.childCount;

		for (int i = 0; i < count; i++)
		{
			var btnObj = parent.GetChild(i).gameObject;
			var btn = btnObj.GetComponent<Button>();
			if (btn == null) continue;

			// 拡大縮小用コンポーネントを追加
			var scaler = btnObj.AddComponent<ButtonScaleOnSelect>();
			// 必要ならスクリプト側で変更可能
			// scaler.normalScale   = new Vector3(1f, 1f, 1f);
			// scaler.selectedScale = new Vector3(1.2f,1.2f,1f);

			// ナビゲーションをExplicitで設定
			var nav = new Navigation { mode = Navigation.Mode.Explicit };
			if (i > 0)
				nav.selectOnLeft = parent.GetChild(i - 1).GetComponent<Button>();
			if (i < count - 1)
				nav.selectOnRight = parent.GetChild(i + 1).GetComponent<Button>();
			btn.navigation = nav;
		}
	}
}
