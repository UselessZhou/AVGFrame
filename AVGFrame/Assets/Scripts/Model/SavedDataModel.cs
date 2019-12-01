using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 序列化存储的数据格式，用于传输时序列化与反序列化，便于存储
/// </summary>
namespace Asserts.Scripts.Model
{
    [SerializeField]
    public class SavedDataModel
    {
        public int savedDataIndex;  //存储序号
        public int chapterIndex;    //跳转的章节号
        public int dialogInedx;      //跳转幕的序号
        public DateTime savedTime;
        //public string savedPicName; //存储关联的截图png文件名
    }
}

