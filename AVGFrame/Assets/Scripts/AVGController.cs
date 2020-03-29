using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class AVGController : MonoBehaviour
{
    public static AVGController instance;

    public AVGState avgState;

    public SavedDatas savedDatas;
    private int savedDatasNum;//可存档总数
    private List<int> sceneList;

    private void Start()
    {
        instance = this;
        savedDatasNum = 80;
        sceneList = new List<int>();
        InitList();
        LoadData();
        SetActiveScene((int)AVGState.Title);
        //ShowTargeScene((int)AVGState.Title);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //将不需要交互的GameObject的Raycast Target设为false，即可区分按钮与页面的点击事件。
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                UserClicked();
            }
            else
            {
                Debug.Log("Mouse is point over GameObject!");
            }
        }
    }
    private void Init()
    {
    }
    public void ShowTargeScene(int num)
    {
        avgState = (AVGState)num;
        SceneManager.instance.ShowScene(GetLastScene(), false);
        SetActiveScene(num);
        SceneManager.instance.ShowScene(avgState, true);
    }

    public void UserClicked()
    {
        switch (avgState)
        {
            case AVGState.Chapter:
                if(DialogueManager.instance.ReturnDialogState() == DialogueState.PAUSED)
                {
                    DialogueManager.instance.NextLine();
                }
                DialogueManager.instance.ShowNextLine();
                break;
            default:
                break;
        }
    }

    public void ProcessChoiceBtnMsg(GameObject btn)
    {
        string btnName = btn.name;
        DialogueManager.instance.JumpTargetDialogue(int.Parse(btnName));
    }

    public void SaveData(int num, Data data)
    {
        if(num == -1)//QuickData
        {
            savedDatas.quickData = data;
        }
        else
        {
            savedDatas.datas[num] = data;
        }
        string jsonString = JsonMapper.ToJson(savedDatas);
        StreamWriter sw = new StreamWriter(Application.dataPath + "/SavedDatas.txt");
        sw.Write(jsonString);
        sw.Close();
    }

    private void LoadData()
    {
        if(File.Exists(Application.dataPath + "/SavedDatas.txt"))
        {
            StreamReader sr = new StreamReader(Application.dataPath + "/SavedDatas.txt");
            string jsonString = sr.ReadToEnd();
            sr.Close();
            savedDatas = JsonMapper.ToObject<SavedDatas>(jsonString);
        }
        else
        {
            Debug.Log("NOT FOUND FILE");
        }
    }

    private void InitList()
    {
        savedDatas = new SavedDatas();
        savedDatas.datas = new List<Data>();
        for (int i = 0; i < savedDatasNum; i++)
        {
            savedDatas.datas.Add(new Data());
        }
    }

    public void ExitBtn(int sceneNum)
    {
        SceneManager.instance.ShowScene((AVGState)sceneNum, false);
        RemoveHistoryScene();
        SceneManager.instance.ShowScene(GetLastScene(), true);
        Debug.Log("GetActiveScene: " + sceneList);
    }

    public void SetActiveScene(int num)
    {
        if(num == (int)AVGState.Title)
        {
            sceneList.Clear();
        }
        sceneList.Add(num);
        Debug.Log("SetActiveScene: " + sceneList);
    }

    public void RemoveHistoryScene()
    {
        int num = sceneList.Count;
        sceneList.RemoveAt(num - 1);
    }

    public AVGState GetLastScene()
    {
        int num = sceneList.Count;
        return (AVGState)sceneList[num - 1];
    }
}
