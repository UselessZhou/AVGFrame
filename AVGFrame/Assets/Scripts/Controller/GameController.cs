using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
//using Newtonsoft.Json.Linq;
using System.Text;
using Asserts.Scripts.Model;
//using Newtonsoft.Json;
using System;

public class GameController : MonoBehaviour
{
    public static GameController _instance;

    public Chapter chapter;
    private static string rootPath; //根目录
    private static string savedDatasPath; //剧本目录
    private static string savedDatasFile; //剧本文件名

    //UI
    public GameObject displayCanvas; //获取displayCanvas
    public GameObject logo; //Logo页面显示
    public Image bg;   //Title背景图片
    public GameObject titleContainer;

    //save页面
    public GameObject savedDataPanel;
    private List<SavedDataModel> savedDatas;

    //load页面
    private bool isLoadPanel;

    //qucikSave/Load
    private SavedDataModel qSavedData;
    private static string qSavedDataFile;

    //跳过序章Container
    private GameObject skipContainer;
    public SettingModel settingDatas;
    private static string settingDataFile;

    private GameObject extraContainer;

    //CGMode
    private GameObject cgPanel;
    private GameObject cgDetail;
    private Button fullCGBtn;
    private Button zeroCGBtn;
    private GameObject confirmPanel;

    private GameObject bgmPanel;
    private AudioSource bgmSource;
    //private bool isPlayMusic;

    public GameObject memoryPanel;
    private int[] memoryStartArray;
    private int[] memoryEndArray;
    //test
    //public CGIndexModel cgim = new CGIndexModel();

    public GameObject settingPannel;

    public string[][] cgArray;
    private ArrayList cgDetailArrayList;
    private string zeroCGArray;
    private string fullCGArray;

    private bool cgMode;    //CG模式


    private void Awake()
    {
        _instance = this;
        //logo.gameObject.SetActive(true);
    }

    private void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
        rootPath = Application.dataPath;
        savedDatasPath = rootPath + "/Resources/SavedData/";
        savedDatasFile = "savedData.json";
        qSavedDataFile = "quickSavedData.json";
        settingDataFile = "settingData.json";
        zeroCGArray = "0;0;0;0;0;0";
        fullCGArray = "1_1,1_2,1_3;2_1,2_in,2_out;3_1,3_2;4_1,4_2;5_1,5_2;6_1,6_2";
        titleContainer = displayCanvas.transform.Find("TitleContainer").gameObject;
        savedDataPanel = displayCanvas.transform.Find("SavedDataPanel").gameObject;
        skipContainer = displayCanvas.transform.Find("SkipContainer").gameObject;
        extraContainer = displayCanvas.transform.Find("ExtraContainer").gameObject;

        cgPanel = displayCanvas.transform.Find("CGPanel").gameObject;
        cgDetail = displayCanvas.transform.Find("CGDetail").gameObject;
        fullCGBtn = extraContainer.transform.Find("FullCG").GetComponent<Button>();
        zeroCGBtn = extraContainer.transform.Find("ZeroCG").GetComponent<Button>();
        confirmPanel = extraContainer.transform.Find("ConfirmPanel").gameObject;

        bgmPanel = displayCanvas.transform.Find("BGMPanel").gameObject;
        bgmSource = displayCanvas.transform.Find("AudioContainer").gameObject.transform.Find("TitleAudio").GetComponent<AudioSource>();

        memoryPanel = displayCanvas.transform.Find("MemoryPanel").gameObject;
        memoryStartArray = new int[] { 29, 46 };
        memoryEndArray = new int[] { 43, 67 };

        settingPannel = displayCanvas.transform.Find("SettingPanel").gameObject;
        savedDatas = new List<SavedDataModel>();
        //LoadDatas<List<SavedDataModel>>(savedDatasFile, savedDatas);
        qSavedData = new SavedDataModel();
       // LoadDatas<SavedDataModel>(qSavedDataFile, qSavedData);
        settingDatas = new SettingModel();
        //LoadDatas<SettingModel>(settingDataFile, settingDatas);
        //AnalyzeCGArray(settingDatas.cgSavedData);
        ChapterController._instance.noClothes = settingDatas.noClothes;//先要读取是否着装的设置

    }

    // Update is called once per frame
    void Update()
    {
        if (cgMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("按下");
                ShowNextCG();
            }
            if (Input.GetMouseButtonDown(1))
            {
                CloseCGDetail();
                cgMode = false;
            }
        }
    }

    private void OnApplicationQuit()
    {
        settingDatas.cgSavedData = CGArrayToString();
        //SaveDatas<SettingModel>(settingDataFile, settingDatas);
        //SaveDatas<SavedDataModel>(qSavedDataFile, qSavedData);
        //SaveDatas<List<SavedDataModel>>(savedDatasFile, savedDatas);
    }

    public void GoToTitle()
    {
        HideAllContainer();
        titleContainer.SetActive(true);
        PlayTitleBGM();
    }

    public void NewGame()
    {
        HideContainer(titleContainer);
        logo.SetActive(false);
        skipContainer.SetActive(true);
    }
    /*
    /// <summary>
    /// 从文件中获取存档的信息
    /// </summary>
    /// <returns></returns>
    /// 
    private void LoadDatas<T>(string fileName, T data)
    {
        if (!Directory.Exists(savedDatasPath))
        {
            Directory.CreateDirectory(savedDatasPath);
        }
        //使用using可在结束后销毁using括号内生成的资源变量
        using (FileStream fs = new FileStream(savedDatasPath + fileName, FileMode.OpenOrCreate)) //文件不存在直接创建
        {
            //读取文件内的所以内容，转换为SavedDataModel
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                string savedDataJson = sr.ReadToEnd();
                if (!string.IsNullOrEmpty(savedDataJson))
                {
                    if (data.GetType() == typeof(List<SavedDataModel>))
                    {
                        JArray savedDataArray = JArray.Parse(savedDataJson);
                        foreach (JToken jSavedData in savedDataArray)    //JToken为JObject的基类，用来遍历较好
                        {
                            SavedDataModel savedData = JsonConvert.DeserializeObject<SavedDataModel>(jSavedData.ToString());//反序列化Json
                            savedDatas.Add(savedData);

                        }
                    }
                    else
                    {
                        JToken jSavedData = JToken.Parse(savedDataJson);
                        if (null != jSavedData && jSavedData.Type != JTokenType.Null)
                        {
                            if (data.GetType() == typeof(SettingModel))
                            {
                                settingDatas = JsonConvert.DeserializeObject<SettingModel>(jSavedData.ToString());//反序列化Json
                            }else if (data.GetType() == typeof(SavedDataModel))
                            {
                                qSavedData = JsonConvert.DeserializeObject<SavedDataModel>(jSavedData.ToString());//反序列化Json
                            }
                        }
                    }
                }
                else
                {
                    InitData<T>(data);
                }
            }
        }
    }
    */
    private void InitData<T>(T data)
    {
        if(data.GetType() == typeof(SettingModel))
        {
            settingDatas = new SettingModel();
            settingDatas.cgSavedData = zeroCGArray;
            settingDatas.memoryIndex = 0;
            settingDatas.SEVolume = 0.5f;
            settingDatas.HSEVolume = 0.5f;
            settingDatas.BGMVolume = 0.5f;
            settingDatas.BGVVolume = 0.5f;
            settingDatas.dialogSpeed = 0.2f;
            settingDatas.skipSpeed = 0.2f;
            settingDatas.charactersVolume = new float[] { 0.5f, 0.5f, 0.5f, 0.5f };
            settingDatas.dialogTransparent = 0.5f;
            settingDatas.noClothes = false;
            settingDatas.isSkipReadedContext = false;
            settingDatas.isSkipUntilHScene = true;
            settingDatas.isSkipUntilShoot = true;
            settingDatas.shootNumber = 5;
            settingDatas.chapterIndex = 0;
            settingDatas.maxDialogIndex = 0;
            settingDatas.isContinuePlayCV = false;
            settingDatas.isChangeReadedTextColor = false;
            settingDatas.rightFunction = 1;
            settingDatas.shootChoices = 3;
            Debug.Log("cgdata: " + settingDatas.cgSavedData);
        }
        else if (data.GetType() == typeof(SavedDataModel))
        {
            qSavedData.savedDataIndex = 0;
            qSavedData.chapterIndex = 0;
            qSavedData.dialogInedx = 0;
        }
        else if (data.GetType() == typeof(List<SavedDataModel>))
        {
            Debug.Log("InitList");
            InitDataList(10);
        }

    }


    public void InitDataList(int size)
    {
        if (size > 0)
        {
            for (int i = 1; i <= size; i++)
            {
                SavedDataModel tempData = new SavedDataModel();
                tempData.chapterIndex = 0;
                tempData.dialogInedx = 0;
                tempData.savedTime = DateTime.Now;
                tempData.savedDataIndex = i;
                savedDatas.Add(tempData);
            }
        }
    }

    //显示s/l界面，并根据传入值判断是s还是l
    public void ShowSavedDataPanel(bool isLoadBtn)
    {
        isLoadPanel = isLoadBtn;
        ChapterController._instance.chapterMode = false;
        //隐藏一些页面，之后应该做成方法
        //hideContainers();

        savedDataPanel.SetActive(true);

        for(int i=0; i<savedDatas.Count; i++)
        {
            Button savedBtn = savedDataPanel.transform.Find(string.Format("SavedField{0}", i)).GetComponent<Button>();
            if (null != savedDatas[i] && !(savedDatas[i].dialogInedx == 0))
            {
                //savedBtn.transform.Find("SavedPic").GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Format("SavedData/{0}", savedDatas[i].savedPicName));
                savedBtn.transform.Find("Date").GetComponent<Text>().text = savedDatas[i].savedTime.ToString();
                savedBtn.transform.Find("SavedPic").GetComponent<Image>().sprite = (Sprite)Resources.Load("Image/ChapterBG/" + savedDatas[i].savedPicName, typeof(Sprite));
            }
            else
            {
                savedBtn.transform.Find("SavedPic").GetComponent<Image>().sprite = Resources.Load<Sprite>("SavedData/noData");
                savedBtn.transform.Find("Date").GetComponent<Text>().text = "----/--/-- --:--:--";
            }
        }
    }

    //点击s/l的按钮，根据index与s/l模式进行处理
    public void LoadAndShowDialog(int savedDataIndex)
    {
        if (isLoadPanel)
        {
            if (null != savedDatas[savedDataIndex] && !(savedDatas[savedDataIndex].dialogInedx == 0))
            {
                if (titleContainer.activeInHierarchy)
                {
                    HideContainer(titleContainer);
                }
                int dialogInedx = savedDatas[savedDataIndex].dialogInedx;
                int chapterIndex = savedDatas[savedDataIndex].chapterIndex;
                ChapterController._instance.bgmAudioSource.Stop();
                ChapterController._instance.bgvAudioSource.Stop();
                //ChapterController._instance.LoadXlsFile(chapterIndex);
                ChapterController._instance.dialogIndex = dialogInedx;
                ChapterController._instance.GetNextDialog();
            }
        }
        else
        {
            savedDatas[savedDataIndex] = ChapterController._instance.GetCurrentData();
            Debug.Log("PIC name: " + savedDatas[savedDataIndex].savedPicName);
            Debug.Log("Transform name: " + this.transform.name);
            
            savedDatas[savedDataIndex].savedDataIndex = savedDataIndex;
            //SaveDatas<List<SavedDataModel>>(savedDatasFile, savedDatas);
            GameObject btn = savedDataPanel.transform.Find("SavedField" + savedDataIndex.ToString()).gameObject;
            btn.transform.Find("SavedPic").gameObject.GetComponent<Image>().sprite = (Sprite)Resources.Load("Image/ChapterBG/" + savedDatas[savedDataIndex].savedPicName, typeof(Sprite));
            btn.transform.Find("Date").gameObject.GetComponent<Text>().text = savedDatas[savedDataIndex].savedTime.ToString();


    }
        //HideContainer(savedDataPanel);
    }

    /*
    /// <summary>
    /// 将存储数据存入本地
    /// </summary>
    public void SaveDatas<T>(string fileName, T datas)
    {
        if (!Directory.Exists(savedDatasPath))
        {
            Directory.CreateDirectory(savedDatasPath);
        }
        using (StreamWriter w = new StreamWriter(savedDatasPath + fileName, false, Encoding.UTF8))
        {
            string savedDataJson = JsonConvert.SerializeObject(datas);
            w.Write(savedDataJson);
        }
    }
    */
    public void SetQuickSavedData()
    {
        qSavedData = ChapterController._instance.GetCurrentData();
        //SaveDatas<SavedDataModel>(qSavedDataFile, qSavedData);
    }

    public void LoadQuickSavedData()
    {
        if(null != qSavedData && !(qSavedData.dialogInedx == 0))
        {
            int dialogInedx = qSavedData.dialogInedx;
            ChapterController._instance.dialogIndex = dialogInedx;
            ChapterController._instance.GetNextDialog();
        }
    }

    //public void CloseSavedDataPanel()
    //{
    //    savedDataPanel.SetActive(false);
    //}

    public void LinkBtn()
    {
        Application.OpenURL("www.baidu.com");
    }


    public void ShowExtraPanel()
    {
        HideContainer(titleContainer);
        extraContainer.SetActive(true);
    }

    public void ExitExtraContainerBtn()
    {
        extraContainer.SetActive(false);
    }

    public void ShowCGPanel()
    {
        cgPanel.SetActive(true);
        //int showNum = settingDatas.cgIndex;
        //int totalNum = 9;
        ////int showNum = 5;
        //for (int i = 1; i <= totalNum; i++)
        //{
        //    Button cgBtn = cgPanel.transform.Find(string.Format("CG{0}", i)).GetComponent<Button>();
        //    if (i <= showNum)
        //    {
        //        cgBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Format("Image/Mode/CG/ev{0}", i));
        //        cgBtn.onClick.AddListener(delegate ()
        //        {
        //            cgDetail.SetActive(true);
        //            Button detailBtn = cgDetail.transform.Find("Detail").GetComponent<Button>();
        //            detailBtn.GetComponent<Image>().sprite = cgBtn.GetComponent<Image>().sprite;
        //        });
        //    }
        //    else
        //    {
        //        cgBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Mode/CG/cgno");
        //    }
        //}
        for(int i=1; i <= cgArray.Length; i++)
        {
            Button cgBtn = cgPanel.transform.Find(string.Format("CG{0}", i)).GetComponent<Button>();
            if (!cgArray[i-1][0].Equals("0"))
            {
                string cgName = i + "_0";
                cgBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Format("Image/ChapterBG/ev{0}", cgName));
            }
            else
            {
                cgBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/ChapterBG/cgno");
            }
        }
    }

    public void ShowCGDetail(int cgIndex)
    {
        cgMode = true;
        cgDetailArrayList = new ArrayList();
        
        for(int i=0; i<cgArray.Length; i++)
        {
            if(i+1 == cgIndex)
            {
                for (int j = 0; j < cgArray[i].Length; j++)
                {
                    cgDetailArrayList.Add(cgArray[i][j]);
                }
            }
        }

        if (!cgDetailArrayList[0].Equals("0"))
        {
            cgDetail.SetActive(true);
            Image cgImage = cgDetail.transform.Find("CGImage").GetComponent<Image>();
            cgImage.sprite = Resources.Load<Sprite>(string.Format("Image/Mode/CG/ev{0}", cgDetailArrayList[0]));
            cgDetailArrayList.RemoveAt(0);
        }
    }

    public void AnalyzeCGArray(string array)
    {
        Debug.Log(array);
        string[] data = array.Split(';');
        cgArray = new string[data.Length][];
        for (int i = 0; i < data.Length; i++)
        {
            string[] tmpArray = data[i].Split(',');
            cgArray[i] = new string[tmpArray.Length];
            //if (tmpArray.Length != 1 || !tmpArray[0].Equals("0"))
            //{
                for (int j = 0; j < tmpArray.Length; j++)
                {
                    cgArray[i][j] = tmpArray[j];
                }
            //}

        }
    }
    
    public string CGArrayToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < cgArray.Length; i++)
        {
            for (int j = 0; j < cgArray[i].Length; j++)
            {
                sb.Append(cgArray[i][j]);
                if(j+1 < cgArray[i].Length)
                {
                    sb.Append(",");
                }
            }

            if (i + 1 < cgArray.Length)
            {
                sb.Append(";");
            }
        }
        return sb.ToString();
    }

    public void ShowNextCG()
    {
        if (cgDetailArrayList.Count > 0)
        {
            Image cgImage = cgDetail.transform.Find("CGImage").GetComponent<Image>();
            cgImage.sprite = Resources.Load<Sprite>(string.Format("Image/Mode/CG/ev{0}", cgDetailArrayList[0]));
            cgDetailArrayList.RemoveAt(0);
        }
        else
        {
            CloseCGDetail();
        }

    }

    public void CloseCGDetail()
    {
        cgDetail.SetActive(false);
    }

    public void ShowFullCG()
    {
        //settingDatas.cgIndex = 9;
        AnalyzeCGArray(fullCGArray);
        //SaveDatas<SettingModel>(settingDataFile, settingDatas);
        settingDatas.memoryIndex = 2;
    }

    public void ShowZeroCG()
    {
        //settingDatas.cgIndex = 0;
        AnalyzeCGArray(zeroCGArray);
        //SaveDatas<SettingModel>(settingDataFile, settingDatas);
        settingDatas.memoryIndex = 0;
    }

    public void ShowConfirmPannel()
    {
        confirmPanel.SetActive(true);
    }

    public void ExitConfirmPannel()
    {
        confirmPanel.SetActive(false);
    }

    public void ExitBtn()
    {
        cgPanel.SetActive(false);
    }

    public void ShowBGMPanel()
    {
        bgmPanel.SetActive(true);
    }

    //public void ExitBGMPanelBtn()
    //{
    //    bgmPanel.SetActive(false);
    //    if (isPlayMusic)
    //    {
    //        isPlayMusic = false;
    //        PlayTitleBGM();
    //    }
    //}

    public void PlayBGMBtn(string bgmName)
    {
        AudioClip bgm = (AudioClip)Resources.Load("BGM/" + bgmName);
        bgmSource.clip = bgm;
        bgmSource.Play();
        //isPlayMusic = true;
    }

    private void PlayTitleBGM()
    {
        AudioClip bgm = (AudioClip)Resources.Load("BGM/bgmTitle");
        bgmSource.clip = bgm;
        bgmSource.Play();
    }

    public void StopBGMBtn()
    {
        //isPlayMusic = false;
        bgmSource.Stop();
    }

    public void ShowSettingPanel()
    {
        ChapterController._instance.chapterMode = false;

        settingPannel.SetActive(true);
        ChangeSettingTab(1);
        settingPannel.transform.Find("SoundSetting").Find("BGM").GetComponent<Slider>().value = settingDatas.BGMVolume;
        settingPannel.transform.Find("SoundSetting").Find("BGV").GetComponent<Slider>().value = settingDatas.BGVVolume;
        settingPannel.transform.Find("SoundSetting").Find("SE").GetComponent<Slider>().value = settingDatas.SEVolume;
        settingPannel.transform.Find("SoundSetting").Find("HSE").GetComponent<Slider>().value = settingDatas.HSEVolume;
        settingPannel.transform.Find("TextSetting").Find("DialogSpeed").GetComponent<Slider>().value = settingDatas.dialogSpeed;
        settingPannel.transform.Find("TextSetting").Find("SkipSpeed").GetComponent<Slider>().value = settingDatas.skipSpeed;
        settingPannel.transform.Find("GraphicSetting").Find("Screen").Find("Dialog").Find("DialogTransparent").GetComponent<Slider>().value = settingDatas.dialogTransparent;
        settingPannel.transform.Find("SoundSetting").Find("FirstVoice").GetComponent<Slider>().value = settingDatas.charactersVolume[0];
        settingPannel.transform.Find("SoundSetting").Find("SecondVoice").GetComponent<Slider>().value = settingDatas.charactersVolume[1];
        settingPannel.transform.Find("SoundSetting").Find("ThirdVoice").GetComponent<Slider>().value = settingDatas.charactersVolume[2];
        settingPannel.transform.Find("SoundSetting").Find("FourthVoice").GetComponent<Slider>().value = settingDatas.charactersVolume[3];
    }

    public void ExitSettingPannel()
    {
        settingDatas.BGMVolume = ChapterController._instance.bgmAudioSource.volume;
        settingDatas.BGVVolume = ChapterController._instance.bgvAudioSource.volume;
        settingDatas.SEVolume = ChapterController._instance.seAudioSource.volume;
        settingDatas.HSEVolume = ChapterController._instance.hseAudioSource.volume;
        settingDatas.dialogSpeed = 0.35f - ChapterController._instance.dialogSpeed;
        settingDatas.skipSpeed = 0.35f - ChapterController._instance.skipSpeed;
        settingDatas.noClothes = ChapterController._instance.noClothes;
        settingDatas.dialogTransparent = ChapterController._instance.lineContainer.GetComponent<Image>().color.a;
        settingDatas.isContinuePlayCV = ChapterController._instance.isContinuePlayCV;
        settingDatas.rightFunction = ChapterController._instance.rightFunction;
        settingDatas.isSkipReadedContext = ChapterController._instance.isSkipUnread;
        settingDatas.isSkipUntilHScene = ChapterController._instance.isSkipUntilHScene;
        settingDatas.isSkipUntilShoot = ChapterController._instance.isSkipUntilShoot;
        settingDatas.shootNumber = ChapterController._instance.shootNumber;
        settingDatas.shootChoices = ChapterController._instance.shootChoice;
        settingDatas.chapterIndex = ChapterController._instance.maxChapterIndex;
        settingDatas.cgSavedData = CGArrayToString();
        //SaveDatas<SettingModel>(settingDataFile, settingDatas);

        HideContainer(settingPannel);
        //如果设置了果体，并在章节内，重新Load一下角色图片以立即更新效果
        if (ChapterController._instance.chapterMode)
        {
            ChapterController._instance.LoadRolePic(ChapterController._instance.dialogIndex - 1, false);//每次显示完，Index+1，所以在这里需要-1
            //如果设置了文本颜色，这里也及时改一下
            ChapterController._instance.ChangeReadedTextColor(settingDatas.isChangeReadedTextColor);

        }
    }

    public void ShowMemoryPanel()
    {
        memoryPanel.SetActive(true);
        int showNum = settingDatas.memoryIndex;
        int totalNum = 2;
        for (int i = 1; i <= totalNum; i++)
        {
            Button memoryBtn = memoryPanel.transform.Find(string.Format("Memory{0}", i)).GetComponent<Button>();
            if (i <= showNum)
            {
                memoryBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Format("Image/Mode/Memory/Memory{0}", i));
                memoryBtn.enabled = true;
            }
            else
            {
                memoryBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Mode/CG/cgno");
                memoryBtn.enabled = false;
            }
        }
    }

    public void ExieMemoryPanel()
    {
        memoryPanel.SetActive(false);
    }

    public void GoMemoryIndex(int index)
    {
        //ChapterController._instance.LoadXlsFile(1);//默认读取正章
        ChapterController._instance.chapterIndex = 1;
        memoryPanel.SetActive(false);
        extraContainer.SetActive(false);
        bgmSource.Stop();
        ChapterController._instance.dialogIndex = memoryStartArray[index];
        ChapterController._instance.memoryEnd = memoryEndArray[index];
        ChapterController._instance.memoryMode = true;
        ChapterController._instance.GetNextDialog();
    }

    public void GameEnd()
    {
        Application.Quit();
    }

    public void ChangeScreen(int flag)
    {
        switch (flag)
        {
            case 0:
                Screen.SetResolution(1920, 1080, true);
                break;
            case 1:
                Screen.SetResolution(1024, 576, false);
                break;
            case 2:
                Screen.SetResolution(1024, 576, false);
                break;
            case 3:
                Screen.SetResolution(1280, 720, false);
                break;
                

        }

    }

    public void ChangeSettingTab(int tabNum)
    {
        switch (tabNum)
        {
            case 1:
                settingPannel.transform.Find("GraphicSetting").gameObject.SetActive(true);
                settingPannel.transform.Find("SoundSetting").gameObject.SetActive(false);
                settingPannel.transform.Find("TextSetting").gameObject.SetActive(false);
                settingPannel.transform.Find("GameplaySetting").gameObject.SetActive(false);
                break;
            case 2:
                settingPannel.transform.Find("GraphicSetting").gameObject.SetActive(false);
                settingPannel.transform.Find("SoundSetting").gameObject.SetActive(true);
                settingPannel.transform.Find("TextSetting").gameObject.SetActive(false);
                settingPannel.transform.Find("GameplaySetting").gameObject.SetActive(false);
                break;
            case 3:
                settingPannel.transform.Find("GraphicSetting").gameObject.SetActive(false);
                settingPannel.transform.Find("SoundSetting").gameObject.SetActive(false);
                settingPannel.transform.Find("TextSetting").gameObject.SetActive(true);
                settingPannel.transform.Find("GameplaySetting").gameObject.SetActive(false);
                break;
            case 4:
                settingPannel.transform.Find("GraphicSetting").gameObject.SetActive(false);
                settingPannel.transform.Find("SoundSetting").gameObject.SetActive(false);
                settingPannel.transform.Find("TextSetting").gameObject.SetActive(false);
                settingPannel.transform.Find("GameplaySetting").gameObject.SetActive(true);
                break;
        }
    }

    public void ChangeReadedTextColour(bool flag)
    {
        settingDatas.isChangeReadedTextColor = flag;
        ChapterController._instance.ChangeReadedTextColor(flag);
    }

    public void SetRightControl(int flag)
    {
        ChapterController._instance.rightFunction = flag;
    }

    public void SetShootNumber(int num)
    {
        ChapterController._instance.shootNumber = num;
    }

    public void SetShootChoices(int choice)
    {
        ChapterController._instance.shootChoice = choice;
    }

    public void HideContainer(GameObject container)
    {
        container.SetActive(false);
        if (ChapterController._instance.chapterContainer.activeInHierarchy)
        {
            ChapterController._instance.chapterMode = true;
        }
        if (!titleContainer.activeInHierarchy)
        {
            bgmSource.Stop();
        }
    }

    private void HideAllContainer()
    {
        logo.gameObject.SetActive(false);
        savedDataPanel.SetActive(false);
        extraContainer.SetActive(false);
        cgPanel.SetActive(false);
        cgDetail.SetActive(false);
        bgmPanel.SetActive(false);
        confirmPanel.SetActive(false);
        settingPannel.SetActive(false);
        memoryPanel.SetActive(false);

    }
}
