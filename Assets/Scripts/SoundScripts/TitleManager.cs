using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    void Start()
    {
        if (SoundManager.Instance != null)
        {
            // すでに再生中かチェックしてから再生
            if (!SoundManager.Instance.bgmSource.isPlaying ||
                SoundManager.Instance.bgmSource.clip.name != "mainBGM")
            {
                SoundManager.Instance.PlayBGM("mainBGM");
            }
        }
        else
        {
            Debug.LogWarning("SoundManager が存在しません！");
        }
    }
}
