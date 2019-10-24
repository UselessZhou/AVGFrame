using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using Asserts.Scripts.Model;
using Newtonsoft.Json;

public class GameController : MonoBehaviour
{
    public static GameController _instance;

    private static string rootPath; //根目录
    private static string savedDatasPath; //剧本目录
    private static string savedDatasFile; //剧本文件名

    //UI
    public GameObject displayCanvas; //获取displayCanvas
    public GameObject logo; //Logo页面显示
    public Image bg;   //Title背景图片
    public GameObject titleContainer;
    public AudioSource titleAudioSource;

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

    public GameObject memoryPanel;
    private int[] memoryStartArray;
    private int[] memoryEndArray;

    private GameObject settingPannel;

    private void Awake()
    {
        _instance = this;
        logo.gameObject.SetActive(true);
    }

    private void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
        rootPath = Application.dataPath;
        savedDatasPath = rootPath + "/Resources/SavedData/";
        savedDatasFile = "savedData.json";
        qSavedDataFile = "quickSavedData.json";
        settingDataFile = "settingData.json";
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

        memoryPanel = displayCanvas.transform.Find("MemoryPanel").gameObject;
        memoryStartArray = new int[] { 28, 44 };
        memoryEndArray = new int[] { 42, 65 };

        settingPannel = displayCanvas.transform.Find("SettingPanel").gameObject;
        LoadSavedDatas();
        LoadQuickSaveData();
        LoadSettingDatas();
        ChapterController._instance.noClothes = settingDatas.noClothes;//先要读取是否着装的设置
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        //如果未做过存档，删除截屏时保留的png
        //if (!ChapterController._instance.isSavedData)
        //{
        //    if (File.Exists(savedDatasPath + ChapterController._instance.screenPicName + ".png"))
        //    {
        //        File.Delete(savedDatasPath + ChapterController._instance.screenPicName + ".png");
        //    }
        //}
        ExitSettingPannel();
    }

    public void GoToTitle()
    {
        logo.gameObject.SetActive(false);
        //bg.gameObject.SetActive(true);
        titleContainer.SetActive(true);
        //AudioClip bgv = (AudioClip)Resources.Load("BGM/bgmTitle");
        //titleAudioSource.clip = bgv;
        //titleAudioSource.Play();
    }

    public void NewGame()
    {
        titleContainer.SetActive(false);
        titleAudioSource.Stop();
        logo.SetActive(false);
        //ChapterController._instance.lineContainer.SetActive(true);
        skipContainer.SetActive(true);
    }

    /// <summary>
    /// 从文件中获取存档的信息
    /// </summary>
    /// <returns></returns>
    private List<SavedDataModel> LoadSavedDatas()
    {
        if (!Directory.Exists(savedDatasPath))
        {
            Directory.CreateDirectory(savedDatasPath);
        }
        //使用using可在结束后销毁using括号内生成的资源变量
        using (FileStream fs = new FileStream(savedDatasPath + savedDatasFile, FileMode.OpenOrCreate)) //文件不存在直接创建
        {
            //读取文件内的所以内容，转换为SavedDataModel
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                string savedDataJson = sr.ReadToEnd();
                if (null == savedDatas) savedDatas = InitList<SavedDataModel>(10);
                if (!string.IsNullOrEmpty(savedDataJson))
                {
                    JArray savedDataArray = JArray.Parse(savedDataJson);
                    foreach (JToken jSavedData in savedDataArray)    //JToken为JObject的基类，用来遍历较好
                    {
                        if (null != jSavedData && jSavedData.Type != JTokenType.Null)
                        {
                            SavedDataModel savedData = JsonConvert.DeserializeObject<SavedDataModel>(jSavedData.ToString());//反序列化Json
                            savedDatas[savedData.savedDataIndex] = savedData;
                        }
                    }
                    Debug.Log(savedDatas[0].ToString());
                    return savedDatas;
                }

            }
        }
        return savedDatas = InitList<SavedDataModel>(10);
    }

    //快存文件的读取,先直接复制黏贴， 之后应该抽象一下
    private SavedDataModel LoadQuickSaveData()
    {
        if (!Directory.Exists(savedDatasPath))
        {
            Directory.CreateDirectory(savedDatasPath);
        }
        //使用using可在结束后销毁using括号内生成的资源变量
        using (FileStream fs = new FileStream(savedDatasPath + qSavedDataFile, FileMode.OpenOrCreate)) //文件不存在直接创建
        {
            //读取文件内的所以内容，转换为SavedDataModel
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                string savedDataJson = sr.ReadToEnd();
                if (null == qSavedData) qSavedData = new SavedDataModel();
                if (!string.IsNullOrEmpty(savedDataJson))
                {
                    JToken jSavedData = JToken.Parse(savedDataJson);
                        if (null != jSavedData && jSavedData.Type != JTokenType.Null)
                        {
                            SavedDataModel savedData = JsonConvert.DeserializeObject<SavedDataModel>(jSavedData.ToString());//反序列化Json
                            qSavedData = savedData;
                        }
                    return qSavedData;
                }

            }
        }
        return qSavedData = new SavedDataModel();
    }

    //配置文件的读取,先直接复制黏贴， 之后应该抽象一下
    private SavedDataModel LoadSettingDatas()
    {
        if (!Directory.Exists(savedDatasPath))
        {
            Directory.CreateDirectory(savedDatasPath);
        }
        //使用using可在结束后销毁using括号内生成的资源变量
        using (FileStream fs = new FileStream(savedDatasPath + settingDataFile, FileMode.OpenOrCreate)) //文件不存在直接创建
        {
            //读取文件内的所以内容，转换为SavedDataModel
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                string settingDataJson = sr.ReadToEnd();
                if (null == settingDatas) settingDatas = new SettingModel();
                if (!string.IsNullOrEmpty(settingDataJson))
                {
                    JToken jSettingData = JToken.Parse(settingDataJson);
                    if (null != jSettingData && jSettingData.Type != JTokenType.Null)
                    {
                        SettingModel settingData = JsonConvert.DeserializeObject<SettingModel>(jSettingData.ToString());//反序列化Json
                        settingDatas = settingData;
                    }
                    return qSavedData;
                }

            }
        }
        return qSavedData = new SavedDataModel();
    }

    /// <summary>
    /// 将存储数据存入本地
    /// </summary>
    public void SaveSettingDatas()
    {
        if (!Directory.Exists(savedDatasPath))
        {
            Directory.CreateDirectory(savedDatasPath);
        }
        using (StreamWriter w = new StreamWriter(savedDatasPath + settingDataFile, false, Encoding.UTF8))
        {
            string settingDataJson = JsonConvert.SerializeObject(settingDatas);
            w.Write(settingDataJson);
        }
    }

    public List<T> InitList<T>(int size)
    {
        List<T> newList = new List<T>();
        if (size > 0)
        {
            for (int i = 0; i < size; i++)
            {
                newList.Add(default(T));
            }
        }
        return newList;
    }

    public void ShowSavedDataPanel(bool isLoadBtn)
    {
        titleAudioSource.Stop();
        isLoadPanel = isLoadBtn;

        //隐藏一些页面，之后应该做成方法
        //hideContainers();

        savedDataPanel.SetActive(true);

        for(int i=0; i<savedDatas.Count; i++)
        {
            Button savedBtn = savedDataPanel.transform.Find(string.Format("SavedField{0}", i)).GetComponent<Button>();
            if (null != savedDatas[i])
            {
                //savedBtn.transform.Find("SavedPic").GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Format("SavedData/{0}", savedDatas[i].savedPicName));
                savedBtn.transform.Find("Date").GetComponent<Text>().text = savedDatas[i].savedTime.ToString();
                savedBtn.transform.Find("SavedPic").GetComponent<Image>().sprite = Resources.Load<Sprite>("SavedData/noData");
            }
            else
            {
                savedBtn.transform.Find("SavedPic").GetComponent<Image>().sprite = Resources.Load<Sprite>("SavedData/noData");
                savedBtn.transform.Find("Date").GetComponent<Text>().text = "----/--/-- --:--:--";
            }
        }
    }

    /// <summary>
    /// Load界面大部分与SAVE相同，先直接用SAVE页面，但是需要一个标志位来确定S/L
    /// </summary>
    public void ShowLoadDataPanel()
    {
        ShowSavedDataPanel(true);
    }

    public void LoadAndShowDialog(int savedDataIndex)
    {
        if (isLoadPanel)
        {
            if (null != savedDatas[savedDataIndex])
            {
                int sceneIndex = savedDatas[savedDataIndex].sceneIndex;
                int chapterIndex = savedDatas[savedDataIndex].chapterIndex;
                ChapterController._instance.bgmAudioSource.Stop();
                ChapterController._instance.bgvAudioSource.Stop();
                ChapterController._instance.LoadXlsFile(chapterIndex);
                ChapterController._instance.dialogIndex = sceneIndex;
                ChapterController._instance.GetNextDialog();
            }
        }
        else
        {
            //这串逻辑为当按下存档按钮时的操作，由于之前截屏图片通过改名后关联到Btn不能立即显示
            if (null != savedDatas[savedDataIndex])
            {
                string tmpPicName = savedDatas[savedDataIndex].savedPicName;
                if (File.Exists(savedDatasPath + tmpPicName))
                {
                    File.Delete(savedDatasPath + tmpPicName);
                }
                if (File.Exists(savedDatasPath + tmpPicName + ".meta"))
                {
                    File.Delete(savedDatasPath + tmpPicName + ".meta");
                }
            }
            savedDatas[savedDataIndex] = ChapterController._instance.GetCurrentData();
            savedDatas[savedDataIndex].savedDataIndex = savedDataIndex;
            savedDatas[savedDataIndex].savedPicName = ChapterController._instance.screenPicName;
            savedDatas[savedDataIndex].chapterIndex = ChapterController._instance.chapterIndex;
            SaveDatas(savedDataIndex);
            //在save的时候存储最新的文本序列号。1.章节号大于setting，2.章节相同，dialogIndex大于setting,已在getnextdialog中做
            //if ((ChapterController._instance.chapterIndex > settingDatas.chapterIndex) || 
            //    ((ChapterController._instance.chapterIndex == settingDatas.chapterIndex) && (ChapterController._instance.dialogIndex > settingDatas.maxDialogIndex)))
            //{
            //    settingDatas.chapterIndex = ChapterController._instance.chapterIndex;
            //    settingDatas.maxDialogIndex = ChapterController._instance.dialogIndex;
            //    SaveSettingDatas();
            //}
        }
    }

    public void hideContainers()
    {
        //TODO:也许可以根据传进来的参数，动态隐藏其他的Containers
        titleContainer.SetActive(false);
        savedDataPanel.SetActive(false);

        ChapterController._instance.background.SetActive(false);
        ChapterController._instance.rolesContainer.SetActive(false);
        ChapterController._instance.lineContainer.SetActive(false);
        ChapterController._instance.optionPanel.SetActive(false);
        ChapterController._instance.background.SetActive(false);
    }

    /// <summary>
    /// 将存储数据存入本地
    /// </summary>
    public void SaveDatas(int savedDataIndex)
    {
        if (!Directory.Exists(savedDatasPath))
        {
            Directory.CreateDirectory(savedDatasPath);
        }
        using (StreamWriter w = new StreamWriter(savedDatasPath + savedDatasFile, false, Encoding.UTF8))
        {
            // TODO: This convertion will fail as the SavedDataModel contains
            //savedDatas[savedDataIndex].sceneIndex = ChapterController._instance.dialogIndex - 1;
            string savedDataJson = JsonConvert.SerializeObject(savedDatas);
            Debug.Log(savedDataJson);
            w.Write(savedDataJson);
        }

        //将截屏的Png更名为可被savedata使用的格式
        //if (File.Exists(linePath + "ScreenShot.png"))
        //{
        //    if(File.Exists(linePath + string.Format("savedImage{0}.png", savedDataIndex)))
        //    {
        //        File.Delete(string.Format(linePath + "savedImage{0}.png", savedDataIndex));
        //        string.Format(linePath + "savedImage{0}.png.meta", savedDataIndex);
        //    }
        //    File.Move(linePath + "ScreenShot.png", string.Format(linePath + "savedImage{0}.png", savedDataIndex));
        //}

        //重新加载页面以显示新的存档
        savedDataPanel.SetActive(false);
        ChapterController._instance.isSavedData = true;
        ChapterController._instance.screenPicName = ChapterController._instance.SetScreenPicName();
    }

    public void SetQuickSavedData()
    {
        if (!Directory.Exists(savedDatasPath))
        {
            Directory.CreateDirectory(savedDatasPath);
        }
        using (StreamWriter w = new StreamWriter(savedDatasPath + qSavedDataFile, false, Encoding.UTF8))
        {
            // TODO: This convertion will fail as the SavedDataModel contains
            //qSavedData.sceneIndex = ChapterController._instance.dialogIndex - 1;
            //qSavedData.savedTime = time
            qSavedData = ChapterController._instance.GetCurrentData();
            string savedDataJson = JsonConvert.SerializeObject(qSavedData);
            w.Write(savedDataJson);
        }
    }

    public void LoadQuickSavedData()
    {
        if(null != qSavedData)
        {
            int sceneIndex = qSavedData.sceneIndex;
            ChapterController._instance.dialogIndex = sceneIndex;
            ChapterController._instance.GetNextDialog();
        }
    }

    public void CloseSavedDataPanel()
    {
        savedDataPanel.SetActive(false);
    }

    public void LinkBtn()
    {
        Application.OpenURL("www.baidu.com");
    }


    public void ShowExtraPanel()
    {
        titleAudioSource.Stop();
        extraContainer.SetActive(true);
    }

    public void ExitExtraContainerBtn()
    {
        extraContainer.SetActive(false);
    }

    public void ShowCGPanel()
    {
        cgPanel.SetActive(true);
        int showNum = settingDatas.cgIndex;
        int totalNum = 9;
        //int showNum = 5;
        for (int i = 1; i <= totalNum; i++)
        {
            Button cgBtn = cgPanel.transform.Find(string.Format("CG{0}", i)).GetComponent<Button>();
            if (i <= showNum)
            {
                cgBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Format("Image/Mode/CG/ev{0}", i));
                cgBtn.onClick.AddListener(delegate ()
                {
                    cgDetail.SetActive(true);
                    Button detailBtn = cgDetail.transform.Find("Detail").GetComponent<Button>();
                    detailBtn.GetComponent<Image>().sprite = cgBtn.GetComponent<Image>().sprite;
                });
            }
            else
            {
                cgBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Mode/CG/cgno");
            }
        }
    }

    public void CloseCGDetail()
    {
        cgDetail.SetActive(false);
    }

    public void ShowFullCG()
    {
        settingDatas.cgIndex = 9;
        settingDatas.memoryIndex = 2;
        SaveSettingDatas();
    }

    public void ShowZeroCG()
    {
        settingDatas.cgIndex = 0;
        settingDatas.memoryIndex = 0;
        SaveSettingDatas();
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

    public void ExitBGMPanelBtn()
    {
        bgmPanel.SetActive(false);
    }

    public void PlayBGMBtn(string bgmName)
    {
        AudioSource bgmSource = bgmPanel.transform.Find("BGM").GetComponent<AudioSource>();
        AudioClip bgm = (AudioClip)Resources.Load("BGM/" + bgmName);
        bgmSource.clip = bgm;
        bgmSource.Play();
    }

    public void StopBGMBtn()
    {
        AudioSource bgmSource = bgmPanel.transform.Find("BGM").GetComponent<AudioSource>();
        bgmSource.Stop();
    }

    public void ShowSettingPanel()
    {
        titleAudioSource.Stop();
        settingPannel.SetActive(true);
        ChangeSettingTab(1);
        settingPannel.transform.Find("ComminSetting").Find("BGM").GetComponent<Slider>().value = settingDatas.BGMVolume;
        settingPannel.transform.Find("ComminSetting").Find("BGV").GetComponent<Slider>().value = settingDatas.BGVVolume;
        settingPannel.transform.Find("ComminSetting").Find("Voice").GetComponent<Slider>().value = settingDatas.VoiceVolume;
        settingPannel.transform.Find("ComminSetting").Find("DialogSpeed").GetComponent<Slider>().value = settingDatas.dialogSpeed;
        settingPannel.transform.Find("ComminSetting").Find("SkipSpeed").GetComponent<Slider>().value = settingDatas.skipSpeed;
        settingPannel.transform.Find("ComminSetting").Find("Screen").Find("Dialog").Find("DialogTransparent").GetComponent<Slider>().value = settingDatas.dialogTransparent;
        settingPannel.transform.Find("CharactersCVSetting").Find("FirstVoice").GetComponent<Slider>().value = settingDatas.charactersVolume[0];
        settingPannel.transform.Find("CharactersCVSetting").Find("SecondVoice").GetComponent<Slider>().value = settingDatas.charactersVolume[1];
        settingPannel.transform.Find("CharactersCVSetting").Find("ThirdVoice").GetComponent<Slider>().value = settingDatas.charactersVolume[2];
        settingPannel.transform.Find("CharactersCVSetting").Find("FourthVoice").GetComponent<Slider>().value = settingDatas.charactersVolume[3];
    }

    public void ExitSettingPannel()
    {
        settingPannel.SetActive(false);
        settingDatas.BGMVolume = ChapterController._instance.bgmAudioSource.volume;
        settingDatas.BGVVolume = ChapterController._instance.bgvAudioSource.volume;
        settingDatas.VoiceVolume = ChapterController._instance.cvAudioSource.volume;
        settingDatas.dialogSpeed = 0.35f - ChapterController._instance.dialogSpeed;
        settingDatas.skipSpeed = 0.35f - ChapterController._instance.skipSpeed;
        settingDatas.noClothes = ChapterController._instance.noClothes;
        settingDatas.dialogTransparent = ChapterController._instance.lineContainer.GetComponent<Image>().color.a;
        settingDatas.isContinuePlayCV = ChapterController._instance.isContinuePlayCV;
        SaveSettingDatas();

        //如果设置了果体，并在章节内，重新Load一下角色图片以立即更新效果
        if (ChapterController._instance.chapterMode)
        {
            ChapterController._instance.LoadRolePic(ChapterController._instance.dialogIndex - 1);//每次显示完，Index+1，所以在这里需要-1
            //如果设置了文本颜色，这里也及时改一下
            if (settingDatas.isChangeReadedTextColor)
            {
                ChapterController._instance.ChangeReadedTextColor(true);
            }
            else
            {
                ChapterController._instance.ChangeReadedTextColor(false);
            }
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
            }
            else
            {
                memoryBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Mode/CG/cgno");
            }
        }
    }

    public void ExieMemoryPanel()
    {
        memoryPanel.SetActive(false);
    }

    public void GoMemoryIndex(int index)
    {
        ChapterController._instance.LoadXlsFile(1);//默认读取正章
        ChapterController._instance.chapterIndex = 1;
        memoryPanel.SetActive(false);
        extraContainer.SetActive(false);
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
                settingPannel.transform.Find("ComminSetting").gameObject.SetActive(true);
                settingPannel.transform.Find("CharactersCVSetting").gameObject.SetActive(false);
                break;
            case 2:
                settingPannel.transform.Find("ComminSetting").gameObject.SetActive(false);
                settingPannel.transform.Find("CharactersCVSetting").gameObject.SetActive(true);
                break;
        }
    }

    public void ChangeReadedTextColour(bool flag)
    {
        settingDatas.isChangeReadedTextColor = flag;
        ChapterController._instance.ChangeReadedTextColor(flag);
    }
}
