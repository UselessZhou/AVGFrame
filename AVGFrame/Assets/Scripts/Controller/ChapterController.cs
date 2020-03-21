using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Asserts.Scripts.Model;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
using UnityEngine.UI;
using System;
using System.Data;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ChapterController : MonoBehaviour
{
    public static ChapterController _instance;

    public GameObject chapterContainer;
    public GameObject lineContainer;
    private GameObject background;
    private GameObject rolesContainer;
    private GameObject audioContainer;
    private GameObject optionPanel;
    private GameObject topMenu;
    private GameObject shootChoiceContainer;

    private static string rootPath; //根目录
    private static string chapterPath; //剧本目录
    private static string chapterFile; //剧本文件名

   // private TextAsset mainScenarioTA;
    private string[][] dialogArray; //剧本的二维数组
    public int dialogIndex;        //剧本的索引

    public Text line;          //对话
    private string lineText;    //具体内容
    public Text roleName;      //角色名称

    private GameObject rightRole;   //右侧角色
    private GameObject centerRole;   //中间角色
    private GameObject leftRole;   //左侧角色

    private GameObject leftRolePic;
    private GameObject centerRolePic;
    private GameObject rightRolePic;

    public string screenPicName;    //背景图片，用于显示和存储在S/L页面
    //public bool isSavedData;何用？？？？

    //private AudioClip cvAudio;
    public AudioSource cvAudioSource;
    public bool isContinuePlayCV;

    //private AudioClip bgvAudio;
    public AudioSource bgvAudioSource;
    public AudioSource bgmAudioSource;
    public AudioSource seAudioSource;
    public AudioSource hseAudioSource;

    private char[] ca;
    private bool showLineTexting;

    private bool isAutoPlay;    //是否自动播放

    private bool isSkipDialog;  //是否快速跳过对话
    public bool isSkipUnread;  //是否未读文本也跳过
    public float skipSpeed;     //skip文本速度
    public bool isSkipUntilHScene;
    public bool isSkipUntilShoot;
    private bool isChooseMode;//选择模式
    Image shootNumImage;    //shoot倒计时图片

    public float dialogSpeed;  //文本显示速度

    public bool memoryMode;     //memory模式
    public int memoryEnd;       //记录memory结束点

    public int chapterIndex;    //章节号
    public int maxChapterIndex; //最大章节

    public bool chapterMode;    //进入章节模式，可用一些ctrl之类的快捷键

    private GameObject endPanel;

    private Image transitionPic;          //转场图片
    private bool isTransition;
    private float timeVal;      //设置转场时间

    //镜头晃动（没有效果）
    public Camera mainCamera;
    private float shakeDelta = 0.005f;
    public float shakeLevel = 3f;
    private Vector3 originalPos;
    private float shake = 0f;
    private Vector3 deltaPos = Vector3.zero;
    private bool animationAction;

    //人物晃动
    private bool isMoveAction;
    private bool isMoveFinish;
    //private Image moveRoleImage;
    private GameObject moveRoleImage;
    public Vector3 moveRoleInitPos;

    public bool noClothes;

    public bool isShowTopMenu;  //显示顶部Menu

    private Button voiceBtn;    //底部重新播放CV的btn

    public GameObject reviewDialogPanel;
    public GameObject dialogLayout;     //回看日志框架
    public GameObject dialogInstance;   //回看预制体

    public bool backDialogMode;     //滚轮回放

    public int tempMaxIndex;        //暂存最大DialogIndex

    public bool isChangeReadedTextColor;    //是否更改已读文本颜色

    public int rightFunction;   //右键功能

    public int shootNumber;     //sj倒数行数

    public int shootChoice;     //sj选择

    private string tempCGPic;   //根据sj选择，显示的CG

    public GameObject displayCanvas;

    // Start is called before the first frame update
    void Start()
    {
        chapterContainer = displayCanvas.transform.Find("ChapterContainer").gameObject;
        background = chapterContainer.transform.Find("Background").gameObject;
        rolesContainer = chapterContainer.transform.Find("RolesContainer").gameObject;
        lineContainer = chapterContainer.transform.Find("LineContainer").gameObject;
        optionPanel = chapterContainer.transform.Find("OptionPanel").gameObject;
        topMenu = chapterContainer.transform.Find("TopMenu").gameObject;
        endPanel = chapterContainer.transform.Find("EndPanel").gameObject;
        transitionPic = chapterContainer.transform.Find("TransitionPic").GetComponent<Image>();
        shootChoiceContainer = chapterContainer.transform.Find("ShootChoiceContainer").gameObject;
        shootNumImage = background.transform.Find("shootNum").GetComponent<Image>();


        line = lineContainer.transform.Find("Line").GetComponent<Text>();
        roleName = lineContainer.transform.Find("RoleName").GetComponent<Text>();
        rightRole = rolesContainer.transform.Find("RightRole").gameObject;
        centerRole = rolesContainer.transform.Find("CenterRole").gameObject;
        leftRole = rolesContainer.transform.Find("LeftRole").gameObject;

        leftRolePic = leftRole.transform.Find("LeftRolePic").gameObject;
        rightRolePic = rightRole.transform.Find("RightRolePic").gameObject;
        centerRolePic = centerRole.transform.Find("CenterRolePic").gameObject;
        voiceBtn = lineContainer.transform.Find("BottomMenu").gameObject.transform.Find("VoiceBtn").GetComponent<Button>();
        //screenPicName = SetScreenPicName();
        //isContinuePlayCV = GameController._instance.settingDatas.isContinuePlayCV;
        isChangeReadedTextColor = GameController._instance.settingDatas.isChangeReadedTextColor;
        rightFunction = GameController._instance.settingDatas.rightFunction;
        isSkipUntilHScene = GameController._instance.settingDatas.isSkipUntilHScene;
        isSkipUnread = GameController._instance.settingDatas.isSkipReadedContext;
        isSkipUntilShoot = GameController._instance.settingDatas.isSkipUntilShoot;
        shootNumber = GameController._instance.settingDatas.shootNumber;
        shootChoice = GameController._instance.settingDatas.shootChoices;
        maxChapterIndex = GameController._instance.settingDatas.chapterIndex;
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
        mainCamera = GetComponent<Camera>();
        chapterIndex = 0;
        //LoadXlsFile(chapterIndex);//首先进入序章

    }

    // Update is called once per frame
    void Update()
    {
        if (isAutoPlay)
        {
            if(!cvAudioSource.isPlaying && !showLineTexting)
            AutoPlay();
        }

        if (chapterMode && !isChooseMode)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Debug.Log("按下");
                isSkipDialog = false;
                SetSkipValue();
            }
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                Debug.Log("弹起");
                isSkipDialog = true;
                SetSkipValue();
            }

            //空格，单机显示LineContainer
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!lineContainer.activeInHierarchy)
                {
                    ShowLineContainer();
                }
                else
                {
                    lineContainer.SetActive(false);
                }

            }

            //鼠标左键功能
            if(Input.GetMouseButtonDown(0) && !isTransition)
            {
                GameObject hitUIObject = null;

                if (EventSystem.current.IsPointerOverGameObject())
                {
                    hitUIObject = GetMouseOverUIObject(displayCanvas);
                    Debug.Log("---- EventSystem.current.IsPointerOverGameObject ----");
                }
                if(null == hitUIObject || hitUIObject.tag.Trim() != "OperationButton")
                {
                    if (!lineContainer.activeInHierarchy && !reviewDialogPanel.activeInHierarchy)
                    {
                        ShowLineContainer();
                    }
                    else if (lineContainer.activeInHierarchy && !backDialogMode)
                    {
                        GetNextDialog();
                    }
                    else if (backDialogMode)
                    {
                        if (tempMaxIndex > dialogIndex)
                        {
                            GetBackDialog(false);
                        }
                        else
                        {
                            backDialogMode = false;
                            GetNextDialog();
                        }
                    }
                }
                
            }

            //鼠标右键功能
            if (Input.GetMouseButtonDown(1) && !isTransition)
            {
                switch (rightFunction)
                {
                    case 1://隐藏对话框
                        if (!lineContainer.activeInHierarchy)
                        {
                            ShowLineContainer();
                        }
                        else
                        {
                            lineContainer.SetActive(false);
                        }
                        break;
                    case 2://重复声音
                        if (voiceBtn.IsActive())
                        {
                            cvAudioSource.Play();
                        }
                        break;
                    case 3://调出Menu
                        if (!GameController._instance.settingPannel.activeInHierarchy)
                        {
                            GameController._instance.ShowSettingPanel();
                        }
                        else
                        {
                            GameController._instance.ExitSettingPannel();
                        }
                        break;
                    case 4://调出Backlog
                        if (!reviewDialogPanel.activeInHierarchy)
                        {
                            ReviewDialog();
                        }
                        else
                        {
                            ExitReviewPanel();
                        }
                        break;
                }

            }

            //滚轮重播之前的剧本
            if (!reviewDialogPanel.activeInHierarchy && Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (tempMaxIndex > dialogIndex)
                {
                    GetBackDialog(false);
                }
                else
                {
                    backDialogMode = false;
                    GetNextDialog();
                }
            }
            if (!reviewDialogPanel.activeInHierarchy && Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                backDialogMode = true;
                GetBackDialog(true);
            }

            //镜头晃动
            if (animationAction)
            {
                if (shake > 0)
                {
                    shake -= Time.deltaTime * 1.0f;
                    mainCamera.rect = new Rect(shakeDelta * (-1.0f + shakeLevel * UnityEngine.Random.value),
                                    shakeDelta * (-1.0f + shakeLevel * UnityEngine.Random.value), 1.0f, 1.0f);
                }
                else
                {
                    shake = 0f;
                    mainCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
                    animationAction = false;
                }
            }
            //设置转场
            if (isTransition)
            {
                if (timeVal >= 0.5f)
                {
                    isTransition = false;
                    timeVal = 0;
                    transitionPic.gameObject.SetActive(false);
                    GetNextDialog();
                }
                else
                {
                    timeVal += Time.deltaTime;
                }
            }
        }
        else
        {
            animationAction = false;
            isSkipDialog = false;
            isAutoPlay = false;
            isTransition = false;
        }
        
        if (isMoveAction && !isMoveFinish)
        {
            //moveRoleImage.transform.Translate(-moveRoleImage.transform.right * 1 * 0.1f);
            moveRoleImage.transform.position += Vector3.right * 10 * Time.deltaTime;
            if (Mathf.Abs(moveRoleInitPos.x - moveRoleImage.transform.position.x) > 1)
            {
                isMoveFinish = true;
            }
        }
        if (isMoveAction && isMoveFinish)
        {
            moveRoleImage.transform.position -= Vector3.right * 10 * Time.deltaTime;
            //moveRoleImage.transform.Translate(moveRoleImage.transform.right * 1 * 0.1f);
            if (Mathf.Abs(moveRoleInitPos.x - moveRoleImage.transform.position.x) < 1)
            {
                moveRoleImage.transform.position = moveRoleInitPos;
                Debug.Log(moveRoleInitPos);
                isMoveAction = false;
                isMoveFinish = false;
            }
        }
        
    }

    void FixedUpdate()
    {
        if (chapterMode)
        {
            //动态显示顶部Menu,条件：1.鼠标移向窗口顶部。2.必须是lineContainer显示的状态。3.非滚轮回滚模式
            //Debug.Log("Mouse: " +  Input.mousePosition.y + " Height: " + Screen.height + " Menu: " + topMenu.GetComponent<RectTransform>().sizeDelta.y);
            if (Input.mousePosition.y > Screen.height - topMenu.GetComponent<RectTransform>().sizeDelta.y && lineContainer.activeInHierarchy && !backDialogMode)
            {
                ShowTopMenu();
            }
            else
            {
                topMenu.SetActive(false);
            }
        }
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
    /*
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
    }*/

    public void GetNextDialog()
    {
        //显示对话场景
        chapterContainer.SetActive(true);
        //退出回忆模式
        if (memoryMode && dialogIndex > memoryEnd)
        {
            ExitMemoryMode();
            return;
        }
        //如果正在播放动画，则停止
        if (animationAction)
        {
            shake = -1.0f;
        }

        //更新最大已读序列与最大chapter
        if (chapterIndex >= maxChapterIndex)
        {
            maxChapterIndex = chapterIndex;
            if(dialogIndex > GameController._instance.settingDatas.maxDialogIndex)
            {
                GameController._instance.settingDatas.maxDialogIndex = dialogIndex;
            }

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

        //先将voice按钮隐藏，待类型是dialog再显示
        voiceBtn.gameObject.SetActive(false);
        //根据设定判断是否需要中断CV
        if (!isContinuePlayCV)
        {
            cvAudioSource.Stop();
        }
        
        //判断目前的dialog是否小于剧本的最大长度 以及剧本列数是否正确
        if (dialogIndex < dialogArray.Length && dialogArray[dialogIndex].Length == 15)
        {
            chapterMode = true;
            tempMaxIndex = dialogIndex;
            string dialogType = dialogArray[dialogIndex][1];
            if (dialogType.Equals("Animation"))
            {
                animationAction = true;
                shake = 2f;
                dialogIndex++;
                return;
            }
            //当选择skip至H，并且剧本标识此index开始为h，将skip功能停止
            if(isSkipUntilHScene && null != dialogArray[dialogIndex][13] && dialogArray[dialogIndex][13].Equals("on"))
            {
                isSkipDialog = false;
            }

            //根据配置的shoot数在场景中显示
            if(!dialogArray[dialogIndex][14].Equals("") && int.Parse(dialogArray[dialogIndex][14]) <= shootNumber)
            {
                //当选择skip至shoot，停止shoot
                if (isSkipUntilShoot && int.Parse(dialogArray[dialogIndex][14]) == shootNumber)
                {
                    isSkipDialog = false;
                }
                shootNumImage.sprite = Resources.Load<Sprite>("Image/ChapterBG/shoot" + dialogArray[dialogIndex][14]);
                shootNumImage.gameObject.SetActive(true);

                //如果num=1，并且需要选择内外射，显示选择框
                if(int.Parse(dialogArray[dialogIndex][14]) == 1 && shootChoice == 3)
                {
                    //选择框
                    //shootChoiceContainer.SetActive(true);
                    isChooseMode = true;
                    isAutoPlay = false;
                    isSkipDialog = false;
                    shootChoiceContainer.SetActive(true);
                }
            }
            else
            {
                shootNumImage.gameObject.SetActive(false);
            }

            if (dialogType.Equals("End"))//故事结束
            {
                //chapterContainer.SetActive(false);
                chapterMode = false;
                endPanel.SetActive(true);
            }
            else if (dialogType.Equals("Dialog") || dialogType.Equals("Aside") || dialogType.Equals("CG"))
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
                transitionPic.sprite = Resources.Load<Sprite>("Image/ChapterBG/loading");
                transitionPic.gameObject.SetActive(true);
            }
            //判断是否解锁memory，当剧本中的Index>setting内的值，更新memory值
            string memoryIndex = dialogArray[dialogIndex][11];
            if (!memoryIndex.Equals(""))
            {
                int indexNum = int.Parse(memoryIndex);
                if (indexNum > GameController._instance.settingDatas.memoryIndex)
                {
                    GameController._instance.settingDatas.memoryIndex = indexNum;
                    //GameController._instance.SaveSettingDatas();
                }
            }
            dialogIndex++;
            
        }
        else if(dialogIndex >= dialogArray.Length && chapterIndex == 0)//序章完成，直接进入第一章节
        {
            //LoadXlsFile(1);
            dialogIndex = 1;
            GetNextDialog();
        }
    }

    //显示下一个对话的具体内容
    private void SetDialogDetail()
    {
        ShowLineContainer();
        string dialogContext = dialogArray[dialogIndex][2];
        //string dialogScene = dialogArray[dialogIndex][3];
        screenPicName = dialogArray[dialogIndex][3];
        string dialogRole = dialogArray[dialogIndex][4];
        string dialogAudio = dialogArray[dialogIndex][7];
        string bgmAudio = dialogArray[dialogIndex][9];
        string cgIndex = dialogArray[dialogIndex][10];
        
        if (backDialogMode)
        {
            line.color = Color.blue;
            roleName.color = Color.blue;
        }
        else
        {
            //更改已读文本颜色
            ChangeReadedTextColor(isChangeReadedTextColor);
        }

        if (!backDialogMode)
        {
            lineContainer.transform.Find("BottomMenu").gameObject.SetActive(true);
            lineText = dialogContext;
            ShowLine();
        }
        else
        {
            line.text = dialogContext;
        }

        if (dialogArray[dialogIndex][1].Equals("CG"))
        {
            string cgName = "";
            
            if(shootChoice == 1)
            {
                cgName = cgIndex + "_in";
                screenPicName = screenPicName + "_in";
                background.transform.Find("bg").GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/ChapterBG/" + screenPicName);
            }
            else if(shootChoice == 2)
            {
                cgName = cgIndex + "_out";
                screenPicName = screenPicName + "_out";
                background.transform.Find("bg").GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/ChapterBG/" + screenPicName);
            }
            else
            {
                if(null == tempCGPic)//如果出现特殊情况变量为空，都内设处理
                {
                    cgName = cgIndex + "_in";
                    screenPicName = screenPicName + "_in";
                    background.transform.Find("bg").GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/ChapterBG/" + screenPicName);
                }
                else
                {
                    cgName = cgIndex + tempCGPic;
                    screenPicName = screenPicName + tempCGPic;
                    background.transform.Find("bg").GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/ChapterBG/" + screenPicName);
                }
            }

            //将CG加入解锁的CG数组中
            AddCGToArray(cgIndex, cgName);
        }
        else
        {
            background.transform.Find("bg").GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/ChapterBG/" + screenPicName);

            if (!cgIndex.Equals(""))
            {
                string[] arrayTemp = cgIndex.Split('_');
                //将CG加入解锁的CG数组中
                AddCGToArray(arrayTemp[0], cgIndex);
                //int arrayIndex = int.Parse(arrayTemp[0]);
                //if (GameController._instance.cgArray[arrayIndex-1].Length == 1 && GameController._instance.cgArray[arrayIndex-1][0].Equals("0"))
                //{
                //    GameController._instance.cgArray[arrayIndex-1][0] = cgIndex;
                //}
                //else
                //{
                //    for (int i = 0; i < GameController._instance.cgArray[arrayIndex-1].Length; i++)
                //    {
                //        if (GameController._instance.cgArray[arrayIndex-1][i].Equals(cgIndex))
                //        {
                //            break;
                //        }
                //        cgList.Add(GameController._instance.cgArray[arrayIndex-1][i]);
                //        if ((i + 1) == GameController._instance.cgArray[arrayIndex-1].Length)
                //        {
                //            cgList.Add(cgIndex);
                //            GameController._instance.cgArray[arrayIndex-1] = cgList.ToArray();
                //        }
                //    }
                //}
            }
        }
        //background.SetActive(true);
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
            LoadRolePic(dialogIndex, true);
            AudioClip voice = (AudioClip)Resources.Load("Audio/" + dialogAudio);
            //根据角色名字设置不同cv的声音大小
            if (dialogRole.Equals("粉"))
            {
                cvAudioSource.volume = GameController._instance.settingDatas.charactersVolume[0];
            }
            if (dialogRole.Equals("橙"))
            {
                cvAudioSource.volume = GameController._instance.settingDatas.charactersVolume[1];
            }
            if (dialogRole.Equals("蓝"))
            {
                cvAudioSource.volume = GameController._instance.settingDatas.charactersVolume[2];
            }
            if (dialogRole.Equals("男"))
            {
                cvAudioSource.volume = GameController._instance.settingDatas.charactersVolume[3];
            }
            cvAudioSource.clip = voice;
            cvAudioSource.Play();
            voiceBtn.gameObject.SetActive(true);
        }
        
        //CaptureScreen();//由于在saveBtn上做截屏的调用只能截取save页面的屏幕，暂时就每次加载dialog的时候都截取一张。
    }

    public void GetBackDialog(bool flag)
    {
        if (flag)
        {
            if (dialogIndex <= 1)
            {
                return;
            }
            dialogIndex--;
            string tempType = dialogArray[dialogIndex][1];
            if (tempType.Equals("Transition"))
            {
                dialogIndex++;
                return;
            }
        }
        else
        {
            if(dialogIndex >= dialogArray.Length)
            {
                return;
            }
            dialogIndex++;
        }

        //如果正在播放动画，则停止
        if (animationAction)
        {
            shake = -1.0f;
        }

        //如果正在协程显示文字，直接关闭协程。
        if (showLineTexting)
        {
            StopCoroutine("ShowLineCoroutine");
            showLineTexting = false;
            line.text = lineText;
            return;
        }

        voiceBtn.gameObject.SetActive(false);
        cvAudioSource.Stop();
        lineContainer.transform.Find("BottomMenu").gameObject.SetActive(false);
        topMenu.SetActive(false);
        string dialogType = dialogArray[dialogIndex][1];
        if (dialogType.Equals("Animation"))
        {
            animationAction = true;
            shake = 2f;
            return;
        }
        if (dialogType.Equals("Transition"))
        {
            return;
        }
        //GameController._instance.hideContainers();
        //rolesContainer.SetActive(true);
       // isSavedData = true;
        chapterMode = true;
        if (dialogType.Equals("Dialog") || dialogType.Equals("Aside"))
        {
            SetDialogDetail();
        }
        if (dialogType.Equals("BGV"))
        {
            PlayBGVAudio();
        }

    }

    //显示游戏选项层
    private void SetOptionMenu()
    {
        //TODO:由于选项的个数并不固定，应该需要动态生成Button
        optionPanel.SetActive(true);
        isChooseMode = true;
        string[] optionContexts = dialogArray[dialogIndex][2].Split('/');
        string[] slipIndex = dialogArray[dialogIndex][3].Split('/');
        GameObject optionMenu = optionPanel.transform.Find("OptionMenu").gameObject;
        Button BtnOne = optionMenu.transform.Find("OptionOne").GetComponent<Button>();
        Button BtnTwo = optionMenu.transform.Find("OptionTwo").GetComponent<Button>();
        BtnOne.transform.Find("Text").GetComponent<Text>().text = optionContexts[0];
        BtnTwo.transform.Find("Text").GetComponent<Text>().text = optionContexts[1];

        BtnOne.onClick.AddListener(delegate ()
        {
            HideContainer(optionPanel);
            dialogIndex = Convert.ToInt32(slipIndex[0]);
            GetNextDialog();
            //SetDialogDetail();
            //dialogIndex++;
        });

        BtnTwo.onClick.AddListener(delegate ()
        {
            HideContainer(optionPanel);
            dialogIndex = Convert.ToInt32(slipIndex[1]);
            GetNextDialog();
            //SetDialogDetail();
            //dialogIndex++;
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
        ShowLineContainer();
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
        tmpData.dialogInedx = dialogIndex - 1;
        tmpData.savedTime = DateTime.Now;
        tmpData.chapterIndex = chapterIndex;
        tmpData.savedPicName = screenPicName;
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
        //LoadXlsFile(1);
        dialogIndex = 1;
        GetNextDialog();
    }

    //不跳过序章的按钮
    public void NoSkipChapter()
    {
        //LoadXlsFile(0);
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
                if((chapterIndex < maxChapterIndex) ||
                   ((chapterIndex == maxChapterIndex) && (dialogIndex < GameController._instance.settingDatas.maxDialogIndex)))
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
        HideContainer(chapterContainer);
        HideContainer(shootChoiceContainer);
        endPanel.SetActive(false);
        bgmAudioSource.Stop();
        bgvAudioSource.Stop();
        cvAudioSource.Stop();
        chapterMode = false;
        GameController._instance.GoToTitle();
    }

    //public void DarkTransition()
    //{
    //    blackMask.color -= new Color(0, 0, 0, Mathf.Lerp(1, 0, 0.2f));
    //    if (blackMask.color.a <= 0.2f)
    //    {
    //        blackMask.color = new Color(0, 0, 0, 0);
    //        isTransition = false;
    //    }
    //}

    public void ShowClothes(bool clothes)
    {
        noClothes = clothes;
    }

    public void SkipUnReadText(bool isSkip)
    {
        isSkipUnread = isSkip;
    }

    public void SkipUntilHScene(bool isUntil)
    {
        isSkipUntilHScene = isUntil;
    }

    public void SkipUntilShoot(bool isUntil)
    {
        isSkipUntilShoot = isUntil;
    }

    public void LoadRolePic(int index, bool isPlayAction)
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
                    if (actionArray.Length > i && actionArray[i].Equals("Move") && isPlayAction)
                    {
                        moveRoleImage = leftRole;
                        //moveRoleImage = leftRolePic.GetComponent<Image>();
                        moveRoleInitPos = leftRole.transform.position;
                        //moveRoleInitPos = leftRolePic.transform.position;
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
                    if (actionArray.Length > i && actionArray[i].Equals("Move") && isPlayAction)
                    {
                        moveRoleImage = centerRole;
                        moveRoleInitPos = centerRole.transform.position;
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
                    if (actionArray.Length > i && actionArray[i].Equals("Move") && isPlayAction)
                    {
                        moveRoleImage = rightRole;
                        moveRoleInitPos = rightRole.transform.position;
                        isMoveAction = true;
                    }
                    break;

                default:
                    break;
            }
        }
    }

    //ClearBtn的点击事件（如果直接可以设置line的Active，是否就不用写这个方法了）
    public void ClearLineContainer()
    {
        lineContainer.SetActive(false);
    }

    //VoiceBtn的点击事件，重新播放一遍CV
    public void RepeatCVAudio()
    {
        cvAudioSource.Play();
    }

    public void ReviewDialog()
    {
        reviewDialogPanel.SetActive(true);
        lineContainer.SetActive(false);
        topMenu.SetActive(false);
        for (int i = 0; i < dialogLayout.transform.childCount; i++)
        {
            Destroy(dialogLayout.transform.GetChild(i).gameObject);
            //dialogLayout.transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = dialogIndex-1; i>0; i--)
        {
            string dialogType = dialogArray[i][1];
            if (dialogType.Equals("Transition"))//转场前的文本无法回看
            {
                break;
            }
            if(dialogType.Equals("Aside") || dialogType.Equals("Dialog"))
            {
                GameObject tempInstance = Instantiate(dialogInstance, dialogLayout.transform);
                string tempContext = dialogArray[i][2];
                tempInstance.transform.Find("InstanceRect").transform.Find("Text").GetComponent<Text>().text = tempContext;
                if (dialogType.Equals("Dialog"))
                {
                    Text tempName = tempInstance.transform.Find("InstanceRect").transform.Find("Name").GetComponent<Text>();
                    tempName.text = dialogArray[i][4];
                    tempName.gameObject.SetActive(true);
                    Button tempBtn = tempInstance.transform.Find("InstanceRect").transform.Find("VoiceBtn").GetComponent<Button>();
                    tempBtn.transform.Find("Text").GetComponent<Text>().text = i.ToString();
                    tempBtn.gameObject.SetActive(true);
                    int dialogIndex = i;
                    tempBtn.onClick.AddListener(delegate ()
                    {
                        ReplayCVAudioBtn(dialogIndex);
                    });
                }
            }
        }

        reviewDialogPanel.transform.Find("Scrollbar").GetComponent<Scrollbar>().value = 0.01f;
    }

    public void ReplayCVAudioBtn(int index)
    {
        //int index = int.Parse(replayBtn.transform.Find("Text").GetComponent<Text>().text);
        AudioClip voice = (AudioClip)Resources.Load("Audio/" + dialogArray[index][7]);
        cvAudioSource.clip = voice;
        cvAudioSource.Play();
    }

    public void ExitReviewPanel()
    {
        reviewDialogPanel.SetActive(false);
        ShowLineContainer();
        ShowTopMenu();
    }

    public void IsContinuePlayCV(bool value)
    {
        isContinuePlayCV = value;
    }

    public void ChangeReadedTextColor(bool value)
    {
        isChangeReadedTextColor = value;
        if (isChangeReadedTextColor)
        {
            if ((chapterIndex < maxChapterIndex) ||
                   ((chapterIndex == maxChapterIndex) && (dialogIndex < GameController._instance.settingDatas.maxDialogIndex)))
            {
                line.color = Color.yellow;
                roleName.color = Color.yellow;
            }
            else
            {
                line.color = Color.white;
                roleName.color = Color.white;
            }
        }
        else
        {
            line.color = Color.white;
            roleName.color = Color.white;
        }
    }
    
    //选择内外设的逻辑，将要显示的图片名称存放，退出choose模式
    public void SetTempCGPic(string cg)
    {
        tempCGPic = cg;
        isChooseMode = false;
        shootChoiceContainer.SetActive(false);
        GetNextDialog();
    }

    private GameObject GetMouseOverUIObject(GameObject canvas)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(pointerEventData, results);
        if (results.Count > 1)//由于Btn下面还有Text，拿0会拿到Text的Object，要么Btn不带字体（Text点隐藏），要么拿1
        {
            return results[1].gameObject;
        }

        return null;
    }

    private void ShowTopMenu()
    {
        topMenu.SetActive(true);
        if (memoryMode)
        {
            topMenu.transform.Find("SaveBtn").gameObject.SetActive(false);
            topMenu.transform.Find("LoadBtn").gameObject.SetActive(false);
        }
        else
        {
            topMenu.transform.Find("SaveBtn").gameObject.SetActive(true);
            topMenu.transform.Find("LoadBtn").gameObject.SetActive(true);
        }
    }

    private void ShowLineContainer()
    {
        lineContainer.SetActive(true);
        if (memoryMode)
        {
            lineContainer.transform.Find("BottomMenu").gameObject.transform.Find("QSaveBtn").gameObject.SetActive(false);
            lineContainer.transform.Find("BottomMenu").gameObject.transform.Find("QLoadBtn").gameObject.SetActive(false);
            //lineContainer.transform.Find("BottomMenu").gameObject.transform.Find("ReturnBtn").gameObject.SetActive(true);
        }
        else
        {
            lineContainer.transform.Find("BottomMenu").gameObject.transform.Find("QSaveBtn").gameObject.SetActive(true);
            lineContainer.transform.Find("BottomMenu").gameObject.transform.Find("QLoadBtn").gameObject.SetActive(true);
            //lineContainer.transform.Find("BottomMenu").gameObject.transform.Find("ReturnBtn").gameObject.SetActive(false);
        }
    }

    public void ExitMemoryMode()
    {
        //rolesContainer.SetActive(false);
        //lineContainer.SetActive(false);
        chapterContainer.SetActive(false);
        StopCoroutine("ShowLineCoroutine");
        showLineTexting = false;
        GameController._instance.memoryPanel.SetActive(true);
        memoryMode = false;
        chapterMode = false;
        bgvAudioSource.Stop();
        cvAudioSource.Stop();
    }

    //public void ShowChoiceContainer(GameObject container)
    //{
    //    container.SetActive(true);
    //    isChooseMode = true;
    //}

    private void HideContainer(GameObject container)
    {
        container.SetActive(false);
        if(!optionPanel.activeInHierarchy && !shootChoiceContainer.activeInHierarchy)
        {
            isChooseMode = false;
        }
        if (!chapterContainer.activeInHierarchy)
        {
            chapterMode = false;
        }
    }

    //将CG加入解锁的CG数组中
    private void AddCGToArray(string cgIndex, string cgName)
    {
        List<string> cgList = new List<string>();
        int arrayIndex = int.Parse(cgIndex);
        if (GameController._instance.cgArray[arrayIndex - 1].Length == 1 && GameController._instance.cgArray[arrayIndex - 1][0].Equals("0"))
        {
            GameController._instance.cgArray[arrayIndex - 1][0] = cgName;
        }
        else
        {
            for (int i = 0; i < GameController._instance.cgArray[arrayIndex - 1].Length; i++)
            {
                if (GameController._instance.cgArray[arrayIndex - 1][i].Equals(cgName))
                {
                    break;
                }
                cgList.Add(GameController._instance.cgArray[arrayIndex - 1][i]);
                if ((i + 1) == GameController._instance.cgArray[arrayIndex - 1].Length)
                {
                    cgList.Add(cgName);
                    GameController._instance.cgArray[arrayIndex - 1] = cgList.ToArray();
                }
            }
        }
    }
}
