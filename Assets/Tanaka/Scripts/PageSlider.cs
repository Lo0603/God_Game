using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageSlider : MonoBehaviour
{
	[SerializeField] private GameObject[] pageArrowButtons;

	public RectTransform[] pages; // ページ群
	public float slideDuration = 0.5f;
	private int currentPage = 0;
	private bool isSliding = false;

	public void SlideToPage(int targetPage)
	{
		if (isSliding || targetPage == currentPage || targetPage < 0 || targetPage >= pages.Length)
			return;

		StartCoroutine(SlideCoroutine(currentPage, targetPage));
		currentPage = targetPage;
	}

	IEnumerator SlideCoroutine(int fromPage, int toPage)
	{
		isSliding = true;

		Vector2 fromStart = pages[fromPage].anchoredPosition;
		float width = Screen.width;

		// スライド方向を決定（左へ or 右へ）
		int direction = (toPage > fromPage) ? 1 : -1;

		// from → to に向けての座標を設定
		Vector2 fromEnd = new Vector2(-direction * width, fromStart.y);
		Vector2 toStart = new Vector2(direction * width, fromStart.y);

		pages[toPage].anchoredPosition = toStart;

		float time = 0f;
		while (time < slideDuration)
		{
			float t = time / slideDuration;
			pages[fromPage].anchoredPosition = Vector2.Lerp(fromStart, fromEnd, t);
			pages[toPage].anchoredPosition = Vector2.Lerp(toStart, Vector2.zero, t);
			time += Time.deltaTime;
			yield return null;
		}

		// 最終位置を明示的に設定
		pages[fromPage].anchoredPosition = fromEnd;
		pages[toPage].anchoredPosition = Vector2.zero;


		// ページ遷移が終わったあとに次ページ内の矢印にカーソル移動
		if (pageArrowButtons.Length > toPage && pageArrowButtons[toPage] != null)
		{
			EventSystem.current.SetSelectedGameObject(pageArrowButtons[toPage]);
		}


		isSliding = false;
	}
}
