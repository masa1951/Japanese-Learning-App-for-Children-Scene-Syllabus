using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System; // Dictionary操作のために必要

public class LessonToggleManager : MonoBehaviour
{
    // 各教科書に対応するパネルとトグルの親オブジェクト
    [System.Serializable]
    public class BookPanel
    {
        public GameObject panelGameObject; // 教科書ごとのパネルオブジェクト
        public List<Toggle> lessonToggles; // そのパネル内の課トグルリスト
    }

    [Header("教科書ごとのパネルとトグル")]
    public List<BookPanel> bookPanels = new List<BookPanel>();

    // どの教科書のどの課が選択されているかを管理するDictionary
    // Key: 課番号 (例: 117, 205, 520)
    // Value: その課のToggleへの参照
    private Dictionary<int, Toggle> allLessonToggles = new Dictionary<int, Toggle>();

    // VerbManagerへの参照 (後でデータを渡すため)
    [SerializeField] private VerbManager verbManager;

    //========================================================================================
    [System.Obsolete]
    void Awake()
    {
     /* // VerbManagerがシーン内に自動的に見つかるようにする
        if (verbManager == null)
        {
            verbManager = FindObjectOfType<VerbManager>();
            if (verbManager == null)
            {
                Debug.LogError("VerbManagerが見つかりません。LessonToggleManagerは正しく機能しません。", this);
            }
        }
     */
        InitializeToggles();
    }

    /// <summary>=========================================================================
    /// 全てのトグルを初期化し、イベントリスナーを設定します。
    /// </summary>
    private void InitializeToggles()
    {
        foreach (var bookPanel in bookPanels)
        {
            if (bookPanel.panelGameObject == null)
            {
                Debug.LogWarning($"BookPanelのパネルオブジェクトが割り当てられていません。スキップします。", this);
                continue;
            }
            foreach (Toggle toggle in bookPanel.lessonToggles)
            {
                int lessonNum = GetLessonNumberFromToggleName(toggle.name);
                if (lessonNum != 0) // 有効な課番号が取得できた場合
                {
                    if (!allLessonToggles.ContainsKey(lessonNum))
                    {
                        allLessonToggles.Add(lessonNum, toggle);
                    }
                    else
                    {
                        Debug.LogWarning($"課番号 {lessonNum} のトグルが重複しています。");
                    }
                }
            }
        }
        Debug.Log($"LessonToggleManager: {allLessonToggles.Count}個のトグルが初期化されました。");
    }

    /// <summary>=========================================================================
    /// トグルの名前から課番号を解析するヘルパーメソッド。
    private int GetLessonNumberFromToggleName(string toggleName)
    {
        string[] parts = toggleName.Split('_');
        if (parts.Length > 1 && int.TryParse(parts.Last(), out int lessonNum))
        {
            return lessonNum;
        }
        // 他の命名規則に対応する場合もここに追加
        // 例: トグルのTextコンポーネントから課番号を取得する など
        Debug.LogWarning($"トグル名 '{toggleName}' から課番号を解析できませんでした。", this);
        return 0; // 無効な番号
    }

    /// <summary>=========================================================================
    /// 現在チェックされている全ての課番号のリストを取得します。
    /// </summary>
    public List<int> GetCheckedLessonNumbers()
    {
        List<int> checkedLessons = new List<int>();
        foreach (var entry in allLessonToggles)
        {
            if (entry.Value.isOn)
            {
                checkedLessons.Add(entry.Key);
            }
        }
        return checkedLessons;
    }

    /// <summary>=========================================================================
    /// 指定された教科書番号以外の全てのトグルをオフにし、指定された教科書のトグル状態は変更しない。
    /// 他の教科書ボタンが押された際に呼び出す。
    /// </summary>
    /// <param name="currentBookPrefix">現在選択された教科書のプレフィックス (例: 100, 200, 300, 400, 500)</param>
    public void DisableOtherBookToggles(int currentBookPrefix)
    {
        foreach (var entry in allLessonToggles)
        {
            int lessonNumber = entry.Key;
            Toggle toggle = entry.Value;

            // 現在の教科書に属さないトグルをオフにする
            if (!IsLessonOfBook(lessonNumber, currentBookPrefix))
            {
                toggle.isOn = false;
            }
        }
        Debug.Log($"教科書 {currentBookPrefix / 100} 以外のトグルをオフにしました。");
        // トグルの状態が変わったので、VerbManagerに通知してcheckNoを更新させる
        verbManager?.UpdateCheckNoFromToggles(GetCheckedLessonNumbers());
    }


    /// <summary>=========================================================================
    /// 指定された課番号が、特定の教科書に属するかどうかを判定します。
    /// </summary>
    private bool IsLessonOfBook(int lessonNumber, int bookPrefix)
    {
        // 例: 課番号117は教科書1 (100) に属する
        // 例: 課番号205は教科書2 (200) に属する
        return (lessonNumber / 100) * 100 == bookPrefix;
    }
    /// <summary>=========================================================================
    public void CurrentBookTogglesAllOn(int currentBookPrefix)
    {
        foreach (var entry in allLessonToggles)
        {
            int lessonNumber = entry.Key;
            Toggle toggle = entry.Value;

            if (IsLessonOfBook(lessonNumber, currentBookPrefix))
            {   toggle.isOn = true;  }
        }
        Debug.Log("現在のトグルをオンにしました。");
        // トグルの状態が変わったので、VerbManagerに通知してcheckNoを更新させる
        verbManager?.UpdateCheckNoFromToggles(GetCheckedLessonNumbers());

    }
    /// <summary>=========================================================================
    public void CurrentBookTogglesAllOff(int currentBookPrefix)
    {
        foreach (var entry in allLessonToggles)
        {
            int lessonNumber = entry.Key;
            Toggle toggle = entry.Value;

            if (IsLessonOfBook(lessonNumber, currentBookPrefix))
            {   toggle.isOn = false; }
        }
        Debug.Log("現在のトグルをオフにしました。");
        // トグルの状態が変わったので、VerbManagerに通知してcheckNoを更新させる
        verbManager?.UpdateCheckNoFromToggles(GetCheckedLessonNumbers());

    }

    /// <summary>=========================================================================
    /// 全てのトグルをオフにします。
    /// 例: トレーニング終了時や、メニューに戻る際に呼び出す。
    /// </summary>
    public void TurnOffAllToggles()
    {
        foreach (var entry in allLessonToggles)
        {
            entry.Value.isOn = false;
        }
        Debug.Log("全てのトグルをオフにしました。");
        // トグルの状態が変わったので、VerbManagerに通知してcheckNoを更新させる
        verbManager?.UpdateCheckNoFromToggles(GetCheckedLessonNumbers());
    }

    internal void DisableOtherBookToggles()
    {
        throw new NotImplementedException();
    }
}