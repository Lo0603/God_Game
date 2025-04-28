using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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

	public void SceneTransition(int sceneNum)
	{
		// フェードを掛けてからシーン遷移
		fade.FadeIn(fadeTime, () =>
		{
			SceneManager.LoadScene("Fade" + sceneNum);
		});

	}
}
