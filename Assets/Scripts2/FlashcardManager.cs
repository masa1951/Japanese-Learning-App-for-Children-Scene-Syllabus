using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class FlashcardManager : MonoBehaviour
{
    [Header("フラッシュカードデータ")]
    public List<FlashcardData> flashcardDatas = new List<FlashcardData>();

 //   [Header("UID設定")]
//    public Color textDisplayColor = Color.black;

    private enum ButtonState { Illustration, AudioOnly, TextDisplay }
    private ButtonState[] buttonStates;

    private Button[] flashcardButtons;
    private Image[] buttonImages;
    private TextMeshProUGUI[] buttonTexts;
    private AudioSource audioSource;

    public Color fadeColor = new Color(1.0f, 1.0f, 1.0f, 0.25f); // 50%透明で暗い色
//    public Color highlightColor = Color.white; // ハイライト用の色
    private Color[] originalButtonColors; // 元のボタン色を保持する配列

    //===========================================================================================
//    [Header("UI設定")]
//    public Color textDisplayColor = Color.black;

  //  private enum ButtonState { Illustration, AudioOnly, TextDisplay }
  //  private ButtonState[] buttonStates;

  //  private Button[] flashcardButtons;
  //  private Image[] buttonImages;
  //  private TextMeshProUGUI[] buttonTexts;
  //  private AudioSource audioSource;


    //===========================================================================================
    // このメソッドはPanelSwitcherから呼び出される
    public void SetupPanel()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        flashcardButtons = GetComponentsInChildren<Button>();
        buttonImages = new Image[flashcardButtons.Length];
        buttonTexts = new TextMeshProUGUI[flashcardButtons.Length];
        buttonStates = new ButtonState[flashcardButtons.Length];
        originalButtonColors = new Color[flashcardButtons.Length]; // ★追加

        for (int i = 0; i < flashcardButtons.Length; i++)
        {
            buttonImages[i] = flashcardButtons[i].GetComponent<Image>();
            buttonTexts[i] = flashcardButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            // ★追加：元の色を保存
            if (buttonImages[i] != null)
            {
                originalButtonColors[i] = buttonImages[i].color;
            }

            flashcardButtons[i].onClick.RemoveAllListeners(); // 既存のリスナーをクリア
            int index = i;
            flashcardButtons[i].onClick.AddListener(() => OnFlashcardButtonClicked(index));
        }

        InitializeButtons();
    }

    //===========================================================================================
    void InitializeButtons()
    {
        //        Debug.Log($"[FlashcardManager] InitializeButtons called for {gameObject.name}");
        for (int i = 0; i < flashcardButtons.Length; i++)
        {
            if (i < flashcardDatas.Count)
            {
                if (buttonImages[i] != null && flashcardDatas[i].illustrationSprite != null)
                {
                    buttonImages[i].sprite = flashcardDatas[i].illustrationSprite;
                }
                if (buttonTexts[i] != null)
                {
                    buttonTexts[i].text = "";
//                    buttonTexts[i].color = new Color(buttonTexts[i].color.r, buttonTexts[i].color.g, buttonTexts[i].color.b, 0f);
                }
                buttonStates[i] = ButtonState.Illustration;
                flashcardButtons[i].interactable = true;
            }
            else
            {
                if (flashcardButtons[i] != null)
                {
                    flashcardButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }
    //===========================================================================================
    //===========================================================================================
    void OnFlashcardButtonClicked(int index)
    {
        switch (buttonStates[index])
        {
            case ButtonState.Illustration:
                // 1クリック目：音声のみを再生し、状態をAudioOnlyに変更
                if (audioSource != null && flashcardDatas[index].pronunciationAudio != null)
                {
                    audioSource.clip = flashcardDatas[index].pronunciationAudio;
                    audioSource.Play();
                }
                buttonStates[index] = ButtonState.AudioOnly;

                // 他のボタンの操作を無効にする
                SetAllButtonsInteractable(false, index);    // ★関数を新しいものに置き換える
                break;

            case ButtonState.AudioOnly:
                // 2クリック目：文字を表示し、状態をTextDisplayに変更
                // 画像を非表示にし、文字を表示する
                if (buttonImages[index] != null)
                {
                    buttonImages[index].sprite = null;
                    buttonImages[index].color = originalButtonColors[index]; // ★元の色に戻す
                }
                if (buttonTexts[index] != null)
                {
                    buttonTexts[index].text = flashcardDatas[index].displayText;
//                    buttonTexts[index].color = new Color(textDisplayColor.r, textDisplayColor.g, textDisplayColor.b, 1.0f);
                }
                buttonStates[index] = ButtonState.TextDisplay;
                break;

            case ButtonState.TextDisplay:
                // 3クリック目：元のイラストに戻し、状態をIllustrationに変更
                // 画像を表示し、文字を非表示にする
                if (buttonImages[index] != null && flashcardDatas[index].illustrationSprite != null)
                {
                    buttonImages[index].sprite = flashcardDatas[index].illustrationSprite;
                }
                if (buttonTexts[index] != null)
                {
                    buttonTexts[index].text = "";
//                    buttonTexts[index].color = new Color(buttonTexts[index].color.r, buttonTexts[index].color.g, buttonTexts[index].color.b, 0f);
                }
                buttonStates[index] = ButtonState.Illustration;

                // すべてのボタンの操作と色を元に戻す
                ResetAllButtonsInteractableAndColor(); // ★新しい関数を呼び出す
                break;
        }
    }
    //===========================================================================================
    // クリックされたボタンをハイライトし、他のボタンを暗くする
    void SetAllButtonsInteractableAndColor(bool interactable, int? excludeIndex = null)
    {
        for (int i = 0; i < flashcardButtons.Length; i++)
        {
            if (flashcardButtons[i] != null)
            {
                if (excludeIndex.HasValue && i == excludeIndex.Value)
                {
                    // クリックされたボタン：有効なままハイライト
                    flashcardButtons[i].interactable = true;
/*                    if (buttonImages[i] != null)
                    {
                        buttonImages[i].color = highlightColor; // ★ハイライト色に
                    }
*/                }
                else
                {
                    // 他のボタン：無効化してフェードアウト
                    flashcardButtons[i].interactable = false;
                    if (buttonImages[i] != null)
                    {
                        buttonImages[i].color = fadeColor; // ★暗い色に
                    }
                }
            }
        }
    }
    //===========================================================================================
    // すべてのボタンの状態と色を元に戻す
    void ResetAllButtonsInteractableAndColor()
    {
        for (int i = 0; i < flashcardButtons.Length; i++)
        {
            if (flashcardButtons[i] != null)
            {
                flashcardButtons[i].interactable = true;
                if (buttonImages[i] != null)
                {
                    buttonImages[i].color = originalButtonColors[i]; // ★元の色に戻す
                }
            }
        }
    }

    //===========================================================================================
    // 以下は変更なし
    void SetAllButtonsInteractable(bool interactable, int? excludeIndex = null)
    {
        for (int i = 0; i < flashcardButtons.Length; i++)
        {
            if (flashcardButtons[i] != null)
            {
                if (excludeIndex.HasValue && i == excludeIndex.Value)
                {
                    flashcardButtons[i].interactable = true;
                }
                else
                {
                    flashcardButtons[i].interactable = interactable;
                }
            }
        }
    }
    //===========================================================================================
    // PanelSwitcherから現在のパネルのボタンをリセットするために公開
    public void ResetAllButtons()
    {
        //        Debug.Log($"[FlashcardManager] ResetAllButtons called for {gameObject.name}. Stopping coroutines and audio.");
        StopAllCoroutines(); // このFlashcardManagerに紐づく全てのコルーチンを停止
        if (audioSource != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop(); // 再生中の音声を停止
            }
            audioSource.clip = null; // ★ここを追加：オーディオクリップを明示的にクリア
                                     //            Debug.Log($"[FlashcardManager] Audio clip cleared for {gameObject.name}.");
        }
        InitializeButtons(); // ボタンを初期状態に戻す
        ResetAllButtonsInteractableAndColor(); // ★追加
    }
}