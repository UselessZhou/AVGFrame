using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Asserts.Scripts.Model;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.UI;
using System;

public class ChapterController : MonoBehaviour
{

    public static ChapterController _instance;
    public GameObject background;
    public GameObject rolesContainer;
    public GameObject lineContainer;
    public GameObject audioContainer;
    public GameObject optionPanel;

    private static string rootPath; //根目录
    private static string linePath; //剧本目录
    private static string lineFile; //剧本文件名

   // private TextAsset mainScenarioTA;
    private string[][] dialogArray; //剧本的二维数组
    private int dialogIndex;        //剧本的索引

    private Text line;          //对话
    private Text roleName;      //角色名称

    private GameObject rightRole;   //右侧角色
    private GameObject centerRole;   //中间角色
    private GameObject leftRole;   //左侧角色

    private GameObject leftRolePic;
    private GameObject centerRolePic;
    private GameObject rightRolePic;

    private bool showNextDialog; //显示下一个对话

    private AudioClip cvAudio;
    public AudioSource cvAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        line = lineContainer.transform.Find("Line").GetComponent<Text>();
        roleName = lineContainer.transform.Find("RoleName").GetComponent<Text>();
        showNextDialog = true;
        rightRole = rolesContainer.transform.Find("RightRole").gameObject;
        centerRole = rolesContainer.transform.Find("CenterRole").gameObject;
        leftRole = rolesContainer.transform.Find("LeftRole").gameObject;

        leftRolePic = leftRole.transform.Find("LeftRolePic").gameObject;
        rightRolePic = rightRole.transform.Find("RightRolePic").gameObject;
        centerRolePic = centerRole.transform.Find("CenterRolePic").gameObject;

        LoadCSVFile();
    }

    private void Awake()
    {
        dialogIndex = 1;
        rootPath = Application.dataPath;
        linePath = rootPath + "/Scripts/Line/";
        lineFile = "line.txt";

    }

    // Update is called once per frame
    void Update()
    {
        //if (showNextDialog)
        //{
        //    GetNextDialog();
        //}
    }

    //private List<SceneAction> LoadSceneAction()
    //{

    //    if (!Directory.Exists(linePath))
    //    {
    //        Directory.CreateDirectory(linePath);
    //    }
    //    //使用using可在结束后销毁using括号内生成的资源变量
    //    using (FileStream fs = new FileStream(linePath + lineFile, FileMode.OpenOrCreate)) //文件不存在直接创建
    //    {
    //        //读取文件内的所以内容，转换为SavedDataModel
    //        using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
    //        {
    //            string lineDataJson = sr.ReadToEnd();
    //            if (null == sceneActions) sceneActions = new List<SceneAction>();
    //            if (!string.IsNullOrEmpty(lineDataJson))
    //            {
    //                JArray lineDataArray = JArray.Parse(lineDataJson);
    //                foreach (JToken jLinedData in lineDataArray)    //JToken为JObject的基类，用来遍历较好
    //                {
    //                    if (null != jLinedData && jLinedData.Type != JTokenType.Null)
    //                    {
    //                        SceneAction lineData = JsonConvert.DeserializeObject<SceneAction>(jLinedData.ToString());//反序列化Json
    //                        sceneActions.Add(lineData);
    //                    }
    //                }
    //                Debug.Log(sceneActions[0].sceneIndex);
    //                return sceneActions;
    //            }

    //        }
    //    }
    //    return new List<SceneAction>();
    //}

    private void LoadCSVFile()
    {
        TextAsset lineTextAsset = (TextAsset)Resources.Load("Line/dialog") as TextAsset;
        string[] lineArray = lineTextAsset.text.Split(new string[]{"\r\n"}, StringSplitOptions.None);
        dialogArray = new string[lineArray.Length][];
        for (int i = 0; i < lineArray.Length; i++)
        {
            dialogArray[i] = lineArray[i].Split(',');
        }
    }

    public void GetNextDialog()
    {
        ScreenCapture.CaptureScreenshot(Application.streamingAssetsPath + "/ScreenShot.png");

        if (dialogArray[dialogIndex].Length == 8 && dialogIndex < dialogArray.Length)
        {
            string dialogType = dialogArray[dialogIndex][1];
            if (dialogType.Equals("Dialog"))
            {
                SetDialogDetail();
            }else if (dialogType.Equals("Option"))
            {
                SetOptionMenu();
            }
            

            dialogIndex++;
        }
    }

    //显示下一个对话的具体内容
    private void SetDialogDetail()
    {
        lineContainer.SetActive(true);
        optionPanel.SetActive(false);
        string dialogContext = dialogArray[dialogIndex][2];
        string dialogScene = dialogArray[dialogIndex][3];
        string dialogRole = dialogArray[dialogIndex][4];
        string dialogAudio = dialogArray[dialogIndex][7];

        string[] rolePicArray = dialogArray[dialogIndex][5].Split('/');
        string[] rolePosArray = dialogArray[dialogIndex][6].Split('/');

        line.text = dialogContext;
        background.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/ChapterBG/" + dialogScene);
        background.SetActive(true);
        roleName.text = dialogRole;
        leftRolePic.SetActive(false);
        centerRolePic.SetActive(false);
        rightRolePic.SetActive(false);
        for (int i = 0; i < rolePosArray.Length; i++)
        {
            switch (rolePosArray[i])
            {
                case "Left":
                    leftRolePic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Role/" + rolePicArray[i]);
                    leftRolePic.SetActive(true);
                    break;
                case "Center":
                    centerRolePic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Role/" + rolePicArray[i]);
                    centerRolePic.SetActive(true);
                    break;
                case "Right":
                    rightRolePic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Role/" + rolePicArray[i]);
                    rightRolePic.SetActive(true);
                    break;

                default:
                    break;
            }
        }
        AudioClip voice = (AudioClip)Resources.Load("Audio/" + dialogAudio);
        cvAudioSource.clip = voice;
        cvAudioSource.Play();
    }

    //显示游戏选项层
    private void SetOptionMenu()
    {
        //TODO:由于选项的个数并不固定，应该需要动态生成Button
        lineContainer.SetActive(false);
        optionPanel.SetActive(true);
        string[] optionContexts = dialogArray[dialogIndex][2].Split('/');
        string[] slipIndex = dialogArray[dialogIndex][3].Split('/');
        GameObject optionMenu = optionPanel.transform.Find("OptionMenu").gameObject;
        Button BtnOne = optionMenu.transform.Find("OptionOne").GetComponent<Button>();
        Button BtnTwo = optionMenu.transform.Find("OptionTwo").GetComponent<Button>();
        BtnOne.transform.Find("Text").GetComponent<Text>().text = optionContexts[0];
        BtnTwo.transform.Find("Text").GetComponent<Text>().text = optionContexts[1];

        BtnOne.onClick.AddListener(delegate ()
        {
            dialogIndex = Convert.ToInt32(slipIndex[0]);
            SetDialogDetail();
        });

        BtnTwo.onClick.AddListener(delegate ()
        {
            dialogIndex = Convert.ToInt32(slipIndex[1]);
            SetDialogDetail();
        });
    }

    public void SkipToAssignDialog()
    {

    }

    /// <summary>
    /// Save时截取屏幕图片
    /// 需要注意的是从屏幕左下角（0，0）开始截取
    /// 需要return WaitForEndOfFrame，否则报错
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public void CaptureScreen(Rect rect)
    {
        //yield return new WaitForSeconds(0.1F);
        //yield return new WaitForEndOfFrame();
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = Application.dataPath + "/ScreenShot.png";
        System.IO.File.WriteAllBytes(filename, bytes);
    }
}
