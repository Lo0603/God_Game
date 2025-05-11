using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class StageButton : MonoBehaviour
{
	[Header("UI 参照")]
	[SerializeField] private Button button;
	[SerializeField] private TextMeshProUGUI stageText;
	[SerializeField] private GameObject lockImage;

	private int stageNumber;

	/// <summary>
	/// ステージボタンの初期化
	/// </summary>
	public void Initialize(int number, bool isUnlocked, UnityAction<int> onClickCallback)
	{
		stageNumber = number;

		// テキストをステージ番号に設定
		if (stageText != null)
		{
			stageText.text = number.ToString();
		}

		// ロック画像の表示・非表示
		if (lockImage != null)
		{
			lockImage.SetActive(!isUnlocked);
		}

		// ボタンの有効・無効切り替え
		if (button != null)
		{
			button.interactable = isUnlocked;

			// 既存リスナーをクリアしてから登録（多重登録防止）
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() => onClickCallback?.Invoke(stageNumber));
		}
	}
}
