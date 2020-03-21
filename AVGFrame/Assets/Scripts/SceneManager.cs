using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject titleContainer;
    [SerializeField]
    private GameObject chapterContainer;

    public DialogueManager dialogManager;

    public void ShowScene(AVGState state)
    {
        switch (state)
        {
            case AVGState.Title:
                ShowTitleContainer(true);
                break;
            case AVGState.Chapter:
                ShowChapterContainer(true);
                break;
            default:
                break;
        }
    }

    private void ShowTitleContainer(bool value)
    {
        titleContainer.SetActive(value);
    }

    private void ShowChapterContainer(bool value)
    {
        if (titleContainer.activeSelf)
        {
            ShowTitleContainer(false);
        }
        chapterContainer.SetActive(value);
        dialogManager.Init();
    }
}
