using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PageSlider : MonoBehaviour
{
	[Header("ページ設定")]
	[SerializeField] private RectTransform[] pages;
	[SerializeField] private float slideDuration = 0.5f;

	[Header("矢印ボタンの設定")]
	[SerializeField] private GameObject arrowButtonPrefab;
	[SerializeField] private Sprite leftArrowSprite;
	[SerializeField] private Sprite rightArrowSprite;

	private int currentPage = 0;
	public int CurrentPage => currentPage;

	private bool isSliding = false;

	private GameObject[] allLeftArrows;
	private GameObject[] allRightArrows;

	void Start()
	{
		InitializeArrows();
		UpdateArrowVisibility();
		EventSystem.current.SetSelectedGameObject(allRightArrows[0]);

	}

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

		// --- 矢印を全非表示 ---
		HideAllArrows();

		Vector2 fromStart = pages[fromPage].anchoredPosition;
		float width = Screen.width;

		int direction = (toPage > fromPage) ? 1 : -1;
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

		pages[fromPage].anchoredPosition = fromEnd;
		pages[toPage].anchoredPosition = Vector2.zero;

		// --- ページ切り替え後に矢印を更新 ---
		UpdateArrowVisibility();

		GameObject nextArrow = allRightArrows[toPage] ?? allLeftArrows[toPage];
		if (nextArrow != null)
			EventSystem.current.SetSelectedGameObject(nextArrow);

		isSliding = false;
	}


	void InitializeArrows()
	{
		allLeftArrows = new GameObject[pages.Length];
		allRightArrows = new GameObject[pages.Length];

		for (int i = 0; i < pages.Length; i++)
		{
			AddArrowsToPage(i);
		}
	}

	void AddArrowsToPage(int pageIndex)
	{
		RectTransform page = pages[pageIndex];
		float xOffset = 800f;
		float y = -50f;

		if (pageIndex > 0)
		{
			GameObject left = Instantiate(arrowButtonPrefab, page);
			left.name = $"LeftArrow_Page{pageIndex}";
			left.GetComponent<RectTransform>().anchoredPosition = new Vector2(-xOffset, y);
			left.GetComponent<Image>().sprite = leftArrowSprite;
			left.GetComponent<Button>().onClick.AddListener(() => SlideToPage(pageIndex - 1));
			allLeftArrows[pageIndex] = left;
		}

		if (pageIndex < pages.Length - 1)
		{
			GameObject right = Instantiate(arrowButtonPrefab, page);
			right.name = $"RightArrow_Page{pageIndex}";
			right.GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset + 100f, y);
			right.GetComponent<Image>().sprite = rightArrowSprite;
			right.GetComponent<Button>().onClick.AddListener(() => SlideToPage(pageIndex + 1));
			allRightArrows[pageIndex] = right;
		}
	}

	void UpdateArrowVisibility()
	{
		for (int i = 0; i < pages.Length; i++)
		{
			if (allLeftArrows[i] != null)
				allLeftArrows[i].SetActive(i == currentPage && currentPage > 0);

			if (allRightArrows[i] != null)
				allRightArrows[i].SetActive(i == currentPage && currentPage < pages.Length - 1);
		}
	}

	void HideAllArrows()
	{
		foreach (var arrow in allLeftArrows)
		{
			if (arrow != null) arrow.SetActive(false);
		}

		foreach (var arrow in allRightArrows)
		{
			if (arrow != null) arrow.SetActive(false);
		}
	}

}
