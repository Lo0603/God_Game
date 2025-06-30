using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class ResultMenuLoader : MonoBehaviour
{
	[SerializeField] private MenuBuilder menuBuilder;
	[SerializeField] private CanvasGroup resultGroup;

	[Header("Sprites")]
	[SerializeField] private Sprite nextSprite;
	[SerializeField] private Sprite selectSprite;
	[SerializeField] private Sprite titleSprite;

	public static bool IsOpen { get; private set; }

	private List<MenuItemData> items;
	private int totalStages = 9;

	private void Awake()
	{
		resultGroup.alpha = 0f;
		resultGroup.interactable = false;
		resultGroup.blocksRaycasts = false;
	}

	public void ShowResult(int currentStage)
	{
		IsOpen = true;
		resultGroup.alpha = 1f;
		resultGroup.interactable = true;
		resultGroup.blocksRaycasts = true;

		// メニュー組み立て
		items = new List<MenuItemData>();
		if (currentStage < totalStages)
			items.Add(new MenuItemData("Next", () => LoadingManager.LoadScene($"Stage{currentStage + 1}")));
		items.Add(new MenuItemData("Select", () => SceneManager.LoadScene("Select")));
		items.Add(new MenuItemData("Title", () => SceneManager.LoadScene("Title")));

		menuBuilder.BuildMenu(items);

		// 画像当て・テキスト隠し・拡大エフェクト
		ApplyButtonImages();
		ApplyButtonScaleEffects();


		// 最初のボタンにフォーカス
		var firstBtn = menuBuilder.panelParent.GetChild(0).gameObject;
		EventSystem.current.SetSelectedGameObject(firstBtn);
		var eventData = new BaseEventData(EventSystem.current);
		ExecuteEvents.Execute(firstBtn, eventData, ExecuteEvents.selectHandler);


		Debug.Log("show result");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			ShowResult(1); // デバッグ用に強制的に結果を表示
			return;
		}

		if (!IsOpen) return;
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
