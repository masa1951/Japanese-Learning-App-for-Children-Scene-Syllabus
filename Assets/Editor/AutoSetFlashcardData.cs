using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq; // LINQを使用するために追加

// Unityエディタ拡張のためEditorWindowを継承
public class AutoSetFlashcardData : EditorWindow
{
    // CSVファイル名を手動で入力する変数に変更
    private string csvFileName = "SyllabusText5.csv";

    // Unityメニューバーに項目を追加
    [MenuItem("Tools/Flashcard/Auto Set Data")]
    public static void ShowWindow()
    {
        GetWindow<AutoSetFlashcardData>("Auto Set Flashcard Data");
    }

    // ウィンドウのGUIを描画
    void OnGUI()
    {
        GUILayout.Label("Flashcard Dataを一括設定", EditorStyles.boldLabel);

        // 修正箇所：CSVファイル名を手動入力
        csvFileName = EditorGUILayout.TextField("CSV File Name (in Assets/Editor)", csvFileName);

        // 実行ボタン
        if (GUILayout.Button("設定を開始"))
        {
            SetFlashcardData();
        }
    }

    private void SetFlashcardData()
    {
        if (string.IsNullOrEmpty(csvFileName))
        {
            Debug.LogError("CSVファイル名が入力されていません。");
            return;
        }

        // CSVの読み込みと解析
        string fullPath = Application.dataPath + "/Editor/" + csvFileName;
        if (!File.Exists(fullPath))
        {
            Debug.LogError("CSVファイルが見つかりません: " + fullPath);
            return;
        }

        var lines = File.ReadAllLines(fullPath);
        var flashcardTexts = new Dictionary<string, List<string>>();

        // CSVのヘッダー行（タイトル）を読み込む
        var titles = lines[0].Split(',').Select(t => t.Trim()).ToArray();

        // CSVの各行（2行目以降）を読み込む
        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',').Select(v => v.Trim()).ToArray();
            for (int j = 0; j < titles.Length; j++)
            {
                if (!flashcardTexts.ContainsKey(titles[j]))
                {
                    flashcardTexts[titles[j]] = new List<string>();
                }
                if (j < values.Length)
                {
                    flashcardTexts[titles[j]].Add(values[j]);
                }
            }
        }

        Debug.Log("CSVデータの読み込みが完了しました。");

        // FlashcardManagerを持つGameObjectを検索し、データを設定
        for (int i = 1; i <= 14; i++)
        {
            string panelName = "Panel_" + i.ToString("D2"); // Panel_01, Panel_02...
            string titleKey = "Syllabus" + i.ToString(); // syllabus1, Syllabus2......Syllabus14

            GameObject panelObject = GameObject.Find(panelName);

            if (panelObject == null)
            {
                Debug.LogWarning($"Hierarchyに '{panelName}' が見つかりませんでした。スキップします。");
                continue;
            }

            FlashcardManager flashcardManager = panelObject.GetComponent<FlashcardManager>();

            if (flashcardManager == null)
            {
                Debug.LogWarning($"'{panelObject.name}' に FlashcardManager コンポーネントが見つかりませんでした。スキップします。");
                continue;
            }

            // CSVから該当する文字データを取得
            if (!flashcardTexts.ContainsKey(titleKey))
            {
                Debug.LogError($"CSVのヘッダーに '{titleKey}' が見つかりませんでした。");
                continue;
            }
            List<string> texts = flashcardTexts[titleKey];

            // FlashcardDataの配列を初期化
            //            flashcardManager.flashcardDatas = new FlashcardData[25];
            flashcardManager.flashcardDatas = new List<FlashcardData>();

            for (int j = 0; j < 25; j++)
            {
                // Resources.Loadを使ってアセットを読み込み
                string illustPath = $"illusts/Scene{i}/{j + 1}";
                string soundPath = $"Sounds/Sound{i}/{j + 1}";

                Sprite illustSprite = Resources.Load<Sprite>(illustPath);
                AudioClip pronunciationAudio = Resources.Load<AudioClip>(soundPath);
                string displayText = (j < texts.Count) ? texts[j] : string.Empty;

                if (illustSprite == null)
                {
                    Debug.LogWarning($"イラストが見つかりません: {illustPath}");
                }
                if (pronunciationAudio == null)
                {
                    Debug.LogWarning($"音声が見つかりません: {soundPath}");
                }

                /*               flashcardManager.flashcardDatas[j] = new FlashcardData
                               {
                                   illustrationSprite = illustSprite,
                                   displayText = displayText,
                                   pronunciationAudio = pronunciationAudio
                               };
                */
                flashcardManager.flashcardDatas.Add(new FlashcardData
                {
                    illustrationSprite = illustSprite,
                    displayText = displayText,
                    pronunciationAudio = pronunciationAudio
                });
            }

            // 変更を保存
            EditorUtility.SetDirty(flashcardManager);
            Debug.Log($"'{panelName}' の FlashcardManager にデータを設定しました。");
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Flashcardデータの自動設定が完了しました！");
    }
}
