using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenuLoader : MonoBehaviour
{
	[SerializeField] private MenuBuilder menuBuilder;

	[Header("Transition用スプライト配列（ボタン順：Start, Select, Quit）")]
	[SerializeField] private Sprite[] defaultSprites;
	[SerializeField] private Sprite[] highlightedSprites;
	[SerializeField] private Sprite[] pressedSprites;
	[SerializeField] private Sprite[] disabledSprites;

	private List<MenuItemData> items;

	private GameObject _lastSelected;

	private void Start()
	{
		// メニュー項目の定義
		items = new List<MenuItemData>()
		{
			new MenuItemData(
				"Start",
				() => LoadingManager.LoadScene("Stage1")
			),
			new MenuItemData(
				"Select",
				() => SceneManager.LoadSceneAsync("Select")
			),
			new MenuItemData(
				"Quit",
				() => GameEnd()
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
		// EventSystem から現在選択中の GameObject を取得
		var current = EventSystem.current.currentSelectedGameObject;
		if (current != _lastSelected)
		{
			_lastSelected = current;
			if (current != null)
			{
				// 名前と、panelParent 内でのインデックスを取得してログ
				int idx = current.transform.GetSiblingIndex();
				Debug.Log($"<color=cyan>Selected Button</color>: {current.name} (Index {idx})");
			}
			else
			{
				Debug.Log("<color=cyan>Selection cleared</color>");
			}
		}
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

	private void GameEnd()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}
}

