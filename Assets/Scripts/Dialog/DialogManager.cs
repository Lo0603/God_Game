using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [Header("UI Componets")]
    public GameObject dialogPanel;
    public Text dialogText;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;     // 文字出力速度

    private string[] sentences;           // 対話文章
    private int currentSentenceIndex = 0;
    private bool isTyping = false;

    // Start is called before the first frame update
    void Start()
    {
        //dialogPanel.SetActive(false);  // start時隠す
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogPanel.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            if (!isTyping)
            {
                NextSentence();
            }
            else
            {
                // 文章出力中Enter押したら全て出力
                StopAllCoroutines();
                dialogText.text = sentences[currentSentenceIndex];
                isTyping = false;
            }
        }
    }


    // 説明ウィンドウ開始呼び出し
    public void StartDialog(string[] dialogSentences)
    {
        sentences = dialogSentences;
        currentSentenceIndex = 0;
        dialogPanel.SetActive(true);
        //Time.timeScale = 0f;  // ゲームstop

        ShowSentence();
    }

    void ShowSentence()
    {
        dialogText.text = "";
        StartCoroutine(TypeSentence(sentences[currentSentenceIndex]));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;

        foreach (char letter in sentence)
        {
            dialogText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed); // Time.timeScale = 0でも作動
        }

        isTyping = false;
    }

    void NextSentence()
    {
        currentSentenceIndex++;

        if (currentSentenceIndex < sentences.Length)
        {
            ShowSentence();
        }
        else
        {
            EndDialog();
        }
    }

    void EndDialog()
    {
        dialogPanel.SetActive(false);
        Time.timeScale = 1f; // ゲーム再開
    }
}
