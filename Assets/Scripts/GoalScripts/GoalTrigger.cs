using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
	private PlayerMoving playerScript;

	[Header("Settings")]
	public string playerTag = "Player";

	// Start is called before the first frame update
	void Start()
	{
		// player script íTçı
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			playerScript = player.GetComponent<PlayerMoving>();
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag(playerTag))
		{
			Debug.Log("Goal!! Stage Clear!");

			playerScript.StopMoving();
			FindObjectOfType<CameraZoomController>().ZoomIn(other.transform);
			// TODO: Ç†Ç∆Ç≈èàóùí«â¡
			// Example:
			// SceneManager.LoadScene("NextStage");
			// UIManager.Instance.ShowStageClear();
		}
	}
	// Update is called once per frame
	void Update()
	{

	}
}
