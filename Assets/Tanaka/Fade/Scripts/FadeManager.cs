using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
	// FadeCanvasを取得
	[SerializeField]
	private Fade fade;

	// フェード時間(秒)
	[SerializeField]
	private float fadeTime;

	void Start()
	{
		// シーン開始時にフェードを掛ける
		fade.FadeOut(fadeTime);
	}

	public void SceneTransition(string sceneNum)
	{
		// フェードを掛けてからシーン遷移
		fade.FadeIn(fadeTime, () =>
		{
			SceneManager.LoadScene(sceneNum);
		});

	}
}
