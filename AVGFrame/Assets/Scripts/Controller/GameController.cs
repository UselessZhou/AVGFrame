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
        titleContainer = displayCanvas.transform.Find("TitleContainer").gameObject;
        savedDataPanel = displayCanvas.transform.Find("SavedDataPanel").gameObject;

        LoadSavedDatas();
        LoadQuickSaveData();
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
        ChapterController._instance.lineContainer.SetActive(true);
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
}
