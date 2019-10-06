using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asserts.Scripts.Model
{
    [SerializeField]
    public class SettingModel
    {
        public int cgIndex; //CG解锁个数
        public int memoryIndex; //Memory解锁个数
        public float VoiceVolume;   //CV声音
        public float BGMVolume;     //BGM声音
        public float BGVVolume;     //BGV声音
        public float dialogSpeed;   //文本速度
        public float skipSpeed;     //skip速度
        public bool noClothes;     //是否着装
        public bool isSkipReadedContext;    //是否跳过已读的文本
        public int chapterIndex;        //已读的最大章节号
        public int maxDialogIndex;      //已读的最大文本序列
    }
}