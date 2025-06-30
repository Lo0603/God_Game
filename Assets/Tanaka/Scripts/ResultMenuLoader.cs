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

	[Header("Key Bindings")]
	[SerializeField] private KeyCode upKey = KeyCode.W;
	[SerializeField] private KeyCode downKey = KeyCode.S;
	[SerializeField] private KeyCode confirmKey = KeyCode.Return;

	public static bool IsOpen { get; private set; }

	private List<MenuItemData> items;
	private Button[] buttons;
	private int selectedIndex = 0;
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

		// ボタン配列キャッシュ
		var parent = menuBuilder.panelParent;
		buttons = new Button[parent.childCount];
		for (int i = 0; i < parent.childCount; i++)
			buttons[i] = parent.GetChild(i).GetComponent<Button>();

		// 最初のボタンにフォーカス
		selectedIndex = 0;
		FocusCurrent();

		Debug.Log("show result");
	}

	private void Update()
	{
		if (!IsOpen) return;

		if (Input.GetKeyDown(upKey) || Input.GetKeyDown(KeyCode.UpArrow))
			ChangeSelection(-1);
		else if (Input.GetKeyDown(downKey) || Input.GetKeyDown(KeyCode.DownArrow))
			ChangeSelection(+1);

		if (Input.GetKeyDown(confirmKey))
			buttons[selectedIndex].onClick.Invoke();
	}

	private void ChangeSelection(int dir)
	{
		int prev = selectedIndex;
		selectedIndex = (selectedIndex + dir + buttons.Length) % buttons.Length;
		FocusCurrent();
		Debug.Log($"Result Selection: {buttons[selectedIndex].gameObject.name} (from {prev})");
	}

	private void FocusCurrent()
	{
		var btn = buttons[selectedIndex];
		// EventSystem にフォーカスを移して、OnSelect を発火
		EventSystem.current.SetSelectedGameObject(btn.gameObject);
		ExecuteEvents.Execute(
			btn.gameObject,
			new BaseEventData(EventSystem.current),
			ExecuteEvents.selectHandler
		);
	}

	private void ApplyButtonImages()
	{
		var parent = menuBuilder.panelParent;
		for (int i = 0; i < parent.childCount; i++)
		{
			var btn = parent.GetChild(i).GetComponent<Button>();
			switch (i)
			{
				case 0: if (nextSprite != null) btn.image.sprite = nextSprite; break;
				case 1: if (selectSprite != null) btn.image.sprite = selectSprite; break;
				case 2: if (titleSprite != null) btn.image.sprite = titleSprite; break;
			}
			// テキスト非表示
			var tmp = parent.GetChild(i).GetComponentInChildren<TMP_Text>();
			if (tmp != null) tmp.gameObject.SetActive(false);
		}
	}

	private void ApplyButtonScaleEffects()
	{
		var parent = menuBuilder.panelParent;
		for (int i = 0; i < parent.childCount; i++)
		{
			var btnObj = parent.GetChild(i).gameObject;
			// 拡大エフェクト
			if (btnObj.GetComponent<ButtonScaleOnSelect>() == null)
				btnObj.AddComponent<ButtonScaleOnSelect>();
			// 自動ナビゲーション
			var btn = btnObj.GetComponent<Button>();
			var nav = btn.navigation;
			nav.mode = Navigation.Mode.Automatic;
			btn.navigation = nav;
		}
	}
}
