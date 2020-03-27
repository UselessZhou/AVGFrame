using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AVGController : MonoBehaviour
{
    public static AVGController instance;

    public AVGState avgState;
    
    public SavedDatas savedDatas;
    private int savedDatasNum;//可存档总数

    private void Start()
    {
        instance = this;
        savedDatasNum = 80;
        InitList();
        LoadData();

        ShowTargetScene(AVGState.Title);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UserClicked();
        }
    }

    public void ShowTargetScene(AVGState tmpState)
    {
        avgState = tmpState;
        SceneManager.instance.ShowScene(avgState);
    }

    public void UserClicked()
    {
        switch (avgState)
        {
            case AVGState.Chapter:
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
        savedDatas.datas[num] = data;
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
}
