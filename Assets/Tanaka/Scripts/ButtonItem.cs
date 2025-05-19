using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonItem : MonoBehaviour
{
	[SerializeField] private TMP_Text label;
	[SerializeField] private Button button;

	public void Setup(string text, UnityAction onClick)
	{
		label.text = text;
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(onClick);
	}
}
