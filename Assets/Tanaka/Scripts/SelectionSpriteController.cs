using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// ボタンに追加して、選択・非選択・押下で画像を切り替えるコンポーネント
public class SelectionSpriteController : MonoBehaviour,
	ISelectHandler, IDeselectHandler,
	IPointerDownHandler, IPointerUpHandler
{
	Image _image;
	Sprite _normal, _highlighted, _pressed, _disabled;

	// 初期化用メソッド
	public void Init(Image image,
					 Sprite normal, Sprite highlighted,
					 Sprite pressed, Sprite disabled)
	{
		_image = image;
		_normal = normal;
		_highlighted = highlighted;
		_pressed = pressed;
		_disabled = disabled;
		// 最初は通常
		_image.sprite = _normal;
	}

	// キーやゲームパッドでフォーカスが来たとき
	public void OnSelect(BaseEventData eventData)
	{
		if (_highlighted != null)
			_image.sprite = _highlighted;
	}

	// フォーカスが外れたとき
	public void OnDeselect(BaseEventData eventData)
	{
		if (_normal != null)
			_image.sprite = _normal;
	}

	// マウスやボタンで押下したとき
	public void OnPointerDown(PointerEventData eventData)
	{
		if (_pressed != null)
			_image.sprite = _pressed;
	}

	// 押下を離したとき
	public void OnPointerUp(PointerEventData eventData)
	{
		// フォーカスが残っていればハイライト、なければ通常
		bool isSelected = EventSystem.current.currentSelectedGameObject == gameObject;
		_image.sprite = isSelected ? _highlighted : _normal;
	}
}
