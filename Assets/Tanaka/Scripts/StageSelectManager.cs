using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
	[Header("StageButton")]
	[SerializeField] private GameObject stageButtonPrefab;

	[Header("StagePage")]
	[SerializeField] private Transform[] pageParents;

	[Header("Unlock設定")]
	[SerializeField] private int unlockedStage = 1;

	private int totalStages = 10;
	private GameObject firstStageButton;


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

			// ロックアイコンを探す
			Transform lockTransform = btnObj.transform.Find("LockIcon");
			GameObject lockIcon = lockTransform != null ? lockTransform.gameObject : null;

			// ボタンとステージ番号の記録
			Button button = btnObj.GetComponent<Button>();
			int stageNum = i;

			if (button != null)
			{
				bool isUnlocked = (stageNum <= unlockedStage);

				button.interactable = isUnlocked;

				if (lockIcon != null)
					lockIcon.SetActive(!isUnlocked);

				// アンロックされたボタンにだけリスナー追加
				if (isUnlocked)
				{
					button.onClick.AddListener(() => Debug.Log($"Stage {stageNum} selected!"));
					button.onClick.AddListener(() => SceneManager.LoadScene($"Stage{stageNum}"));
				}
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
