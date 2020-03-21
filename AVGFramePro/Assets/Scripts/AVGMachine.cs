using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AVGMachine : MonoBehaviour
{

    public enum STATE{
        OFF,
        TYPING,
        PAUSED,
        CHOICE
    }
    public STATE state;

    private bool justEnter;

    public Story01 data;
    public AVGAssetConfig asset;
    public UIPanel panel;
    private int curLine;

    [Range(1f,10f)]
    public float typingSpeed = 5f;

    private string targetString;
    private float timerValue;
    // Start is called before the first frame update
    void Start()
    {
        //Init();
        state = STATE.OFF;
        justEnter = true;
    }

    public void StartAVG()
    {
        GoToState(STATE.TYPING);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case STATE.OFF:
                if (justEnter)
                {
                    Init();
                    LoadCharaTexture(asset.charaATex, asset.charaBTex);
                    justEnter = false;
                }
                break;
            case STATE.TYPING:
                if (justEnter)
                {
                    ShowUI();
                    LoadContent(data.dataList[curLine].Dialogtext, data.dataList[curLine].Charaadisplay, data.dataList[curLine].Charabdisplay);
                    justEnter = false;
                    timerValue = 0;
                }
                CheckTypingFinished();
                UpdateContentString();
                break;
            case STATE.PAUSED:
                if (justEnter)
                {
                    justEnter = false;
                }
                break;
            case STATE.CHOICE:
                if (justEnter)
                {
                    panel.ShowButtons(true);
                    panel.SetButtonNames(data.dataList[curLine].Btnamsg, data.dataList[curLine].Btnbmsg);
                    panel.SetButtonTexts(data.dataList[curLine].Btnatext, data.dataList[curLine].Btnbtext);
                    justEnter = false;
                }
                break;
            default:
                break;
        }

        /*
        if(Input.GetKeyDown("1"))
        {
            Init();
            LoadCharaTexture(asset.charaATex, asset.charaBTex);
            ShowUI();
        }

        if (Input.GetMouseButtonDown(0))
        {
            NextLine();
            if(curLine >= data.dataList.Count)
            {
                curLine = data.dataList.Count;
                Init();
            }
            LoadContent(data.dataList[curLine].Dialogtext, data.dataList[curLine].Charaadisplay, data.dataList[curLine].Charabdisplay);
        }*/
    }

    public void UserClicked()
    {
        switch (state)
        {
            //case STATE.OFF:
            //    GoToState(STATE.TYPING);
            //    break;
            case STATE.TYPING:
                break;
            case STATE.PAUSED:
                NextLine();
                if (curLine >= data.dataList.Count)
                {
                    GoToState(STATE.OFF);
                }
                else
                {
                    GoToState(STATE.TYPING);

                }
                break;
            default:
                break;
        }
    }

    private void CheckTypingFinished()
    {
        if(state == STATE.TYPING)
        {
            if(Mathf.Min((int)Mathf.Floor(timerValue * typingSpeed))>= targetString.Length){
                if (data.dataList[curLine].Ischoice)
                {
                    GoToState(STATE.CHOICE);
                }
                else
                {
                    GoToState(STATE.PAUSED);
                }
            }
        }
    }

    private void GoToState(STATE next)
    {
        state = next;
        justEnter = true;
    }

    void Init()
    {
        HideUI();
        curLine = 0;
        panel.SetContentText("");
        LoadContent(data.dataList[curLine].Dialogtext, data.dataList[curLine].Charaadisplay, data.dataList[curLine].Charabdisplay);
        panel.ShowButtons(false);
    }

    void ShowUI()
    {
        panel.ShowCanvas(true);
    }

    void HideUI()
    {
        panel.ShowCanvas(false);
    }

    void NextLine()
    {
        curLine++;
    }

    //void LoadText(string value)
    //{
    //    panel.SetContentText(value);
    //}

    void LoadContent(string value, bool charaADisplay, bool charaBDisplay)
    {
        //panel.SetContentText(value);
        targetString = value;

        panel.ShowCharaA(charaADisplay);
        panel.ShowCharaB(charaBDisplay);
    }

    void UpdateContentString()
    {
        timerValue += Time.deltaTime;
        string tempString = targetString.Substring(0, Mathf.Min((int)Mathf.Floor(timerValue*typingSpeed),targetString.Length));
        panel.SetContentText(tempString);
    }

    void LoadCharaTexture(Texture charaATex, Texture charaBTex)
    {
        panel.ChangeCharaATexture(charaATex);
        panel.ChangeCharaBTexture(charaBTex);
    }

    public void ProcessBtnMsg(GameObject obj)
    {
        switch (obj.name)
        {
            case "AAA":
                Story01 tempStory = Resources.Load<Story01>("Story02");
                data = tempStory;

                Init();
                LoadCharaTexture(asset.charaATex, asset.charaBTex);
                justEnter = false;
                GoToState(STATE.TYPING);
                break;
            case "BBB":
                break;

            default:
                break;
        }
    }
}
