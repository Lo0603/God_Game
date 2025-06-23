using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenuLoader : MonoBehaviour
{
	[SerializeField] private MenuBuilder menuBuilder; // メニュー生成を行うビルダー

	[Header("Button Images (Start, Select, Quit)")]
	[SerializeField] private Sprite startSprite;
	[SerializeField] private Sprite selectSprite;
	[SerializeField] private Sprite quitSprite;

	private List<MenuItemData> items; // メニュー項目を保持するリスト


	private void Start()
	{
		// メニュー項目の定義
		items = new List<MenuItemData>()
		{
			new MenuItemData("Start",  ()=> LoadingManager.LoadScene("Stage1")),
			new MenuItemData("Select", ()=> SceneManager.LoadScene("Select")),
			new MenuItemData("Quit",   ()=> GameEnd())
		};

		// メニュー生成
		menuBuilder.BuildMenu(items);

		// ボタンの Image コンポーネントにスプライトを割り当て
		ApplyButtonImages();

		// ボタン選択時にスケール変更する
		ApplyButtonScaleEffects();

		// 最初のボタンにフォーカス
		var firstBtn = menuBuilder.panelParent.GetChild(0).gameObject;
		EventSystem.current.SetSelectedGameObject(firstBtn);
		var eventData = new BaseEventData(EventSystem.current);
		ExecuteEvents.Execute(firstBtn, eventData, ExecuteEvents.selectHandler);
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
					case 0: if (startSprite != null) btn.image.sprite = startSprite; break;
					case 1: if (selectSprite != null) btn.image.sprite = selectSprite; break;
					case 2: if (quitSprite != null) btn.image.sprite = quitSprite; break;
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

	private void GameEnd()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}
}

