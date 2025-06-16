using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuLoader : MonoBehaviour
{
	[SerializeField] private MenuBuilder menuBuilder;

	[Header("Transition用スプライト配列（ボタン順：Resume, Title）")]
	[SerializeField] private Sprite[] defaultSprites;
	[SerializeField] private Sprite[] highlightedSprites;
	[SerializeField] private Sprite[] pressedSprites;
	[SerializeField] private Sprite[] disabledSprites;

	// メニュー項目の定義をメンバ変数化
	private List<MenuItemData> items;

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

		// SpriteSwap 設定を適用
		ApplySpriteSwap();

		// 最初のボタンにフォーカス
		var firstBtn = menuBuilder.panelParent.GetChild(0).gameObject;
		EventSystem.current.SetSelectedGameObject(firstBtn);
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
		Debug.Log("蒼響");
	}

	private void ResumeGame()
	{
		Time.timeScale = 1f;
		gameObject.SetActive(false);
	}

	private void ApplySpriteSwap()
	{
		Transform parent = menuBuilder.panelParent;
		int count = parent.childCount;

		for (int i = 0; i < count; i++)
		{
			var btnObj = parent.GetChild(i).gameObject;
			var btn = parent.GetChild(i).GetComponent<Button>();
			if (btn == null) continue;

			// Unity が内部で使う Image を取り出す
			var img = btn.image;
			if (img == null) continue;

			// トランジションを SpriteSwap に
			btn.transition = Selectable.Transition.SpriteSwap;
			btn.targetGraphic = img;

			// デフォルトスプライトを設定
			if (i < defaultSprites.Length && defaultSprites[i] != null)
				img.sprite = defaultSprites[i];
			else
				Debug.LogWarning($"defaultSprites[{i}] が設定されていません。");

			// 各状態用スプライトを構築
			//SpriteState ss = new SpriteState
			//{
			//	highlightedSprite = (i < highlightedSprites.Length && highlightedSprites[i] != null)
			//					  ? highlightedSprites[i]
			//					  : img.sprite,
			//	pressedSprite = (i < pressedSprites.Length && pressedSprites[i] != null)
			//					  ? pressedSprites[i]
			//					  : img.sprite,
			//	disabledSprite = (i < disabledSprites.Length && disabledSprites[i] != null)
			//					  ? disabledSprites[i]
			//					  : img.sprite
			//};
			//btn.spriteState = ss;

			var ctrl = btnObj.AddComponent<SelectionSpriteController>();
			Sprite normal = (i < defaultSprites.Length) ? defaultSprites[i] : img.sprite;
			Sprite highlighted = (i < highlightedSprites.Length) ? highlightedSprites[i] : normal;
			Sprite pressed = (i < pressedSprites.Length) ? pressedSprites[i] : normal;
			Sprite disabled = (i < disabledSprites.Length) ? disabledSprites[i] : normal;
			ctrl.Init(img, normal, highlighted, pressed, disabled);

			// Navigation を Explicit に設定して左右移動を有効化
			var nav = new Navigation { mode = Navigation.Mode.Explicit };
			if (i > 0)
				nav.selectOnLeft = parent.GetChild(i - 1).GetComponent<Button>();
			if (i < count - 1)
				nav.selectOnRight = parent.GetChild(i + 1).GetComponent<Button>();
			btn.navigation = nav;
		}
	}
}
