using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultMenuLoader : MonoBehaviour
{
	[Header("Menu Builder")]
	[SerializeField] private MenuBuilder menuBuilder; // メニュー生成を行うビルダー

	[Header("Stage Settings")]
	[SerializeField] private int totalStages = 10;  // 全ステージ数

	[Header("Button Sprites (Next, Play Again, Title)")]
	[SerializeField] private Sprite nextSprite;
	[SerializeField] private Sprite playAgainSprite;
	[SerializeField] private Sprite titleSprite;

	private List<MenuItemData> items; // メニュー項目を保持するリスト


	private void Start()
	{
		// 今のステージ番号をシーン名から取得
		string sceneName = SceneManager.GetActiveScene().name;
		int currentStage = 0;
		if (sceneName.StartsWith("Stage") &&
			int.TryParse(sceneName.Substring("Stage".Length), out var num))
		{
			currentStage = num;
		}

		// メニュー項目の定義
		items = new List<MenuItemData>();

		// Next ボタン：10ステージ未満なら表示
		if (currentStage > 0 && currentStage < totalStages)
		{
			int nextStage = currentStage + 1;
			items.Add(new MenuItemData(
				"Next",
				() => LoadingManager.LoadScene($"Stage{nextStage}")
			));
		}

		// Play Again
		items.Add(new MenuItemData(
			"Play Again",
			() => LoadingManager.LoadScene(sceneName)
		));

		// Title
		items.Add(new MenuItemData(
			"Title",
			() => SceneManager.LoadScene("Title")
		));

		// メニュー生成
		menuBuilder.BuildMenu(items);

		// ボタン画像を設定＋テキストを非表示
		ApplyButtonImagesAndHideText();

		// ボタン選択時にスケール変更する
		ApplyButtonScaleEffects();

		// 最初のボタンにフォーカス
		var firstBtn = menuBuilder.panelParent.GetChild(0).gameObject;
		EventSystem.current.SetSelectedGameObject(firstBtn);
		var eventData = new BaseEventData(EventSystem.current);
		ExecuteEvents.Execute(firstBtn, eventData, ExecuteEvents.selectHandler);
	}

	private void ApplyButtonImagesAndHideText()
	{
		var parent = menuBuilder.panelParent;
		for (int i = 0; i < parent.childCount; i++)
		{
			var btnObj = parent.GetChild(i).gameObject;
			var btn = btnObj.GetComponent<Button>();
			if (btn == null) continue;

			// 画像を割り当て
			switch (i)
			{
				case 0:
					if (nextSprite != null) btn.image.sprite = nextSprite;
					break;
				case 1:
					if (playAgainSprite != null) btn.image.sprite = playAgainSprite;
					break;
				case 2:
					if (titleSprite != null) btn.image.sprite = titleSprite;
					break;
			}

			var tmp = btnObj.GetComponentInChildren<TMP_Text>();
			if (tmp != null) tmp.gameObject.SetActive(false);
		}
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