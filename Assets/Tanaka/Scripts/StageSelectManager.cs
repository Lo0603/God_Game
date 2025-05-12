using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class StageSelectManager : MonoBehaviour
{
	[Header("StageButton")]
	[SerializeField] private GameObject stageButtonPrefab; // ステージボタンのプレハブ

	[Header("StagePage")]
	[SerializeField] private Transform[] pageParents;      // 各ページの親Transform


	private int totalStages = 10;         // ステージ数

	private GameObject firstStageButton; // ステージ1のボタンを覚えておく

	void Start()
	{
		CreateStageButtons();
		SelectFirstStageButton();
	}

	void CreateStageButtons()
	{
		for (int i = 1; i <= totalStages; i++)
		{
			int pageIndex = (i - 1) / 9;
			Transform parent = pageParents[pageIndex];

			GameObject btnObj = Instantiate(stageButtonPrefab, parent);

			// ボタンのテキスト設定
			TextMeshProUGUI text = btnObj.GetComponentInChildren<TextMeshProUGUI>();
			if (text != null)
			{
				text.text = $"Stage {i}";
			}

			// 全ステージをアンロック
			Button button = btnObj.GetComponent<Button>();
			if (button != null)
			{
				button.interactable = true;

				// ボタンクリックでステージ番号ログを出す
				int stageNum = i;
				button.onClick.AddListener(() => Debug.Log($"Stage {stageNum} selected!"));
			}

			// ステージ1のボタンを保存
			if (i == 1)
			{
				firstStageButton = btnObj;
			}
		}
	}

	void SelectFirstStageButton()
	{
		if (firstStageButton != null)
		{
			EventSystem.current.SetSelectedGameObject(firstStageButton);
		}
	}
}
