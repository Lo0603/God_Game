using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MenuBuilder : MonoBehaviour
{
	[SerializeField] private GameObject buttonPrefab; // MenuButtonプレハブ
	[SerializeField] public Transform panelParent;   // MenuPanelのTransform

	public void BuildMenu(List<MenuItemData> menuItems)
	{
		// 既存の子オブジェクトを全削除
		Clear();

		GameObject firstButton = null;

		// 新規ボタンを順に生成
		foreach (var item in menuItems)
		{
			var btnObj = Instantiate(buttonPrefab, panelParent);
			if (firstButton == null)
				firstButton = btnObj;

			// ラベル設定
			var tmp = btnObj.GetComponentInChildren<TMP_Text>();
			if (tmp != null) tmp.text = item.label;

			// クリックイベント登録
			var btn = btnObj.GetComponent<Button>();
			if (btn != null)
				btn.onClick.AddListener(() => item.onClick?.Invoke());
		}

		// 最初のボタンを選択状態に
		if (firstButton != null)
		{
			EventSystem.current.SetSelectedGameObject(null);
			EventSystem.current.SetSelectedGameObject(firstButton);
		}
	}

	public void Clear()
	{
#if UNITY_EDITOR
		while (panelParent.childCount > 0)
		{
			DestroyImmediate(panelParent.GetChild(0).gameObject);
		}
#else
        foreach (Transform child in panelParent)
        {
            Destroy(child.gameObject);
        }
#endif
	}
}
