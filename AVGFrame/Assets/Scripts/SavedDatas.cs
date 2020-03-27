using System;
using System.Collections.Generic;
using UnityEngine;

public class SavedDatas:ScriptableObject
{
    public List<Data> datas;
}

public class Data
{
    public int chapterIndex;
    public int lineIndex;
    public string bgPic;
    public DateTime dateTime;
    public string dialogContent;
}