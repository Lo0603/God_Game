using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleToStage1 : MonoBehaviour
{
    public void OnStartGameButton()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopBGM();
        }

        SceneManager.LoadScene("Stage1");
    }
}
