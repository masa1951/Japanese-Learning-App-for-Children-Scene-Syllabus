using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Coroutineを使うために必要

public class FlashcardButton : MonoBehaviour
{
    [Header("表示設定")]
    public Sprite defaultImage; // 初期表示のイラスト
    public string displayText;   // 切り替わるテキスト
    public AudioClip pronunciationAudio; // 発音する音声クリップ

    private Image buttonImage;
    private Text buttonText;
    private AudioSource audioSource;

    private bool isShowingText = false; // 現在テキストが表示されているか
    //==============================================================================
    void Awake()
    {
        // コンポーネントの取得
        buttonImage = GetComponent<Image>();
        // ボタンの子要素にテキストコンポーネントがあると仮定
        buttonText = GetComponentInChildren<Text>();
        audioSource = GetComponent<AudioSource>();

        // AudioSourceがアタッチされていない場合は追加
        if (audioSource == null)
        {            audioSource = gameObject.AddComponent<AudioSource>();        }

        // 初期状態を設定
        if (buttonImage != null && defaultImage != null)
        {            buttonImage.sprite = defaultImage;        }
        if (buttonText != null)
        {            buttonText.text = ""; // 初期はテキストを非表示
        }

        // ボタンクリックイベントの登録
        Button btn = GetComponent<Button>();
        if (btn != null)
        {            btn.onClick.AddListener(OnButtonClicked);        }
    }
    //==============================================================================
    void OnButtonClicked()
    {
        if (!isShowingText) // テキスト表示中でない場合のみ処理
        {            StartCoroutine(ShowTextAndPlayAudio());        }
    }
    //==============================================================================
    IEnumerator ShowTextAndPlayAudio()
    {
        isShowingText = true;

        // 1. 画像から文字に切り替え
        if (buttonImage != null)
        {            buttonImage.sprite = null; // 画像を非表示
        }
        if (buttonText != null)
        {            buttonText.text = displayText; // テキストを表示
        }

        // 2. 1秒待機
        yield return new WaitForSeconds(1.0f);

        // 3. 音声を再生
        if (audioSource != null && pronunciationAudio != null)
        {   audioSource.clip = pronunciationAudio;
            audioSource.Play();
        }

        // 4. 音声出力2秒後に元の状態に戻す
        // 音声が再生されなくても、ここで2秒待機
        float audioClipLength = (pronunciationAudio != null) ? pronunciationAudio.length : 0f;
        yield return new WaitForSeconds(Mathf.Max(2.0f, audioClipLength)); // 最低2秒、または音声の長さ分待つ

        // 元のイラストに戻す
        if (buttonImage != null && defaultImage != null)
        {            buttonImage.sprite = defaultImage;        }
        if (buttonText != null)
        {            buttonText.text = ""; // テキストを非表示
        }

        isShowingText = false;
    }
}