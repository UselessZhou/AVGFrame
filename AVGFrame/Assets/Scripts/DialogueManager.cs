using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    [SerializeField]
    private DialogueState dialogState;
    public SceneManager sceneManager;
    public GameObject chapterContainer;
    public Sheet1[] chapterDatas;
    private Sheet1 curChapterData; //这个是QucikSheet解析的Excel数据

    [Range(0.01f,0.08f)]
    public float typingSpeed = 0.05f;
    private float autoPlaySpeed = 0.6f;
    public int curNum;
    public int chapterIndex;
    public string curDialogContent;
    public string curBGPic;

    [Header("Roles")]
    public GameObject leftRole;
    public GameObject rightRole;
    public GameObject centerRole;

    private GameObject leftRolePic;
    private GameObject rightRolePic;
    private GameObject centerRolePic;
    private GameObject bgPic;

    private Text roleName;
    private Text dialogueContent;

    private GameObject topMenu;
    private bool isTopMenu;
    private bool isAutoPlay;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        curNum = 0;
        chapterIndex = 0;
        dialogState = DialogueState.START;

        GameObject lineContainer = chapterContainer.transform.Find("LineContainer").gameObject;

        roleName = lineContainer.transform.Find("RoleName").gameObject.GetComponent<Text>();
        dialogueContent = lineContainer.transform.Find("Line").gameObject.GetComponent<Text>();

        leftRolePic = leftRole.transform.Find("LeftRolePic").gameObject;
        rightRolePic = rightRole.transform.Find("RightRolePic").gameObject;
        centerRolePic = centerRole.transform.Find("CenterRolePic").gameObject;
        bgPic = chapterContainer.transform.Find("Background").gameObject.transform.Find("bg").gameObject;

        topMenu = chapterContainer.transform.Find("TopMenu").gameObject;

    }

    private void FixedUpdate()
    {
        ShowTopMenu();
        
    }

    void Update()
    {
        switch (dialogState)
        {
            case DialogueState.TYPING:
                break;
            case DialogueState.PAUSED:
                break;
            case DialogueState.CHOICE:
                break;
            default:
                break;
        }

        if (isAutoPlay)
        {
            AutoPlay();
        }
    }

    public void Init()
    {
        AVGController.instance.avgState = AVGState.Chapter;
        isTopMenu = false;
        LoadChapter();
        SetBGPic();
        SetRoles();
        SetContent();
        chapterContainer.SetActive(true);
    }

    public void ShowChapterContainer(bool value)
    {
        if (value)
        {
            Init();
        }
        else
        {
            chapterContainer.SetActive(value);
        }
    }

    private void LoadChapter()
    {
        curChapterData = chapterDatas[chapterIndex];
    }

    private void SetContent()
    {
        dialogState = DialogueState.TYPING;
        roleName.text = curChapterData.dataList[curNum].Name;
        curDialogContent = curChapterData.dataList[curNum].Content;
        StartContentCoroutine(true);
    }

    private void SetBGPic()
    {
        curBGPic = curChapterData.dataList[curNum].Background;
        bgPic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/ChapterBG/" + curBGPic);
    }


    private void SetRoles()
    {
        if (curChapterData.dataList[curNum].Showleft)
        {
            leftRole.SetActive(true);
            leftRolePic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Role/" + curChapterData.dataList[curNum].Leftpic);
        }
        else
        {
            leftRole.SetActive(false);
        }

        if (curChapterData.dataList[curNum].Showright)
        {
            rightRole.SetActive(true);
            rightRolePic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Role/" + curChapterData.dataList[curNum].Rightpic);
        }
        else
        {
            rightRole.SetActive(false);
        }

        if (curChapterData.dataList[curNum].Showcenter)
        {
            centerRole.SetActive(true);
            centerRolePic.GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Role/" + curChapterData.dataList[curNum].Centerpic);
        }
        else
        {
            centerRole.SetActive(false);
        }
    }
    

    public void JumpTargetDialogue(int targetId)
    {
        Sheet1Data tmpData = curChapterData.dataList.Find((Sheet1Data data) => data.Index == targetId);
        curNum = tmpData.Index - 1;
        sceneManager.ShowChoicePanel(false);
        dialogState = DialogueState.PAUSED;
        ShowNextLine();
    }

    //为什么不将NextLine写在这里：通过协程显示文字时，协程还在继续，但函数已返回，NextLine写在这会被调用
    //这样会产生问题：协程期间再点击，本应该只停协程，由于之前调用了NextLine，协程停止后显示的将是下一句话。
    public void ShowNextLine()
    {
        //if (isTopMenu)//topMenu显示时，点击事件无效,虽然用了RaycastTarget来控制点击，但top的Image与鼠标的位置还存在一定的偏移。
        //{
        //    return;
        //}
        if(dialogState == DialogueState.TYPING)
        {
            StartContentCoroutine(false);
            return;
        }
        else if(dialogState == DialogueState.PAUSED)
        {
            if(curNum >= curChapterData.dataArray.Length)
            {
                Init();
                return;
            }
            else
            {
                string tempType = curChapterData.dataArray[curNum].Type;
                switch (tempType)
                {
                    case "Dialogue":
                        SetBGPic();
                        SetRoles();
                        SetContent();
                        break;
                    case "Choice":
                        dialogState = DialogueState.CHOICE;
                        sceneManager.SetChoicePanel(curChapterData.dataList[curNum].Choicecontent);
                        sceneManager.ShowChoicePanel(true);
                        break;
                    case "Jump":
                        JumpTargetDialogue(curChapterData.dataArray[curNum].Targetindex);
                        return;
                    default:
                        break;
                }
            }
        }
        else
        {
            return;
        }
    }
    
    IEnumerator ContentCoroutine()
    {
        var charArray = curChapterData.dataList[curNum].Content.ToCharArray(); ;
        string tempText = "";
        for (int i = 0; i < charArray.Length; i++)
        {
            tempText += charArray[i];
            dialogueContent.text = tempText;
            yield return new WaitForSeconds(typingSpeed);
        }
        if (isAutoPlay)
        {
            yield return new WaitForSeconds(autoPlaySpeed);
        }
        //协程被Stop时，这些语句不会执行
        dialogState = DialogueState.PAUSED;
        //NextLine();
    }

    private void StartContentCoroutine(bool value)
    {
        if (value)
        {
            StartCoroutine("ContentCoroutine");
        }
        else
        {
            StopCoroutine("ContentCoroutine");
            dialogueContent.text = curChapterData.dataList[curNum].Content;
            dialogState = DialogueState.PAUSED;
            //NextLine();
        }
    }

    public void NextLine()
    {
        curNum++;
    }

    private void ShowTopMenu()
    {
        if(AVGController.instance.avgState == AVGState.Chapter)
        {
            //Debug.Log("Input.mousePosition.y: " + Input.mousePosition.y + "; Screen.height" + Screen.height + "; topMenu.GetComponent<RectTransform>().position.y: " + topMenu.GetComponent<RectTransform>().rect.height);
            //最后"-8"是因为直接用height，在game里鼠标与top的图片还有一定距离就能认为在范围内了，很奇怪！
            if (Input.mousePosition.y > Screen.height - Mathf.Abs(topMenu.GetComponent<RectTransform>().rect.height - 8))
            {
                topMenu.SetActive(true);
                isTopMenu = true;
            }
            else
            {
                topMenu.SetActive(false);
                isTopMenu = false;
            }
        }
    }

    public DialogueState ReturnDialogState()
    {
        return dialogState;
    }

    public void SetAutoPlay()
    {
        isAutoPlay = !isAutoPlay;
    }

    private void AutoPlay()
    {
        if (dialogState == DialogueState.PAUSED)
        {
            NextLine();
            ShowNextLine();
        }
    }

    public Data ReturnCurrentData()
    {
        Data tmpData = new Data();
        tmpData.chapterIndex = chapterIndex;
        tmpData.lineIndex = curNum - 1;
        tmpData.bgPic = curBGPic;
        tmpData.dialogContent = curDialogContent;
        tmpData.dateTime = DateTime.Now;
        return tmpData;
    }

    public void QuickSave()
    {
        AVGController.instance.SaveData(-1, ReturnCurrentData());
    }

    public void QucikLoad()
    {
        if (AVGController.instance.savedDatas.quickData.dateTime != DateTime.MinValue)
        {
            StartContentCoroutine(false);
            chapterIndex = AVGController.instance.savedDatas.quickData.chapterIndex;
            curNum = AVGController.instance.savedDatas.quickData.lineIndex;
            Init();
        }
    }
}
