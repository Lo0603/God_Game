using System.Collections;
using UnityEngine;

public class PageSlider : MonoBehaviour
{
	[Header("ページ登録")]
	[SerializeField] private RectTransform[] pages;

	[Header("スライド時間")]
	[SerializeField] private float slideDuration = 0.5f;

	private int currentPage = 0;
	public bool isSliding = false;
	public bool IsSliding => isSliding;

	public int CurrentPage => currentPage;
	public RectTransform[] Pages => pages;


	public void SlideToPage(int targetPage)
	{
		if (isSliding || targetPage == currentPage || targetPage < 0 || targetPage >= pages.Length)
			return;

		StartCoroutine(SlideCoroutine(currentPage, targetPage));
		currentPage = targetPage;
	}

	private IEnumerator SlideCoroutine(int from, int to)
	{
		isSliding = true;

		float width = Screen.width;
		Vector2 startFrom = pages[from].anchoredPosition;
		Vector2 endFrom = new Vector2((from < to ? -width : width), startFrom.y);
		Vector2 startTo = new Vector2((from < to ? width : -width), startFrom.y);

		pages[to].anchoredPosition = startTo;

		float elapsed = 0f;
		while (elapsed < slideDuration)
		{
			float t = elapsed / slideDuration;
			pages[from].anchoredPosition = Vector2.Lerp(startFrom, endFrom, t);
			pages[to].anchoredPosition = Vector2.Lerp(startTo, Vector2.zero, t);
			elapsed += Time.deltaTime;
			yield return null;
		}

		pages[from].anchoredPosition = endFrom;
		pages[to].anchoredPosition = Vector2.zero;

		isSliding = false;
	}
}
