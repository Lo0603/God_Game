using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonItem : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI label;
	[SerializeField] private Button button;

	public void Setup(string text, UnityAction onClick)
	{
		if (label != null)
			label.text = text;

		if (button != null)
		{
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(onClick);
		}
	}
}
