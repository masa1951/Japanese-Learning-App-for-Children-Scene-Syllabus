using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
//using UnityEditor.VersionControl;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
// using UnityEditor; // この行を追加
using System.Linq; // この行を追加
//using UnityEngine.UIElements;

public class VerbManager : MonoBehaviour

    {
    public const int DimMax = 340;
    //　読み込んだテキストを出力するUIテキスト
    public TMP_Text dataText0;        // ルビ1
    public TMP_Text dataText1;        // 現在形　活用形１
    public TMP_Text dataText2;        // 　過去形　ルビ２
    public TMP_Text dataText3;        // 　否定形　活用形２
    public TMP_Text dataText4;        // 　過去否定形　活用形２
    public TMP_Text dataText6;        // インフォメーション１
    public TMP_Text dataText7;        // インフォメーション２
    public TMP_Text dataText8;        // スライド秒数
    public TMP_Text dataText9;        // カウント数
    //　Resourcesフォルダから直接テキストを読み込む
    private string ldTxtR1, ldTxtV1, ldTxtV2, ldTxtV3, ldTxtV4;
    private string Vkind1, Vkind2, Vkind3, Vkind4;
    private string Aconjug;
    //private string Bconjug;
    //　改行で分割して配列に入れる
    private string[] spltTxtR1, spltTxtV1, spltTxtV2, spltTxtV3, spltTxtV4;
    private string[] spltVkind1 = new string[DimMax];
    private string[] spltVkind2 = new string[DimMax];
    private string[] spltVkind3 = new string[DimMax];
    private string[] spltVkind4 = new string[DimMax];
    private int[] vKindNo = new int[DimMax];

    //    public GameObject panelPages;       // page1 Title, page2 Select, page3 Training
    public GameObject Mainpanels, Selectpanels;
    public LessonToggleManager lessonToggleManager;

    //----------  SELECT Panel ------------------------------------------------------------------------------------------------------
    public Toggle toggleAmasu; public Toggle toggleAjisho;
    public Toggle toggleG1;
    public Toggle toggleG11; public Toggle toggleG12; public Toggle toggleG13; public Toggle toggleG14; public Toggle toggleG15;
    public Toggle toggleG2; public Toggle toggleG3;

    //----------  SELECT Panel ------------------------------------------------------------------------------------------------------
    // =====
    //    public Sprite verb1, verb2, verb3, verb4, verb5, verb6, verb7・・・・・, verb298, verb299, verb300;
    // 公開リストにすることで、インスペクターから確認できます
    public List<Sprite> verbSprites = new List<Sprite>();

    //　現在表示中テキスト番号
    private int num1;           // 選択された動詞の表示した「数」　最初は [0]
    //    private int flg0;   // flg0 = 0 ます形　flg0 = 1 辞書形
    private int flg1;   // ToggleG1のON/OFFチェック
    private int flg2;   // 表示の順の確認
    private int Sc_Position;
    private bool formFlg = true;        //true : masu, false : jisho
    private bool pauseFlg = false;
    public Button Initial_Button, Conjug_Button, Syllabus_Button, Back1_Button;     // Initial_ButtonはInspectorで設定
    public Button TextBook1_Button, TextBook2_Button, TextBook3_Button, TextBook4_Button, TextBook5_Button;
    public Button TextBook6_Button;         // Syllabus 動詞活用
    //
    public TMP_Text TrainingText_TMP;
    public Button SoundButton, RandomButton; //, DesinenceButton;
    public TMP_Text SoundText_TMP, RandomText_TMP; //, DesinenceText_TMP;
    public Sprite ButtonVoice, ButtonVoiceB;
    public Sprite Shuffle, Sequence;
    public Sprite ButtonRed, ButtonPink;
    public Button StepButton, BackButton;           //, RetalkButton;
    public Button AutoButton, ClrButton, SelectButton;
    public TMP_Text AutoButton_TMP;
    //
    public int currentBookPrefix;
    //
    public AudioClip offBeep, pressBeep;   // Use when Voice OFF
    AudioSource verbSource;
    public List<AudioClip> audioClipJisho_present = new List<AudioClip>();
    public List<AudioClip> audioClipJisho_past = new List<AudioClip>();
    public List<AudioClip> audioClipJisho_negative = new List<AudioClip>();
    public List<AudioClip> audioClipJisho_past_negative = new List<AudioClip>();
    public List<AudioClip> audioClipMasu_present = new List<AudioClip>();
    public List<AudioClip> audioClipMasu_past = new List<AudioClip>();
    public List<AudioClip> audioClipMasu_negative = new List<AudioClip>();
    public List<AudioClip> audioClipMasu_past_negative = new List<AudioClip>();

    private int CountLE = 0;                       //選択された総数

    public float spdTime;
    private Image ImagePic;
    private Image FormImage;
    private int[] tempNo = new int[DimMax]; 		//2次元から1次元に変更
    private int[] dspNo = new int[DimMax];

    private bool soundFlg = true;
    private bool randomFlg = true;

    Slider _slider;
    //||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
#if UNITY_EDITOR
    [ContextMenu("Load All Audio Clips")]
    void LoadAllAudioClips()
    {
        LoadAudioClips("jisho_present", audioClipJisho_present);
        LoadAudioClips("jisho_past", audioClipJisho_past);
        LoadAudioClips("jisho_negative", audioClipJisho_negative);
        LoadAudioClips("jisho_past_negative", audioClipJisho_past_negative);
        LoadAudioClips("masu_present", audioClipMasu_present);
        LoadAudioClips("masu_past", audioClipMasu_past);
        LoadAudioClips("masu_negative", audioClipMasu_negative);
        LoadAudioClips("masu_past_negative", audioClipMasu_past_negative);
        Debug.Log("すべてのオーディオクリップをロードしました。");
    }

    void LoadAudioClips(string folderName, List<AudioClip> audioList)
    {
        audioList.Clear(); // 既存のリストをクリア

        // Resources.LoadAllを使って、指定されたフォルダ内のすべてのAudioClipをロード
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds_1/" + folderName);

        // ファイル名を数値でソート
        // 例: "0.mp3", "1.mp3", ... "298.mp3" の順に並べる
        List<AudioClip> sortedClips = new List<AudioClip>(clips);
        sortedClips.Sort((a, b) => {
            string aName = Path.GetFileNameWithoutExtension(a.name);
            string bName = Path.GetFileNameWithoutExtension(b.name);
            int aNum, bNum;
            if (int.TryParse(aName, out aNum) && int.TryParse(bName, out bNum))
            {
                return aNum.CompareTo(bNum);
            }
            return 0;
        });

        // ソートされたクリップをリストに追加
        audioList.AddRange(sortedClips);
        Debug.Log($"フォルダ '{folderName}' から {audioList.Count} 個のオーディオクリップをロードしました。");
    }
#endif
    //||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
    // publicとすることで、UnityエディタのInspectorウィンドウから
    // 作成したLessonVerbDataアセットをドラッグ＆ドロップで設定できるようになります。
    public LessonVerbs lessonVerbs;
    public List<int> GetVerbsLesson(int lessonNum)
    {
        if (lessonVerbs == null)
        {
            Debug.LogError("LessonVerbs is not assigned in VerbManager! Please assign the LessonVerbData asset in the Inspector.");
            return new List<int>(); // 設定されていない場合は空のリストを返す
        }
        return lessonVerbs.GetVerbsLesson(lessonNum);
    }
    public List<int> checkNo = new List<int>(); // 整数型に変更

    //||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
    public void GoToSelectScene()
    {
        SceneManager.LoadScene("SelectScene");
    }

    //||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
    //    public LessonVerbData lessonVerbData; // Inspectorから設定
    // LessonToggleManagerが動詞データを更新する際に呼び出すメソッド
    // LessonToggleManagerから選択された課のリストを受け取るように変更
    public void UpdateCheckNoFromToggles(List<int> selectedLessonNumbers)   //========================
    {
        checkNo.Clear(); // 既存のリストをクリア

        if (lessonVerbs == null)
        {
            Debug.LogError("LessonVerbData is not assigned in VerbManager! Please assign it in the Inspector.");
            return;
        }
    //***************************************************************
        foreach (int lessonNum in selectedLessonNumbers)
        {
            // ScriptableObjectから動詞リストを取得し、checkNoに追加
            List<int> verbs = lessonVerbs.GetVerbsLesson(lessonNum);
            if (verbs != null && verbs.Count > 0)
            {
                checkNo.AddRange(verbs);
            }
        }

        Debug.Log($"VerbManager: checkNoに{checkNo.Count}個の動詞が取り込まれました。");
        // Debug.Log($"checkNoの内容: {string.Join(", ", checkNo)}"); // デバッグ用
    }
    //||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
    //=+=+=+=+=+=+=+=+=+=+=+= START +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
    void Start()
    {
        LoadVerbSprites();

        if (SceneManager.GetActiveScene().name != "Dsp_Conjugation")
            return;

        // ボタンのOnClickイベントにChangeTextメソッドを登録
        Initial_Button.onClick.AddListener(PushInitialButton);          // 教科書選定ボタンから初期画面へ?
        Back1_Button.onClick.AddListener(PushBack1_Button);             // 同　下 ↓
        Conjug_Button.onClick.AddListener(Push_Conjug_Button);          // 教科書選定場面へ
        TextBook1_Button.onClick.AddListener(PushTextBook1_Button);
        TextBook2_Button.onClick.AddListener(PushTextBook2_Button);
        TextBook3_Button.onClick.AddListener(PushTextBook3_Button);
        TextBook4_Button.onClick.AddListener(PushTextBook4_Button);
        TextBook5_Button.onClick.AddListener(PushTextBook5_Button);
        TextBook6_Button.onClick.AddListener(PushTextBook6_Button);     // Syllabus Conjugation

        Syllabus_Button.onClick.AddListener(Push_Syllabus_Button);
        SoundButton.onClick.AddListener(PushSoundButton);
        SoundText_TMP.text = "音声Voice";
        RandomButton.onClick.AddListener(PushRandomButton);
        SelectButton.onClick.AddListener(PushSetButton);        //Config Button
        ClrButton.onClick.AddListener(PushClrButton);
        AutoButton.onClick.AddListener(PushAutoButton);
        BackButton.onClick.AddListener(PushBackButton);
        StepButton.onClick.AddListener(PushStepButton);

        // Toggle 初期値
        toggleAmasu.isOn = true;            // toggleBjisho.isOn = true;
        toggleG1.isOn = true; toggleG11.isOn = true; toggleG12.isOn = true; toggleG13.isOn = true; toggleG14.isOn = true; toggleG15.isOn = true;
        toggleG2.isOn = true;
        toggleG3.isOn = true;

        _slider = GameObject.Find("Slider").GetComponent<Slider>();
        spdTime = 1.0f;         // ループ時間　1秒
        //-------------------------------------------------------------------------------------------
        ImagePic = GameObject.Find("ImagePic").GetComponent<Image>();
        FormImage = GameObject.Find("FormImage").GetComponent<Image>();
        //++++++++++++++++++++
        num1 = 0;
        flg2 = 0;         // 表示の順番の確認

        verbSource = gameObject.GetComponent<AudioSource>();
//        verbSource.clip = offBeep;
        verbSource.PlayOneShot(pressBeep);
    }
    //=+=+=+=+=+=+=+=+=+=+=+= +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
    private void LoadVerbSprites()
    {
        // Assets/Resources/Images/Minna フォルダからすべてのSpriteを読み込む
        // ファイル名は "verb1" から "verb297" の形式である必要があります
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/Minna");

        // ファイル名（"verb1", "verb2"など）の番号順にソートします
        // この処理がないと、読み込まれる順番が保証されません
        var sortedSprites = sprites.OrderBy(s => {
            string name = s.name;
            if (name.StartsWith("verb") && int.TryParse(name.Substring(4), out int number))
            {
                return number;
            }
            return int.MaxValue; // ソート対象外のファイルは最後に配置
        }).ToList();

        // ソートされたリストをverbSpritesに格納します
        verbSprites = sortedSprites;

        Debug.Log($"{verbSprites.Count}個の動詞の画像を読み込みました。");
    }

    //=+=+=+=+=+=+=+=+=+=+=+= +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
    private void TrainingInit()             // Data Load Section
    {
        // 活用種類の決定と表示
        if (toggleAmasu.isOn) { Aconjug = "masu"; formFlg = true; Sprite image = Resources.Load<Sprite>("Images/masu"); FormImage.sprite = image; }
        else if (toggleAjisho.isOn) { Aconjug = "jisho"; formFlg = false; Sprite image = Resources.Load<Sprite>("Images/jisho"); FormImage.sprite = image; }

        ldTxtR1 = (Resources.Load("Texts/" + Aconjug + "_present_r", typeof(TextAsset)) as TextAsset).text;
        ldTxtV1 = (Resources.Load("Texts/" + Aconjug + "_present", typeof(TextAsset)) as TextAsset).text;
        ldTxtV2 = (Resources.Load("Texts/" + Aconjug + "_past", typeof(TextAsset)) as TextAsset).text;
        ldTxtV3 = (Resources.Load("Texts/" + Aconjug + "_negative", typeof(TextAsset)) as TextAsset).text;
        ldTxtV4 = (Resources.Load("Texts/" + Aconjug + "_past_negative", typeof(TextAsset)) as TextAsset).text;

        Vkind1 = (Resources.Load("Texts/Vkind", typeof(TextAsset)) as TextAsset).text;
        Vkind2 = (Resources.Load("Texts/LesGp", typeof(TextAsset)) as TextAsset).text;
        Vkind3 = (Resources.Load("Texts/LesGp2", typeof(TextAsset)) as TextAsset).text;
        Vkind4 = (Resources.Load("Texts/LesGp3", typeof(TextAsset)) as TextAsset).text;

        spltTxtR1 = ldTxtR1.Split(char.Parse("\n"));    // "\r"を使うと改行がずれる
        spltTxtV1 = ldTxtV1.Split(char.Parse("\n"));
        spltTxtV2 = ldTxtV2.Split(char.Parse("\n"));
        spltTxtV3 = ldTxtV3.Split(char.Parse("\n"));
        spltTxtV4 = ldTxtV4.Split(char.Parse("\n"));
        // 動詞活用種類の配列　(1：G11) (2：G12) (3：G13) (4：G14) (5：G15) (6：Group2) (7:Group3)
        spltVkind1 = Vkind1.Split(char.Parse("\n"));
        spltVkind2 = Vkind2.Split(char.Parse("\n"));

        for (int i = 0; i < DimMax; i++)
        {
           print("i = " + i + "  " + vKindNo[i]);
           vKindNo[i] = Convert.ToInt32(spltVkind1[i]);

        }                 // 整数化  配列は「0」始まり
 //       print("0 vKindNo[DimMax-1]= " + "  " + vKindNo[DimMax] + " spltVkind3=" + spltVkind3 + "  spltVkind4=" + spltVkind4);

        spltVkind3 = Vkind3.Split(char.Parse("\n"));
        spltVkind4 = Vkind4.Split(char.Parse("\n"));

        reFillNo();     //配列dspNo[X,0]にチェックした課の番号を入力、入力総数はCountLE
        dataText9.text = (CountLE - num1).ToString() + " left";
//        print("_CountLE=" + CountLE + "   num1=" + num1);
    }
    //===================================================================
    private IEnumerator GetLloop()
    {
        while (true)
        {
            if (pauseFlg)
            {
                yield break;
            }
            //一秒ごと
            yield return new WaitForSeconds(spdTime);
            OnTimer();
        }
    }
    //=====
    //===================================================================
    private void OnTimer()    //１秒ごとに呼ばれる
    {
        dataText9.text = (CountLE - num1).ToString() + " left";
        print("CountLE="+CountLE+"   num1=" + num1 + "   dspNo[num1]=" + dspNo[num1]);       //
        switch (flg2)
        {
            case 0:
                Sprite image = Resources.Load<Sprite>("Images/Minna/verb" + (dspNo[num1]+1));
                ImagePic.sprite = image;
                dataText0.text = spltTxtR1[dspNo[num1]];                    // ルビ
                dataText1.text = spltTxtV1[dspNo[num1]];                    // 現在形
                dataText2.text = "";
                dataText3.text = "";
                dataText4.text = "";

                dataText6.text = spltVkind2[dspNo[num1]];
                dataText7.text = spltVkind3[dspNo[num1]] + "\n" + spltVkind4[dspNo[num1]];

                if (soundFlg)
                {
                    if (formFlg) { verbSource.PlayOneShot(audioClipMasu_present[dspNo[num1]]); }
                    else { verbSource.PlayOneShot(audioClipJisho_present[dspNo[num1]]); }
                }
                else verbSource.PlayOneShot(offBeep);
                flg2++;
                break;

            case 1:
                dataText2.text = spltTxtV2[dspNo[num1]];                    // 過去形
                if (soundFlg)
                {
                    if (formFlg) { verbSource.PlayOneShot(audioClipMasu_past[dspNo[num1]]); }
                    else { verbSource.PlayOneShot(audioClipJisho_past[dspNo[num1]]); }
                }
                else verbSource.PlayOneShot(offBeep);
                flg2++;
                break;

            case 2:
                dataText3.text = spltTxtV3[dspNo[num1]];                    //否定形
                if (soundFlg)
                {
                    if (formFlg) { verbSource.PlayOneShot(audioClipMasu_negative[dspNo[num1]]); }
                    else { verbSource.PlayOneShot(audioClipJisho_negative[dspNo[num1]]); }
                }
                else verbSource.PlayOneShot(offBeep);
                flg2++;
                break;

            case 3:
                dataText4.text = spltTxtV4[dspNo[num1]];                    //過去否定形
                if (soundFlg)
                {
                    if (formFlg) { verbSource.PlayOneShot(audioClipMasu_past_negative[dspNo[num1]]); }
                    else { verbSource.PlayOneShot(audioClipJisho_past_negative[dspNo[num1]]); }
                }
                else verbSource.PlayOneShot(offBeep);
                //flg2++;
                flg2 = 0;
                num1++;
                break;
            default:
                break;
        }
        if (CountLE <= num1)
        {             // ストップ処理
            pauseFlg = true;
            dataText9.text = "End\nPress CLR";
            AutoButton_TMP.text = "自　動\nAUTO";
            AutoButton.GetComponent<Image>().sprite = ButtonRed;
            num1 = 0;
            flg2 = 0;
        }
    }
    //===================================================================
    private void reFillNo() //配列dspNo[]にチェックした課の番号を入力、入力総数はCountLE
    {
        CountLE = checkNo.Count;
        print("1_CountLE = " + CountLE);

        //　「checkNo[i]」:選択された課の表示番号 
        //　CountLE    :選択された表示総数
        int j = 0;
        for (int i = 0; i < CountLE; i++)  // すべて語彙数
        {
        //    print("i=" + i + "(" + checkNo[i] + ") , "+ vKindNo[checkNo[i]]);
         //   print("j=" + j );

            switch (vKindNo[checkNo[i]])      // すべての語彙の動詞活用種類　
                                              // 選択された課の動詞が、選択された動詞種類であるかどうか
            {
                case 1:
                    if (toggleG11.isOn) { tempNo[j] = checkNo[i]; j++; }                              // グループ１　「う・つ・る」
                    break;
                case 2:
                    if (toggleG12.isOn) { tempNo[j] = checkNo[i]; j++; }                             // グループ１　「ぶ・む・ぬ」
                    break;
                case 3:
                    if (toggleG13.isOn) { tempNo[j] = checkNo[i]; j++; }                              // グループ１　「す」
                    break;
                case 4:
                    if (toggleG14.isOn) { tempNo[j] = checkNo[i]; j++; }                             // グループ１　「く」
                    break;
                case 5:
                    if (toggleG15.isOn) { tempNo[j] = checkNo[i]; j++; }                              // グループ１　「ぐ」
                    break;
                case 6:
                    if (toggleG2.isOn) { tempNo[j] = checkNo[i]; j++; }                               // グループ２　「る」
                    break;
                case 7:
                    if (toggleG3.isOn) { tempNo[j] = checkNo[i]; j++; }                               // グループ３　｛する・くる｝
                    break;
                case 8:
                    tempNo[j] = checkNo[i]; j++;                                                      // 形容詞他　無条件追加
                    break;
                default:
                    break;
            }
        }
        CountLE = j;
    //    print("2_CountLE=" + CountLE);
        for (int i = 0; i < CountLE; i++) { checkNo[i] = tempNo[i]; }    //
                                                                         // checkNo[] 選択された動詞のリスト　　　　上記のラインはなぜ必要だったか？　＊＊＊シャッフルとシーケンシャルの両方を保つため
                                                                         // tempNo[]  選択された動詞のリストから活用トグルがオンである動詞のリスト
                                                                         //        print("3_CountLE=" + CountLE + "   num1=" + num1);

         Randomization(); 

        //　「tempNo[i,1]」:選択された課の表示番号「tempNo[i,0]」の中における、選択されている動詞活用の表示番号
        //　選択された表示総数(j)　=> CountLE
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------
    public void PushG1ToggleButton()
    {
        if (flg1 == 0)
        {
            flg1 = 1;
            toggleG11.isOn = true; toggleG12.isOn = true; toggleG13.isOn = true; toggleG14.isOn = true; toggleG15.isOn = true;
        }
        else
        {
            flg1 = 0;
            toggleG11.isOn = false; toggleG12.isOn = false; toggleG13.isOn = false; toggleG14.isOn = false; toggleG15.isOn = false;
        }

    }
    //-------------------------------------------------------------------------
    //=================================================================== 乱数化ルーチン
    //　ランダム化  ランダム前配列 tempNo[i,1] 乱数化後は変化する「ランダム後配列 dspNo[i]」　
    //　ランダム化する総数 CountLE　配列開始番号は０から(配列の使用が「0」からか「1」からかで混乱するので注意）
    public void Randomization()
    {
        int CountE = CountLE;          //ランダム化する総数
        for (int i = 0; i < CountLE; i++) //０から総数までカウントアップ
        {
            int RndJ = UnityEngine.Random.Range(0, CountE); // RndJ:「０からCountE」までの整数乱数
            dspNo[i] = tempNo[RndJ];                      // 表示配列にランダム前配列のランダム数位置の数字を入力
            for (int j = RndJ; j < CountE; j++) { tempNo[j] = tempNo[j + 1]; }
            CountE--;                                       // ダウンカウント
        }
    }
    //===================================================================
    public void PushSoundButton()
    {
        if (soundFlg)
        {
            soundFlg = false;
            SoundText_TMP.text = "ビープ\nbeep";
            SoundButton.GetComponent<Image>().sprite = ButtonVoiceB;
        }
        else
        {
            soundFlg = true;
            SoundText_TMP.text = "音声Voice";
            SoundButton.GetComponent<Image>().sprite = ButtonVoice;
        }
    }
    //===================================================================
    public void PushRandomButton()
    {
        PushClrButton();
        if (randomFlg)
        {
            randomFlg = false;
//            RandomText_TMP.text = "<size=28>表示順序 順次\r\n<size=20><color=white>Display Order Sequential";
            RandomButton.GetComponent<Image>().sprite = Sequence;
            for (int i = 0; i < CountLE; i++) { dspNo[i] = checkNo[i]; }
        }
        else
        {
            randomFlg = true;
//            RandomText_TMP.text = "<size=28>表示順序\r\nランダム\r\n<size=22><color=white>Display Order Random";
            RandomButton.GetComponent<Image>().sprite = Shuffle;
            for (int i = 0; i < CountLE; i++) { tempNo[i] = checkNo[i]; }
            Randomization();
        }
    }
    //===================================================================
    public void PushAutoButton()
    {
        if (pauseFlg)
        {
            pauseFlg = false;
            AutoButton_TMP.text = "停　止\nSTOP";
            AutoButton.GetComponent<Image>().sprite = ButtonPink;
            StartCoroutine(GetLloop());
        }
        else
        {
            AutoButton_TMP.text = "自　動\nAUTO";
            AutoButton.GetComponent<Image>().sprite = ButtonRed;
            pauseFlg = true;
        }

    }
    //===================================================================
    public void PushStepButton()
    {
        OnTimer();
        //        print("0:flg2=" + flg2 + "  num1=" + num1);
    }
    //===================================================================
    public void PushBackButton()
    {
        if (num1 == 0 && flg2 == 0) { verbSource.PlayOneShot(offBeep); } // NoVoice 
        else
        {
            switch (flg2)
            {
                case 0:                                                      // 第４表示後　flg2=0
                    dataText4.text = "";
                    num1--;
                    flg2 = 3;
                    break;
                case 1:                                                      // 第１表示後　flg2=1
                    dataText0.text = "";
                    dataText1.text = "";
                    flg2--;
                    break;
                case 2:                                                      // 第２表示後　flg2=2
                    dataText2.text = "";
                    flg2--;
                    break;
                case 3:                                                      // 第３表示後　flg2=3
                    dataText3.text = "";
                    flg2--;
                    break;
                default:
                    break;
            }
        }
    }
    //===================================================================
    public void PushClrButton()
    {
        num1 = 0;
        flg2 = 0;
        dataText9.text = (CountLE - num1).ToString() + " left";
        dataText0.text = ""; dataText1.text = ""; dataText2.text = ""; dataText3.text = ""; dataText4.text = ""; dataText6.text = ""; dataText7.text = "";
        Sprite image = Resources.Load<Sprite>("Images/masa");
        ImagePic.sprite = image;

    }
    //===================================================================
    public void PushInitialButton()
    {
        verbSource.PlayOneShot(pressBeep);
        pauseFlg = true;
        Mainpanels.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }
    //===================================================================
    public void PushSetButton()
    {
        RandomButton.GetComponent<Image>().sprite = Shuffle;    //　必ずランダムになるのでボタン画像の設定
        pauseFlg = true;
        //        print("Press Set_Button");
        //        Mainpanels.transform.localPosition = new Vector3(0.0f, 1500.0f, 0.0f);
        //        Selectpanels.transform.localPosition = new Vector3(2500.0f, 0.0f, 0.0f);
        Mainpanels.transform.localPosition = new Vector3(0.0f, 3000.0f, 0.0f);
        Selectpanels.transform.localPosition = new Vector3(0.0f, Sc_Position, 0.0f);
    }
    //===================================================================
    public void SliderChange()
    {
        spdTime = _slider.value;
        dataText8.text = spdTime.ToString("#,0.#") + " sec";
    }
    //===================================================================
    void Update() { }
    //===================================================================
    public void Push_Conjug_Button()
    {
        verbSource.PlayOneShot(pressBeep);
        Mainpanels.transform.localPosition = new Vector3(0.0f, 1500.0f, 0.0f);
    }
    //===================================================================
    public void PushBack1_Button()
    {
//        print("Press Back1_Button");
        Mainpanels.transform.localPosition = new Vector3(0.0f, 1500.0f, 0.0f);
        Selectpanels.transform.localPosition = new Vector3(2500.0f, 0.0f, 0.0f);
    }
    //===================================================================
    //===================================================================
    public void PushTextBook1_Button()
    {
        currentBookPrefix = 100; Sc_Position = -340;
        Mainpanels.transform.localPosition = new Vector3(0.0f, 3000.0f, 0.0f);
        Selectpanels.transform.localPosition = new Vector3(0.0f, Sc_Position, 0.0f);
    }
    //===================================================================
    public void PushTextBook2_Button()
    {
        currentBookPrefix = 200; Sc_Position = 1160;
        Mainpanels.transform.localPosition = new Vector3(0.0f, 3000.0f, 0.0f);
        Selectpanels.transform.localPosition = new Vector3(0.0f, Sc_Position, 0.0f);
        //    Selectpanels.transform.localPosition = new Vector3(0.0f, 1160.0f, 0.0f);
    }
    //===================================================================
    public void PushTextBook3_Button()
    {
        currentBookPrefix = 300; Sc_Position = 2660;
        Mainpanels.transform.localPosition = new Vector3(0.0f, 3000.0f, 0.0f);
        Selectpanels.transform.localPosition = new Vector3(0.0f, Sc_Position, 0.0f);
        //    Selectpanels.transform.localPosition = new Vector3(0.0f, 2660.0f, 0.0f);
    }
    //===================================================================
    public void PushTextBook4_Button()
    {
        currentBookPrefix = 400; Sc_Position = 4160;
        Mainpanels.transform.localPosition = new Vector3(0.0f, 3000.0f, 0.0f);
        Selectpanels.transform.localPosition = new Vector3(0.0f, Sc_Position, 0.0f);
        //    Selectpanels.transform.localPosition = new Vector3(0.0f, 4160.0f, 0.0f);
    }
    //===================================================================
    public void PushTextBook5_Button()
    {
        currentBookPrefix = 500; Sc_Position = 5660;
        Mainpanels.transform.localPosition = new Vector3(0.0f, 3000.0f, 0.0f);
        Selectpanels.transform.localPosition = new Vector3(0.0f, Sc_Position, 0.0f);
        //    Selectpanels.transform.localPosition = new Vector3(0.0f, 5660.0f, 0.0f);
    }
    //===================================================================
    public void PushTextBook6_Button()
    {
        currentBookPrefix = 600; Sc_Position = 7160;
        Mainpanels.transform.localPosition = new Vector3(0.0f, 3000.0f, 0.0f);
        Selectpanels.transform.localPosition = new Vector3(0.0f, Sc_Position, 0.0f);
        //    Selectpanels.transform.localPosition = new Vector3(0.0f, 7160.0f, 0.0f);
    }
    //===================================================================
    public void Push_Syllabus_Button()
    {
        verbSource.PlayOneShot(pressBeep);
        GoToSelectScene();
     //   SceneManager.LoadScene("SyllabusScene");
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    public void PushGo_Button()
    {
        lessonToggleManager.DisableOtherBookToggles(currentBookPrefix);
        // 選択された課の動詞リストを取得し、VerbManagerを更新
        List<int> selectedLessons = lessonToggleManager.GetCheckedLessonNumbers();
        UpdateCheckNoFromToggles(selectedLessons);

        SliderChange();
        PushClrButton();
        TrainingInit();
        if (CountLE != 0)
        {
        //    PushRandomButton();             //
            Mainpanels.transform.localPosition = new Vector3(0.0f, 4500.0f, 0.0f);
            Selectpanels.transform.localPosition = new Vector3(2500.0f, 0.0f, 0.0f);
        }
        else
        {
//            TrainingText_TMP.text = "<size=36>「動詞グループ」「課」を選んで\nください";
            TrainingText_TMP.text = "<size=36>「動詞グループ」「課」を選んでください";
        }
    }
    //===================================================================
    public void PushCurrentAllOn_Button()
    {
        lessonToggleManager.CurrentBookTogglesAllOn(currentBookPrefix);
    }
    //===================================================================
    public void PushCurrentAllOff_Button()
    {
        lessonToggleManager.CurrentBookTogglesAllOff(currentBookPrefix);
    }
}
