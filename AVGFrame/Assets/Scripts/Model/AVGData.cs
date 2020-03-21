using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "AVGData", menuName = "AVG Model/AVGData")]
public class AVGData : ScriptableObject
{
    public List<DialogContent> contents;
}
[System.Serializable]
public class DialogContent
{
    public string dialogueContent;
    public bool leftRoleHide;
    public bool rightRoleHide;
    public bool centerRoleHide;
    public Texture leftRolePic;
    public Texture rightRolePic;
    public Texture centerRolePic;
    
}
