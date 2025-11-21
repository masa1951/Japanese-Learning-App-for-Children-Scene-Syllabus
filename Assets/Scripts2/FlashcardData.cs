using UnityEngine;

[System.Serializable] // Unityエディタで表示・編集可能にする
public class FlashcardData
{
    public Sprite illustrationSprite; // イラスト画像
    public string displayText;        // 表示する文字
    public AudioClip pronunciationAudio; // 発音する音声
}