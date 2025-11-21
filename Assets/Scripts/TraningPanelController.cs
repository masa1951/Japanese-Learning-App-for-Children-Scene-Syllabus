using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingPanelController : MonoBehaviour
{
    public LessonToggleManager lessonToggleManager;
    public VerbManager verbManager;
    public GameObject trainingPanel; // トレーニングパネルのGameObject
    public GameObject Mainpanels, Selectpanels;


    public void GoToTrainingPanel()
    {
        Mainpanels.transform.localPosition = new Vector3(0.0f, 4500.0f, 0.0f);
        Selectpanels.transform.localPosition = new Vector3(2500.0f, 0.0f, 0.0f);

        // 選択された課の動詞リストを取得し、VerbManagerを更新
        List<int> selectedLessons = lessonToggleManager.GetCheckedLessonNumbers();
        verbManager.UpdateCheckNoFromToggles(selectedLessons);

        // トレーニングパネルに移動 (例:SetActive)
        // trainingPanel.SetActive(true);
        // ここで現在のパネルを非表示にするなども
        Debug.Log("トレーニングパネルへ移動し、動詞データをロードしました。");
    }
}