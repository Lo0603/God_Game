using UnityEngine;
using System.Collections;

public class StagePageManager : MonoBehaviour
{
	[SerializeField] private RectTransform pageRoot;
	[SerializeField] private float slideDuration = 0.5f;
	private int currentPage = 0;
	private readonly float pageWidth = 1920f;

	public void NextPage()
	{
		if (currentPage >= 1) return;
		currentPage++;
		StartCoroutine(SlideToPage(currentPage));
	}

	public void PreviousPage()
	{
		if (currentPage <= 0) return;
		currentPage--;
		StartCoroutine(SlideToPage(currentPage));
	}

	private IEnumerator SlideToPage(int pageIndex)
	{
		Vector2 startPos = pageRoot.anchoredPosition;
		Vector2 endPos = new Vector2(-pageWidth * pageIndex, 0);
		float elapsed = 0f;

		while (elapsed < slideDuration)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / slideDuration);
			pageRoot.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));
			yield return null;
		}

		pageRoot.anchoredPosition = endPos;
	}
}