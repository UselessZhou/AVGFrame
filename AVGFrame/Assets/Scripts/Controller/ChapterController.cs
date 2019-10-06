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
using Excel;
using System.Data;

public class ChapterController : MonoBehaviour
{
    public static ChapterController _instance;
    
    public GameObject background;
    public GameObject rolesContainer;
    public GameObject lineContainer;
    public GameObject audioContainer;
    public GameObject optionPanel;

    private static string rootPath; //根目录
    private static string chapterPath; //剧本目录
    private static string chapterFile; //剧本文件名

   // private TextAsset mainScenarioTA;
    private string[][] dialogArray; //剧本的二维数组
    public int dialogIndex;        //剧本的索引

    private Text line;          //对话
    private string lineText;    //具体内容
    private Text roleName;      //角色名称

    private GameObject rightRole;   //右侧角色
    private GameObject centerRole;   //中间角色
    private GameObject leftRole;   //左侧角色

    private GameObject leftRolePic;
    private GameObject centerRolePic;
    private GameObject rightRolePic;

    public string screenPicName;
    public bool isSavedData;

    //private AudioClip cvAudio;
    public AudioSource cvAudioSource;

    //private AudioClip bgvAudio;
    public AudioSource bgvAudioSource;
    public AudioSource bgmAudioSource;

    private char[] ca;
    private bool showLineTexting;

    private bool isAutoPlay;    //是否自动播放

    private bool isSkipDialog;  //是否快速跳过对话
    private bool isSkipUnread;  //是否未读文本也跳过
    public float skipSpeed;     //skip文本速度

    public float dialogSpeed;  //文本显示速度

    public bool memoryMode;     //memory模式
    public int memoryEnd;       //记录memory结束点

    public int chapterIndex;    //章节号

    public bool chapterMode;    //进入章节模式，可用一些ctrl之类的快捷键

    public GameObject endPanel;

    public Image blackMask;          //转场遮罩层
    private bool isTransition;
    private float timeVal;      //设置转场时间

    //镜头晃动（没有效果）
    public Transform cameraTransform;
    private Vector3 originalPos;
    private float shake = 0f;
    private Vector3 deltaPos = Vector3.zero;

    //人物晃动
    private bool isMoveAction;
    private bool isMoveFinish;
    private Image moveRoleImage;
    public Vector3 moveRoleInitPos;

    public bool noClothes;


    // Start is called before the first frame update
    void Start()
    {
        line = lineContainer.transform.Find("Line").GetComponent<Text>();
        roleName = lineContainer.transform.Find("RoleName").GetComponent<Text>();
        rightRole = rolesContainer.transform.Find("RightRole").gameObject;
        centerRole = rolesContainer.transform.Find("CenterRole").gameObject;
        leftRole = rolesContainer.transform.Find("LeftRole").gameObject;

        leftRolePic = leftRole.transform.Find("LeftRolePic").gameObject;
        rightRolePic = rightRole.transform.Find("RightRolePic").gameObject;
        centerRolePic = centerRole.transform.Find("CenterRolePic").gameObject;

        screenPicName = SetScreenPicName();

    }

    private void Awake()
    {
        _instance = this;
        dialogIndex = 1;
        rootPath = Application.dataPath;
        chapterPath = rootPath + "/StreamingAssets/";
        chapterFile = "chapter.xlsx";
        dialogSpeed = 0.05f;
        skipSpeed = 0.05f;
        memoryMode = false;
        //cameraTransform = GetComponent(typeof(Transform)) as Transform;
        originalPos = cameraTransform.localPosition;
        chapterIndex = 0;
        LoadXlsFile(chapterIndex);//首先进入序章

    }

    // Update is called once per frame
    void Update()
    {
        if (isAutoPlay)
        {
            if(!cvAudioSource.isPlaying && !showLineTexting)
            AutoPlay();
        }

        if (chapterMode && Input.GetKeyDown(KeyCode.LeftControl))
        {
            Debug.Log("按下");
            isSkipDialog = false;
            SetSkipValue();
        }
        if (chapterMode && Input.GetKeyUp(KeyCode.LeftControl))
        {
            Debug.Log("弹起");
            isSkipDialog = true;
            SetSkipValue();
        }
        if (isTransition)
        {
            if(timeVal >= 0.5f)
            {
                isTransition = false;
                timeVal = 0;
                GetNextDialog();
            }
            else
            {
                timeVal += Time.deltaTime;
            }
        }
        if (isMoveAction && !isMoveFinish)
        {
            moveRoleImage.transform.Translate(-moveRoleImage.transform.right * 100 * 0.1f);
            if (Mathf.Abs(moveRoleInitPos.x - moveRoleImage.transform.position.x) > 60)
            {
                isMoveFinish = true;
            }
        }
        if (isMoveAction && isMoveFinish)
        {
            moveRoleImage.transform.Translate(moveRoleImage.transform.right * 100 * 0.1f);
            if (Mathf.Abs(moveRoleInitPos.x - moveRoleImage.transform.position.x) < 20)
            {
                moveRoleImage.transform.position = moveRoleInitPos;
                Debug.Log(moveRoleInitPos);
                isMoveAction = false;
                isMoveFinish = false;
            }
        }

        //if(shake > 0)
        //{
        //    transform.localPosition -= deltaPos;

        //    deltaPos = UnityEngine.Random.insideUnitSphere * 30f;

        //    transform.localPosition += deltaPos;
        //    shake -= Time.deltaTime * 1.0f; ;
        //}
        //else
        //{
        //    shake = 0f;
        //}
    }

    //弃用，CSV无法一个文件以sheet来分章节
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

    //按照章节读取剧本
    public void LoadXlsFile(int chapterNum)
    {
        FileStream fileStream = File.Open(chapterPath + chapterFile, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
        DataSet result = excelDataReader.AsDataSet();
        
        int columns = result.Tables[chapterNum].Columns.Count;
        int rows = result.Tables[chapterNum].Rows.Count;

        dialogArray = new string[rows][];

        for (int i = 0; i < rows; i++)
        {
            string[] value = new string[columns];
            for (int j = 0; j < columns; j++)
            {
                value[j] = result.Tables[chapterNum].Rows[i][j].ToString();
            }
            dialogArray[i] = value;
        }
        chapterIndex = chapterNum;
    }

    public void GetNextDialog()
    {
        if(memoryMode && dialogIndex > memoryEnd)
        {
            rolesContainer.SetActive(false);
            lineContainer.SetActive(false);
            GameController._instance.memoryPanel.SetActive(true);
            memoryMode = false;
            chapterMode = false;
            bgvAudioSource.Stop();
            cvAudioSource.Stop();
            return;

        }
        //如果正在协程显示文字，直接关闭协程。
        if (showLineTexting)
        {
            StopCoroutine("ShowLineCoroutine");
            showLineTexting = false;
            line.text = lineText;
            return;
        }
        //暂时强制让BGV听完再能显示下一个dialog
        //if (bgvAudioSource.isPlaying)
        //{
        //    isAutoPlay = false;
        //    isSkipDialog = false;
        //    return;
        //}

        cvAudioSource.Stop();

        if (dialogIndex < dialogArray.Length && dialogArray[dialogIndex].Length == 12)
        {
            string dialogType = dialogArray[dialogIndex][1];
            if (dialogType.Equals("Animation"))
            {
                shake = 3f;
                return;
            }

            GameController._instance.hideContainers();
            rolesContainer.SetActive(true);
            isSavedData = true;
            chapterMode = true;
            if (dialogType.Equals("End"))//故事结束
            {
                endPanel.SetActive(true);
            }
            else if (dialogType.Equals("Dialog") || dialogType.Equals("Aside"))
            {
                SetDialogDetail();
            }else if (dialogType.Equals("Option"))
            {
                isAutoPlay = false;
                isSkipDialog = false;
                SetOptionMenu();
            }else if (dialogType.Equals("BGV"))
            {
                PlayBGVAudio();
            }else if (dialogType.Equals("Transition"))
            {
                //目前只有一种转场特效，不做判断
                isTransition = true;
            }

            string cgIndex = dialogArray[dialogIndex][10];
            string memoryIndex = dialogArray[dialogIndex][11];
            if (!cgIndex.Equals(""))
            {
                int indexNum = int.Parse(cgIndex);
                if(indexNum > GameController._instance.settingDatas.cgIndex)
                {
                    GameController._instance.settingDatas.cgIndex = indexNum;
                    GameController._instance.SaveSettingDatas();
                }
            }
            if (!memoryIndex.Equals(""))
            {
                int indexNum = int.Parse(memoryIndex);
                if (indexNum > GameController._instance.settingDatas.memoryIndex)
                {
                    GameController._instance.settingDatas.memoryIndex = indexNum;
                    GameController._instance.SaveSettingDatas();
                }
            }
            dialogIndex++;
            
        }
        else if(dialogIndex >= dialogArray.Length && chapterIndex == 0)//序章完成，直接进入第一章节
        {
            LoadXlsFile(1);
            dialogIndex = 1;
            GetNextDialog();
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
        string bgmAudio = dialogArray[dialogIndex][9];

        lineText = dialogContext;
        ShowLine();
        background.transform.Find("bg").GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/ChapterBG/" + dialogScene);
        background.SetActive(true);
        roleName.text = dialogRole;
        leftRolePic.SetActive(false);
        centerRolePic.SetActive(false);
        rightRolePic.SetActive(false);
        bgvAudioSource.Stop();

        if (!bgmAudio.Equals("") && ! bgmAudio.Equals("off"))
        {
            AudioClip bgm = (AudioClip)Resources.Load("BGM/" + bgmAudio);
            bgmAudioSource.clip = bgm;
            bgmAudioSource.Play();
        }else if(bgmAudio.Equals("off"))
        {
            bgmAudioSource.Stop();
        }

        if (dialogArray[dialogIndex][1].Equals("Dialog"))
        {
            LoadRolePic(dialogIndex);
            AudioClip voice = (AudioClip)Resources.Load("Audio/" + dialogAudio);
            cvAudioSource.clip = voice;
            cvAudioSource.Play();
        }
        

        //CaptureScreen();//由于在saveBtn上做截屏的调用只能截取save页面的屏幕，暂时就每次加载dialog的时候都截取一张。
    }

    //显示游戏选项层
    private void SetOptionMenu()
    {
        //TODO:由于选项的个数并不固定，应该需要动态生成Button
        lineContainer.SetActive(false);
        rolesContainer.SetActive(false);
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
            dialogIndex++;
        });

        BtnTwo.onClick.AddListener(delegate ()
        {
            dialogIndex = Convert.ToInt32(slipIndex[1]);
            SetDialogDetail();
            dialogIndex++;
        });
    }

    private void PlayBGVAudio()
    {
        string bgvAudio = dialogArray[dialogIndex][9];
        string dialogContext = dialogArray[dialogIndex][2];
        lineText = dialogContext;
        ShowLine();
        isAutoPlay = false;
        isSkipDialog = false;
        if (!bgvAudio.Equals(""))//当之前已在播放BGV并没有到结束的时刻，默认继续循环播放
        {
            AudioClip bgv = (AudioClip)Resources.Load("BGV/" + bgvAudio);
            bgvAudioSource.clip = bgv;
            bgvAudioSource.Play();
        }
        rolesContainer.SetActive(false);
        //line.text = "";
        roleName.text = "";
        lineContainer.SetActive(true);
        string cg = dialogArray[dialogIndex][3];
        background.transform.Find("bg").GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Mode/CG/" + cg);
        background.SetActive(true);
    }

    /// <summary>
    /// Save时截取屏幕图片
    /// 需要注意的是从屏幕左下角（0，0）开始截取
    /// 需要return WaitForEndOfFrame，否则报错
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public void CaptureScreen()
    {
        //yield return new WaitForSeconds(0.1F);
        //yield return new WaitForEndOfFrame();
        //Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        //screenShot.ReadPixels(rect, 0, 0);
        //screenShot.Apply();
        //byte[] bytes = screenShot.EncodeToPNG();
        //string filename = Application.dataPath + "/ScreenShot.png";
        //System.IO.File.WriteAllBytes(filename, bytes);


        //ScreenCapture.CaptureScreenshot(rootPath + "/Resources/SavedData/" + screenPicName + ".png");//截取屏幕快照，以作存档图片
    }

    public SavedDataModel GetCurrentData()
    {
        SavedDataModel tmpData = new SavedDataModel();
        tmpData.sceneIndex = dialogIndex - 1;
        tmpData.savedTime = DateTime.Now;
        return tmpData;
    }

    public string SetScreenPicName()
    {
        DirectoryInfo folder = new DirectoryInfo(rootPath + "/Resources/SavedData/");
        bool foundPic;
        for(int i=0; i<=10; i++)
        {
            foundPic = false;
            string tmpPicName = string.Format("savedImage{0}", i);
            foreach (FileInfo file in folder.GetFiles("*.png"))
            {
                if (file.Name == tmpPicName + ".png")
                {
                    foundPic = true;
                    break;
                }
            }
            if (!foundPic) return tmpPicName;
        }
        return null;//发现所有的命名规则都已被占用，有问题。
    }

    //跳过序章的按钮
    public void SkipChapter()
    {
        LoadXlsFile(1);
        dialogIndex = 1;
        GetNextDialog();
    }

    //不跳过序章的按钮
    public void NoSkipChapter()
    {
        LoadXlsFile(0);
        dialogIndex = 1;
        GetNextDialog();
    }

    //协程逐字显示对话文本
    public void ShowLine()
    {
        StopCoroutine("ShowLineCoroutine");
        StartCoroutine("ShowLineCoroutine");
    }
    IEnumerator ShowLineCoroutine()
    {
        ca = lineText.ToCharArray();
        showLineTexting = true;
        string tempText = "";
        for (int i = 0; i < ca.Length; i++)
        {

            tempText += ca[i];
            line.text = tempText;
            yield return new WaitForSeconds(dialogSpeed);

        }
        showLineTexting = false;
    }

    //自动播放
    private void AutoPlay()
    {
        GetNextDialog();
    }

    public void SetAutoPlayValue()
    {
        isAutoPlay = !isAutoPlay;
    }

    public void SetSkipValue()
    {
        isSkipDialog = !isSkipDialog;
        if (isSkipDialog)
        {
            StartCoroutine("SkipDialogs");
        }
        else
        {
            StopCoroutine("SkipDialogs");
        }
    }


    //Skip快速跳过文本
    IEnumerator SkipDialogs()
    {
        while (isSkipDialog)
        {
            //跳过未读文本，直接一直读取下一句
            if (isSkipUnread)
            {
                GetNextDialog();
                yield return new WaitForSeconds(skipSpeed);
            }
            else//不跳过未读文本，需要将settingDatas下存储的最新Index作比较
            {
                if((chapterIndex < GameController._instance.settingDatas.chapterIndex) ||
                   ((chapterIndex == GameController._instance.settingDatas.chapterIndex) && (dialogIndex < GameController._instance.settingDatas.maxDialogIndex)))
                {
                    GetNextDialog();
                    yield return new WaitForSeconds(skipSpeed);
                }
                else
                {
                    isSkipDialog = false;
                }
            }
        }
    }

    public void ShowCGMode()
    {

    }

    public void ReturnMenu()
    {
        rolesContainer.SetActive(false);
        lineContainer.SetActive(false);
        background.SetActive(false);
        endPanel.SetActive(false);
        isSkipDialog = false;
        isAutoPlay = false;
        bgmAudioSource.Stop();
        bgvAudioSource.Stop();
        cvAudioSource.Stop();
        GameController._instance.titleContainer.SetActive(true);
    }

    public void DarkTransition()
    {
        blackMask.color -= new Color(0, 0, 0, Mathf.Lerp(1, 0, 0.2f));
        if (blackMask.color.a <= 0.2f)
        {
            blackMask.color = new Color(0, 0, 0, 0);
            isTransition = false;
        }
    }

    public void ShowClothes(bool clothes)
    {
        noClothes = clothes;
    }

    public void SkipUnReadText(bool isSkip)
    {
        isSkipUnread = isSkip;
    }

    public void LoadRolePic(int index)
    {

        string[] rolePicArray = dialogArray[index][5].Split('/');
        string[] rolePosArray = dialogArray[index][6].Split('/');
        string[] actionArray = dialogArray[index][8].Split('/');
        for (int i = 0; i < rolePosArray.Length; i++)
        {
            switch (rolePosArray[i])
            {
                case "left":
                    if (noClothes)
                    {
                        leftRolePic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Role/" + rolePicArray[i] + "_no");
                    }
                    else
                    {
                        leftRolePic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Role/" + rolePicArray[i]);
                    }
                    leftRolePic.SetActive(true);
                    if (actionArray.Length > i && actionArray[i].Equals("Move"))
                    {
                        moveRoleImage = leftRolePic.GetComponent<Image>();
                        moveRoleInitPos = leftRolePic.transform.position;
                        isMoveAction = true;
                    }

                    break;
                case "center":
                    if (noClothes)
                    {
                        centerRolePic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Role/" + rolePicArray[i] + "_no");
                    }
                    else
                    {
                        centerRolePic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Role/" + rolePicArray[i]);
                    }
                    centerRolePic.SetActive(true);
                    if (actionArray.Length > i && actionArray[i].Equals("Move"))
                    {
                        moveRoleImage = centerRolePic.GetComponent<Image>();
                        moveRoleInitPos = centerRolePic.transform.position;
                        isMoveAction = true;
                    }
                    break;
                case "right":
                    if (noClothes)
                    {
                        rightRolePic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Role/" + rolePicArray[i] + "_no");
                    }
                    else
                    {
                        rightRolePic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Role/" + rolePicArray[i]);
                    }
                    rightRolePic.SetActive(true);
                    if (actionArray.Length > i && actionArray[i].Equals("Move"))
                    {
                        moveRoleImage = rightRolePic.GetComponent<Image>();
                        moveRoleInitPos = rightRolePic.transform.position;
                        isMoveAction = true;
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
