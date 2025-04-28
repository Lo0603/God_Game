using TMPro;
using UnityEngine;

public class MyButtonEvent : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI lavel;

	private int count = 0;

	void Start()
	{
		// ラベルの初期化
		lavel.text = $"{count}";
	}

	public void OnPressed()
	{
		// ボタンが押されたらカウンターを一つ増やし、ラベルを更新
		count++;
		lavel.text = $"{count}";
	}
}
