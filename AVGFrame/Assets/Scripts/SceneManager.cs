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
    

    public void ShowScene(AVGState state, bool value)
    {
        switch (state)
        {
            case AVGState.Title:
                TitleManager.instance.ShowTitleContainer(value);
                break;
            case AVGState.Chapter:
                DialogueManager.instance.ShowChapterContainer(value);
                break;
            case AVGState.Save:
                SaveLoadManager.instance.ShowSaveLoadContainer(value);
                break;
            case AVGState.Load:
                SaveLoadManager.instance.ShowSaveLoadContainer(value);
                break;
            default:
                break;
        }
    }

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

}
