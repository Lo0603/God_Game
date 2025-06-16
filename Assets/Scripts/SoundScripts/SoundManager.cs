// SoundManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioSource bgmSource; // BGM用AudioSource
    public AudioSource seSource;  // SE用AudioSource
    Dictionary<string, AudioSource> loopSESources = new Dictionary<string, AudioSource>();

    public AudioClip[] bgmClips; // BGM用の音ファイル一覧
    public AudioClip[] seClips;  // SE用の音ファイル一覧

    void Awake()
    {
        // シングルトンパターン（他スクリプトからアクセスするため）
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも削除されない
        }
        else
        {
            Destroy(gameObject); // 重複インスタンスは破棄
        }
    }

    // BGMを再生
    public void PlayBGM(string name)
    {
        AudioClip clip = GetClipByName(bgmClips, name);
        if (clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    // BGMを停止
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // SEを再生
    public void PlaySE(string name)
    {
        AudioClip clip = GetClipByName(seClips, name);
        if (clip != null)
        {
            seSource.PlayOneShot(clip);
        }
    }

    public void PlayLoopSE(string name)
    {
        if (loopSESources.ContainsKey(name))
        {
            // もう再生中なら停止
            return;
        }

        AudioClip clip = GetClipByName(seClips, name);
        if (clip != null)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.Play();
            loopSESources[name] = source;
        }
    }

    public void StopLoopSE(string name)
    {
        if (loopSESources.TryGetValue(name, out AudioSource source))
        {
            source.Stop();
            Destroy(source);
            loopSESources.Remove(name);
        }
    }

    // 名前でClipを探す
    private AudioClip GetClipByName(AudioClip[] clips, string name)
    {
        foreach (AudioClip clip in clips)
        {
            if (clip.name == name)
                return clip;
        }
        Debug.LogWarning("サウンドが見つかりません: " + name);
        return null;
    }

    //public class TitleManager : MonoBehaviour
    //{
    //    void Start()
    //    {
    //        if (SoundManager.Instance != null)
    //        {
    //            SoundManager.Instance.PlayBGM("ゆるやか１");
    //        }
    //        else
    //        {
    //            Debug.LogWarning("SoundManager が存在しません！");
    //        }
    //    }
    //}
    //void Start()
    //{
    //    if (SoundManager.Instance != null)
    //    {
    //        if (!SoundManager.Instance.bgmSource.isPlaying ||
    //            SoundManager.Instance.bgmSource.clip.name != "ゆるやか１")
    //        {
    //            SoundManager.Instance.PlayBGM("ゆるやか１");
    //        }
    //    }
    //}

}

