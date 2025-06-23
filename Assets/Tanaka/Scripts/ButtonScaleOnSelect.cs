using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class ButtonScaleOnSelect : MonoBehaviour,
	ISelectHandler,    // フォーカスされたとき
	IDeselectHandler   // フォーカスが外れたとき
{
	[Tooltip("通常時の大きさ")]
	public Vector3 normalScale = Vector3.one;
	[Tooltip("選択時の大きさ")]
	public Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1f);

	RectTransform _rect;

	void Awake()
	{
		_rect = GetComponent<RectTransform>();
		// Inspector に値を入れていなければ現在の localScale を normalScale に
		if (_rect.localScale != normalScale)
			normalScale = _rect.localScale;
	}

	// 矢印キー / ゲームパッド操作でこのボタンにフォーカスが来たとき
	public void OnSelect(BaseEventData eventData)
	{
		_rect.localScale = selectedScale;
	}

	// フォーカスが別の UI 要素に移ったとき
	public void OnDeselect(BaseEventData eventData)
	{
		_rect.localScale = normalScale;
	}
}
