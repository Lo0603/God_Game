using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
	private TMP_Text labelText;
	[SerializeField] private GameObject clearIcon; // クリア済みアイコンなどあれば

	void Awake()
	{
		// 子オブジェクトから TMP_Text を探して保持
		labelText = GetComponentInChildren<TMP_Text>();
		if (labelText == null)
		{
			Debug.LogWarning($"{gameObject.name} に TMP_Text が見つかりませんでした。");
		}
	}

	public void SetData(string label, bool isCleared)
	{
		if (labelText != null)
		{
			labelText.text = label;
		}

		if (clearIcon != null)
		{
			clearIcon.SetActive(isCleared);
		}
	}
}
