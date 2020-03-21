using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AVGController : MonoBehaviour
{
    public AVGState avgState;

    public DialogueManager dialogManager;
    public SceneManager sceneManager;
    
    public void ShowTargetScene(AVGState tmpState)
    {
        avgState = tmpState;
        sceneManager.ShowScene(avgState);
    }

    public void UserClicked()
    {
        switch (avgState)
        {
            case AVGState.Chapter:
                dialogManager.ShowNextLine();
                break;
            default:
                break;
        }
    }

    public void ProcessChoiceBtnMsg(GameObject btn)
    {
        string btnName = btn.name;
        dialogManager.JumpTargetDialogue(int.Parse(btnName));
    }
}
