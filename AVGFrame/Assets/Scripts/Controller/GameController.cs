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
    private static string linePath; //剧本目录
    private static string lineFile; //剧本文件名

    //UI
    public GameObject displayCanvas; //获取displayCanvas
    public Image logo; //Logo页面显示
    public Image bg;   //Title背景图片
    private GameObject titleContainer;

    //save页面
    private GameObject savedDataPanel;
    private List<SavedDataModel> savedDatas;

    private void Awake()
    {
        logo.gameObject.SetActive(true);
    }

    private void Start()
    {
        rootPath = Application.dataPath;
        linePath = rootPath + "/Resources/SavedData/";
        lineFile = "savedData.json";
        titleContainer = displayCanvas.transform.Find("TitleContainer").gameObject;
        savedDataPanel = displayCanvas.transform.Find("SavedDataPanel").gameObject;

        LoadSavedDatas();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToTitle()
    {
        logo.gameObject.SetActive(false);
        //bg.gameObject.SetActive(true);
        titleContainer.SetActive(true);
    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// 从文件中获取存档的信息
    /// </summary>
    /// <returns></returns>
    private List<SavedDataModel> LoadSavedDatas()
    {
        if (!Directory.Exists(linePath))
        {
            Directory.CreateDirectory(linePath);
        }
        //使用using可在结束后销毁using括号内生成的资源变量
        using (FileStream fs = new FileStream(linePath + lineFile, FileMode.OpenOrCreate)) //文件不存在直接创建
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
                    Debug.Log(savedDatas[0]);
                    return savedDatas;
                }

            }
        }
        return savedDatas = InitList<SavedDataModel>(10);
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

    public void ShowSavedDataPanel()
    {
        savedDataPanel.SetActive(true);

        for(int i=0; i<savedDatas.Count; i++)
        {
            Button savedBtn = savedDataPanel.transform.Find(string.Format("SavedField{0}", i)).GetComponent<Button>();
            if (null != savedDatas[i])
            {
                savedBtn.transform.Find("SavedPic").GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Format("SavedData/savedImage{0}", i));
                savedBtn.transform.Find("Date").GetComponent<Text>().text = savedDatas[i].savedTime.ToString();
            }
            else
            {
                savedBtn.transform.Find("SavedPic").GetComponent<Image>().sprite = Resources.Load<Sprite>("SavedData/noData");
                savedBtn.transform.Find("Date").GetComponent<Text>().text = "----/--/-- --:--:--";
            }
        }
    }
}
