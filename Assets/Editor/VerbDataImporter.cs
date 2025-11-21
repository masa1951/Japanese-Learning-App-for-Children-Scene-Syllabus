using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class VerbDataImporter
{
    [MenuItem("Tools/Import Verb Data from CSV")]
    public static void ImportVerbData()
    {
        string path = EditorUtility.OpenFilePanel("Select CSV file for Verb Data", "", "csv");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        // ScriptableObjectのアセットを作成または取得
        LessonVerbs lessonVerbs = AssetDatabase.LoadAssetAtPath<LessonVerbs>("Assets/LessonVerbs.asset");
        if (lessonVerbs == null)
        {
            lessonVerbs = ScriptableObject.CreateInstance<LessonVerbs>();
            AssetDatabase.CreateAsset(lessonVerbs, "Assets/LessonVerbs.asset");
        }
        else
        {
            // 既存のデータをクリア
            lessonVerbs.allLessonVerbs.Clear();
        }

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');

            if (parts.Length > 0)
            {
                if (int.TryParse(parts[0], out int lessonNumber))
                {
//                    LessonVerbData.VerbList verbList = new LessonVerbData.VerbList();
                    LessonVerbs.VerbList verbList = new LessonVerbs.VerbList();
                    verbList.lessonNumber = lessonNumber;
                    verbList.verbNumbers = new List<int>();

                    for (int i = 1; i < parts.Length; i++)
                    {
                        if (int.TryParse(parts[i].Trim(), out int verbNumber))
                        {
                            verbList.verbNumbers.Add(verbNumber);
                        }
                    }
                    lessonVerbs.allLessonVerbs.Add(verbList);
                }
            }
        }

        EditorUtility.SetDirty(lessonVerbs); // 変更を保存
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Verb data imported successfully from CSV!");
    }
}