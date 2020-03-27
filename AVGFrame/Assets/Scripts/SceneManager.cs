using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance;

    [SerializeField]
    private GameObject choicePanel;
    public GameObject[] choiceBtns;

    private void Awake()
    {
        instance = this;
        
    }
    private void Start()
    {
    }

    public void ShowScene(AVGState state)
    {
        switch (state)
        {
            case AVGState.Title:
                TitleManager.instance.ShowTitle(true);
                break;
            case AVGState.Chapter:
                TitleManager.instance.ShowTitle(false);
                DialogueManager.instance.Init();
                break;
            case AVGState.Save:
                SaveLoadManager.instance.Init();
                break;
            case AVGState.Load:
                SaveLoadManager.instance.Init();
                break;
            default:
                break;
        }
    }

    //private void ShowTitleContainer(bool value)
    //{
    //    TitleManager.instance.ShowTitle(value);
    //}

    //private void ShowChapterContainer(bool value)
    //{
    //    if (titleContainer.activeSelf)
    //    {
    //        ShowTitleContainer(false);
    //    }
    //    chapterContainer.SetActive(value);
    //    dialogManager.Init();
    //}

    public void ShowChoicePanel(bool value)
    {
        choicePanel.SetActive(value);
    }

    public void SetChoicePanel(string choiceContent)
    {
        string[] choiceArray = choiceContent.Split('#');
        for (int i = 0; i < choiceArray.Length; i++)
        {
            string[] tmpContent = choiceArray[i].Split('>');
            choiceBtns[i].GetComponentInChildren<Text>().text = tmpContent[0];
            choiceBtns[i].name = tmpContent[1];
        }
    }

    //public void ShowSaveLoadContainer(bool isShow)
    //{
    //    saveLoadContainer.SetActive(isShow);
    //}

}
