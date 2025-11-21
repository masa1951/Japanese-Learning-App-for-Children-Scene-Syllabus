using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LessonVerbs", menuName = "Scriptable Objects/LessonVerbs")]
public class LessonVerbs : ScriptableObject
{
    // 各課の動詞リストを保持するクラス
    [System.Serializable]
    public class VerbList
    {
        public int lessonNumber;
        public List<int> verbNumbers;
    }

    public List<VerbList> allLessonVerbs = new List<VerbList>();

    // 特定の課の動詞リストを取得するメソッド
    public List<int> GetVerbsLesson(int lessonNum)
    {
        foreach (var item in allLessonVerbs)
        {
            if (item.lessonNumber == lessonNum)
            {
                return item.verbNumbers;
            }
        }
        Debug.LogWarning($"Lesson number {lessonNum} not found in LessonVerbData.");
        return null;
    }
}
