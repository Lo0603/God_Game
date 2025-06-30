using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultMenuLoader : MonoBehaviour
{
	[Header("Menu Builder")]
	[SerializeField] private GameObject resultCanvas;
	[SerializeField] private MenuBuilder menuBuilder; // メニュー生成を行うビルダー

	[Header("Button Sprites (Next, Select, Title)")]
	[SerializeField] private Sprite nextSprite;
	[SerializeField] private Sprite selectSprite;
	[SerializeField] private Sprite titleSprite;

	private List<MenuItemData> items; // メニュー項目を保持するリスト

	private int totalStages = 10;  // 全ステージ数

	private void Awake()
	{
		// Canvas は最初オフにしておく
		//if (resultCanvas != null)
		//	resultCanvas.SetActive(false);
	}

	public void ShowResult(int currentStage)
	{
        Debug.Log("result canvas");
        if (resultCanvas == null || menuBuilder == null)
		{
			Debug.LogError("ResultMenuLoader: resultCanvas／menuBuilder をセットしてください");
			return;
		}

		// 1) Canvas を表示
		resultCanvas.SetActive(true);

		// 2) メニュー項目を組み立て
		items = new List<MenuItemData>();
		// Next は最終ステージ以外
		if (currentStage < totalStages)
		{
			items.Add(new MenuItemData(
				"Next",
				() => SceneManager.LoadScene($"Stage{currentStage + 1}")
			));
		}
		// Select
		items.Add(new MenuItemData(
			"Select",
			() => LoadingManager.LoadScene("Select")
		));
		items.Add(new MenuItemData(
			"Title",
			() => SceneManager.LoadScene("Title")
		));

		// 3) ボタンを生成
		menuBuilder.BuildMenu(items);

		// 4) 画像を当てる
		ApplyButtonImages();

		// 5) 拡大エフェクト + キー移動ナビを設定
		ApplyButtonScaleEffects();

		// 6) 最初のボタンにフォーカス
		var firstBtn = menuBuilder.panelParent.GetChild(0).gameObject;
		EventSystem.current.SetSelectedGameObject(firstBtn);
		ExecuteEvents.Execute(
			firstBtn,
			new BaseEventData(EventSystem.current),
			ExecuteEvents.selectHandler
		);
	}

	private void ApplyButtonImages()
	{
		var parent = menuBuilder.panelParent;
		for (int i = 0; i < parent.childCount; i++)
		{
			var btnObj = parent.GetChild(i).gameObject;

			var btn = btnObj.GetComponent<Button>();
			if (btn != null)
			{
				// Sprite を当てる
				switch (i)
				{
					case 0: if (nextSprite != null) btn.image.sprite = nextSprite; break;
					case 1: if (selectSprite != null) btn.image.sprite = selectSprite; break;
					case 2: if (titleSprite != null) btn.image.sprite = titleSprite; break;
				}
			}

			var tmp = btnObj.GetComponentInChildren<TMP_Text>();
			if (tmp != null)
				tmp.gameObject.SetActive(false);
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