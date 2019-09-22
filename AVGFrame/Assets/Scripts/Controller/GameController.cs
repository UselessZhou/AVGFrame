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
    private GameObject titleContainer;

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
    private SettingModel settingDatas;
    private static string settingDataFile;

    private GameObject extraContainer;

    //CGMode
    private GameObject cgPanel;
    private GameObject cgDetail;
    private Button fullCGBtn;
    private Button zeroCGBtn;

    private GameObject bgmPanel;

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

        bgmPanel = displayCanvas.transform.Find("BGMPanel").gameObject;

        settingPannel = displayCanvas.transform.Find("SettingPanel").gameObject;
        LoadSavedDatas();
        LoadQuickSaveData();
        LoadSettingDatas();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        //如果未做过存档，删除截屏时保留的png
        if (!ChapterController._instance.isSavedData)
        {
            if (File.Exists(savedDatasPath + ChapterController._instance.screenPicName + ".png"))
            {
                File.Delete(savedDatasPath + ChapterController._instance.screenPicName + ".png");
            }
        }
        ExitSettingPannel();
    }

    public void GoToTitle()
    {
        logo.gameObject.SetActive(false);
        //bg.gameObject.SetActive(true);
        titleContainer.SetActive(true);
    }

    public void NewGame()
    {
        titleContainer.SetActive(false);
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
                ChapterController._instance.dialogIndex = sceneIndex;
                ChapterController._instance.GetNextDialog();
            }
        }
        else
        {
            //这串逻辑为当按下存档按钮时的操作，由于之前截屏图片通过改名后关联到Btn不能立即显示
            if(null != savedDatas[savedDataIndex])
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
            SaveDatas(savedDataIndex);

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

    public void ClosePanel()
    {
        savedDataPanel.SetActive(false);
    }

    public void LinkBtn()
    {
        Application.OpenURL("www.baidu.com");
    }


    public void ShowExtraPanel()
    {
        extraContainer.SetActive(true);
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
        SaveSettingDatas();
    }

    public void ShowZeroCG()
    {
        settingDatas.cgIndex = 0;
        SaveSettingDatas();
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
        settingPannel.SetActive(true);
        settingPannel.transform.Find("BGM").GetComponent<Slider>().value = settingDatas.BGMVolume;
        settingPannel.transform.Find("Voice").GetComponent<Slider>().value = settingDatas.VoiceVolume;
        settingPannel.transform.Find("DialogSpeed").GetComponent<Slider>().value = settingDatas.dialogSpeed;
    }

    public void ExitSettingPannel()
    {
        settingPannel.SetActive(false);
        settingDatas.BGMVolume = ChapterController._instance.bgvAudioSource.volume;
        settingDatas.VoiceVolume = ChapterController._instance.cvAudioSource.volume;
        settingDatas.dialogSpeed = 0.35f - ChapterController._instance.dialogSpeed;
        SaveSettingDatas();
    }
}
