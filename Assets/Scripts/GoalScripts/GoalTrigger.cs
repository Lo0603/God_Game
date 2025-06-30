using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
	private PlayerMoving playerScript;
    private Animator anim;

    [Header("Settings")]
	public string playerTag = "Player";

    void Awake()
    {
        anim = GetComponent<Animator>();
    }
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

            SoundManager.Instance.PlaySE("ClearSound");

            anim.SetBool("IsOpen",true);

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
