
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelSwitcher : MonoBehaviour
{
    [Header("メインパネル群")]
    public GameObject StartPanel; // StartPanel GameObject
    public GameObject ScenePanels; // ScenePanels GameObject (Panel_1-10の親)

    [Header("ScenePanels内の各パネル")]
    public GameObject[] sceneSubPanels; // Panel_1からPanel_10までのGameObjectをここに割り当てる

    [Header("StartPanelの切り替えボタン")]
    public Button[] startPanelButtons; // StartPanel内のButton_1からButton_10をここに割り当てる

    [Header("ナビゲーションボタン")]
    public Button BackButton;
    public Button BackInitButton; // 初期画面へ

    private FlashcardManager[] flashcardManagers; // 各sceneSubPanelsのFlashcardManagerを保持
    private int currentActiveScenePanelIndex = -1; // 現在表示中のScenePanels内のサブパネルのインデックス
//||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
    public void GoTo_Dsp_Conjugation()
    {
        SceneManager.LoadScene("Dsp_Conjugation");
    }

    //==========================================================================================
    void Awake()
    {
        // 各sceneSubPanelsのFlashcardManagerコンポーネントを取得
        flashcardManagers = new FlashcardManager[sceneSubPanels.Length];
        for (int i = 0; i < sceneSubPanels.Length; i++)
        {
            flashcardManagers[i] = sceneSubPanels[i].GetComponent<FlashcardManager>();
            if (flashcardManagers[i] == null)
            {
                Debug.LogError($"Scene Sub Panel {i + 1} に FlashcardManager が見つかりません！");
            }
        }

        // StartPanelの切り替えボタンにイベントを登録
        for (int i = 0; i < startPanelButtons.Length; i++)
        {
            int panelIndex = i; // クロージャ問題回避のため
            startPanelButtons[i].onClick.AddListener(() => OnStartPanelButtonClicked(panelIndex));
        }

        // BackButtonにイベントを登録
        if (BackButton != null)
        {
            BackButton.onClick.AddListener(OnBackButtonClicked);
        }
        // BackInitButtonはダミーだったが、Scene Dsp_Conjugation の初期画面へのボタンにする
        if (BackInitButton != null)
        {
            BackInitButton.onClick.AddListener(OnBackInitButtonClicked);
        }
    }
    //==========================================================================================
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "SelectScene")
            return;

        // ゲーム開始時にStartPanelを表示し、ScenePanelsを非表示にする
        ShowStartPanel();
    }
    //==========================================================================================
    // StartPanelを表示し、ScenePanelsを非表示にする
    public void ShowStartPanel()
    {
        StartPanel.SetActive(true);
        ScenePanels.SetActive(false);

        // 現在アクティブなサブパネルがあれば非表示にする
        if (currentActiveScenePanelIndex != -1)
        {
            sceneSubPanels[currentActiveScenePanelIndex].SetActive(false);
            // 以前のパネルのボタン状態をリセット
            if (flashcardManagers[currentActiveScenePanelIndex] != null)
            {
                flashcardManagers[currentActiveScenePanelIndex].ResetAllButtons();
            }
            currentActiveScenePanelIndex = -1; // インデックスをリセット
        }

        // BackButtonとGotoButtonの表示状態を調整（StartPanelでは非表示にするなど）
        if (BackButton != null) BackButton.gameObject.SetActive(false);
        if (BackInitButton != null) BackInitButton.gameObject.SetActive(true);
    }
    //==========================================================================================
    // 指定されたScenePanels内のサブパネルを表示し、StartPanelを非表示にする
    public void ShowScenePanel(int panelIndex)
    {
//        print("panel Index = " + panelIndex);
        if (panelIndex < 0 || panelIndex >= sceneSubPanels.Length)
        {
            Debug.LogWarning($"無効なパネルインデックス: {panelIndex}");
            return;
        }

        // StartPanelを非表示にする
        StartPanel.SetActive(false);
        // ScenePanelsを有効にする（その中の特定のサブパネルを有効にするため）
        ScenePanels.SetActive(true);

        // 現在アクティブなサブパネルがあれば非表示にする
        if (currentActiveScenePanelIndex != -1)
        {
            sceneSubPanels[currentActiveScenePanelIndex].SetActive(false);
            if (flashcardManagers[currentActiveScenePanelIndex] != null)
            {
                flashcardManagers[currentActiveScenePanelIndex].ResetAllButtons();
            }
        }

        // 新しいサブパネルを表示する
        sceneSubPanels[panelIndex].SetActive(true);
        currentActiveScenePanelIndex = panelIndex;

        // 新しいパネルのFlashcardManagerをセットアップ（データをボタンに設定）
        if (flashcardManagers[panelIndex] != null)
        {
            flashcardManagers[panelIndex].SetupPanel();
        }

        // BackButtonとGotoButtonの表示状態を調整（ScenePanelsでは表示するなど）
        if (BackButton != null) BackButton.gameObject.SetActive(true);
        if (BackInitButton != null) BackInitButton.gameObject.SetActive(false);
    }
    //==========================================================================================
    // StartPanel内のボタンがクリックされた時の処理
    void OnStartPanelButtonClicked(int index)
    {
        ShowScenePanel(index);
    }
    //==========================================================================================
    // BackButtonがクリックされた時の処理
    void OnBackButtonClicked()
    {
        ShowStartPanel();
    }
    //==========================================================================================
    void OnBackInitButtonClicked()
    {
        GoTo_Dsp_Conjugation();
    }

}