using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    [SerializeField]
    private DialogueState dialogState;

    public GameObject chapterContainer;
    public Sheet1[] chapterDatas;
    private Sheet1 curChapterData; //这个是QucikSheet解析的Excel数据

    [Range(0.01f,0.08f)]
    public float typingSpeed = 0.05f;
    
    [SerializeField]
    private int curNum;
    private int chapterIndex;

    [Header("Roles")]
    public GameObject leftRole;
    public GameObject rightRole;
    public GameObject centerRole;

    [Header("ChoicePanel")]
    public GameObject choicePanel;
    public GameObject[] choiceBtns;

    private GameObject leftRolePic;
    private GameObject rightRolePic;
    private GameObject centerRolePic;

    private Text roleName;
    private Text dialogueContent;

    // Start is called before the first frame update
    void Start()
    {
        dialogState = DialogueState.START;

        GameObject lineContainer = chapterContainer.transform.Find("LineContainer").gameObject;

        roleName = lineContainer.transform.Find("RoleName").gameObject.GetComponent<Text>();
        dialogueContent = lineContainer.transform.Find("Line").gameObject.GetComponent<Text>();

        leftRolePic = leftRole.transform.Find("LeftRolePic").gameObject;
        rightRolePic = rightRole.transform.Find("RightRolePic").gameObject;
        centerRolePic = centerRole.transform.Find("CenterRolePic").gameObject;
    }

    // Update is called once per frame
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
    }

    public void Init()
    {
        curNum = 0;
        chapterIndex = 0;
        LoadChapter();
        ShowRoles();
        ShowContent();
        //NextLine();
    }

    private void LoadChapter()
    {
        curChapterData = chapterDatas[chapterIndex];
    }

    private void ShowContent()
    {
        dialogState = DialogueState.TYPING;
        roleName.text = curChapterData.dataList[curNum].Name;
        StartContentCoroutine(true);
    }

    private void ShowRoles()
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

    //private void CheckTypingFinished()
    //{
    //    if(dialogState == DialogueState.TYPING)
    //    {
    //        dialogState = DialogueState.PAUSED;

    //    }
    //}

    private void ShowChoicePanel(bool value)
    {
        dialogState = DialogueState.CHOICE;
        if (value)
        {
            choicePanel.SetActive(true);
            var choiceContent = curChapterData.dataList[curNum].Choicecontent;
            string[] choiceArray = choiceContent.Split('#');
            for (int i = 0; i < choiceArray.Length; i++)
            {
                string[] tmpContent = choiceArray[i].Split('>');
                choiceBtns[i].GetComponentInChildren<Text>().text = tmpContent[0];
                choiceBtns[i].name = tmpContent[1];
            }
        }
        else
        {
            choicePanel.SetActive(false);
        }

    }

    public void JumpTargetDialogue(int targetId)
    {
        Sheet1Data tmpData = curChapterData.dataList.Find((Sheet1Data data) => data.Index == targetId);
        curNum = tmpData.Index - 1;
        ShowChoicePanel(false);
        dialogState = DialogueState.PAUSED;
        ShowNextLine();
    }

    //为什么不将NextLine写在这里：通过协程显示文字时，协程还在继续，但函数已返回，NextLine写在这会被调用
    //这样会产生问题：协程期间再点击，本应该只停协程，由于之前调用了NextLine，协程停止后显示的将是下一句话。
    public void ShowNextLine()
    {
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
                        ShowRoles();
                        ShowContent();
                        break;
                    case "Choice":
                        ShowChoicePanel(true);
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
        //协程被Stop时，这些语句不会执行
        dialogState = DialogueState.PAUSED;
        NextLine();
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
            NextLine();
        }
    }

    private void NextLine()
    {
        curNum++;
    }
}
