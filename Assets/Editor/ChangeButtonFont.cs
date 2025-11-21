using UnityEditor;
using UnityEngine;
using TMPro; // TextMeshProを使用するため

public class ChangeButtonFont : EditorWindow
{
    private TMP_FontAsset newFontAsset;

    [MenuItem("Tools/Change Button Font")]
    public static void ShowWindow()
    {
        GetWindow<ChangeButtonFont>("Change Button Font");
    }

    void OnGUI()
    {
        GUILayout.Label("Font Asset for Buttons", EditorStyles.boldLabel);

        newFontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("New Font Asset", newFontAsset, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Change Fonts"))
        {
            ChangeSelectedButtonFonts();
        }
    }

    void ChangeSelectedButtonFonts()
    {
        if (newFontAsset == null)
        {
            Debug.LogError("新しいフォントアセットが設定されていません。");
            return;
        }

        // Hierarchyで選択されているすべてのGameObjectを対象にする
        foreach (GameObject obj in Selection.gameObjects)
        {
            // Buttonコンポーネントがあるか、またはその子にButtonがあるかをチェック
            // 今回はHierarchyのPanelの下に配置されている想定なので、Panelの子オブジェクトを直接探す方が良いかもしれません
            // ここでは汎用的に選択オブジェクトの子孫を探す例とします
            TextMeshProUGUI[] tmProTexts = obj.GetComponentsInChildren<TextMeshProUGUI>(true); // 非アクティブなものも含む

            foreach (TextMeshProUGUI tmProText in tmProTexts)
            {
                // 親にButtonコンポーネントがあるか確認するなど、さらに絞り込むことも可能
                // 例えば、ボタンの子としてText (TMP) がある場合
                if (tmProText.GetComponentInParent<UnityEngine.UI.Button>() != null)
                {
                    tmProText.font = newFontAsset;
                    EditorUtility.SetDirty(tmProText); // 変更を保存するために必要
                    Debug.Log($"Changed font of {tmProText.name} to {newFontAsset.name}");
                }
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log("フォントアセットの変更が完了しました。");
    }
}