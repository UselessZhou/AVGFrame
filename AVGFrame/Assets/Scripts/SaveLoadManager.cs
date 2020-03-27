using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;
    public GameObject saveLoadContainer;
    public Button firstPageBtn;
    public int curPage;
    private int btnNums;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        btnNums = 8;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        curPage = 0;
        firstPageBtn.Select();
        ShowSaveLoadContainer(true);
        ShowSavedDatas(0, AVGController.instance.savedDatas);
    }

    public void ShowSaveLoadContainer(bool value)
    {
        saveLoadContainer.SetActive(value);
    }

    public void RefreshSavedData(int index, Data data)
    {
        saveLoadContainer.transform.Find("SavedField" + index).transform.Find("SavedPic").GetComponent<Image>().sprite = (Sprite)Resources.Load("Image/ChapterBG/" + data.bgPic, typeof(Sprite));
        saveLoadContainer.transform.Find("SavedField" + index).transform.Find("DataContent").Find("Date").GetComponent<Text>().text = data.dateTime.ToString();
        saveLoadContainer.transform.Find("SavedField" + index).transform.Find("DataContent").Find("DialogContent").GetComponent<Text>().text = data.dialogContent;
    }

    public void ShowSavedDatas(int index, SavedDatas savedDatas)
    {
        for (int i = index; i < index + btnNums; i++)
        {
            int btnIndex = i - index;
            if (savedDatas.datas[i].lineIndex == 0)
            {
                saveLoadContainer.transform.Find("SavedField" + btnIndex).transform.Find("SavedPic").GetComponent<Image>().sprite = (Sprite)Resources.Load("Image/ChapterBG/noData", typeof(Sprite));
                saveLoadContainer.transform.Find("SavedField" + btnIndex).transform.Find("DataContent").Find("Date").GetComponent<Text>().text = "----/--/-- --:--:--";
                saveLoadContainer.transform.Find("SavedField" + btnIndex).transform.Find("DataContent").Find("DialogContent").GetComponent<Text>().text = "";
            }
            else
            {
                saveLoadContainer.transform.Find("SavedField" + btnIndex).transform.Find("SavedPic").GetComponent<Image>().sprite = (Sprite)Resources.Load("Image/ChapterBG/" + savedDatas.datas[i].bgPic, typeof(Sprite));
                saveLoadContainer.transform.Find("SavedField" + btnIndex).transform.Find("DataContent").Find("Date").GetComponent<Text>().text = savedDatas.datas[i].dateTime.ToString();
                saveLoadContainer.transform.Find("SavedField" + btnIndex).transform.Find("DataContent").Find("DialogContent").GetComponent<Text>().text = savedDatas.datas[i].dialogContent;
            }
            saveLoadContainer.transform.Find("SavedField" + btnIndex).transform.Find("DataContent").Find("Number").GetComponent<Text>().text = (i+1).ToString().PadLeft(3, '0');
        }
    }

    public void OnClick(int num)
    {
        Data tmpData = new Data();
        if(AVGController.instance.avgState == AVGState.Save)
        {
            int index = curPage * 8 + num;
            tmpData.chapterIndex = DialogueManager.instance.chapterIndex;
            tmpData.lineIndex = DialogueManager.instance.curNum - 1;
            tmpData.bgPic = DialogueManager.instance.curBGPic;
            tmpData.dialogContent = DialogueManager.instance.curDialogContent;
            tmpData.dateTime = DateTime.Now;
            AVGController.instance.SaveData(index, tmpData);
            RefreshSavedData(index, tmpData);
        }
        else if (AVGController.instance.avgState == AVGState.Load)
        {
            tmpData = AVGController.instance.savedDatas.datas[num];
            DialogueManager.instance.chapterIndex = tmpData.chapterIndex;
            DialogueManager.instance.curNum = tmpData.lineIndex;
            ShowSaveLoadContainer(false);
            AVGController.instance.avgState = AVGState.Chapter;
            AVGController.instance.ShowTargetScene(AVGState.Chapter);
        }
    }

    public void ChangeDataPage(int index)
    {
        ShowSavedDatas(index * btnNums, AVGController.instance.savedDatas);
    }
}
