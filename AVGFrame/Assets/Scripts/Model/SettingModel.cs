using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asserts.Scripts.Model
{
    [SerializeField]
    public class SettingModel
    {
        public string cgSavedData; //CG解锁情况
        public int memoryIndex; //Memory解锁个数
        public float VoiceVolume;   //CV声音
        public float BGMVolume;     //BGM声音
        public float BGVVolume;     //BGV声音
        public float dialogSpeed;   //文本速度
        public float skipSpeed;     //skip速度
        public float[] charactersVolume;   //角色声音数组
        public float dialogTransparent;     //对话框透明度
        public bool noClothes;     //是否着装
        public bool isSkipReadedContext;    //是否跳过已读的文本
        public bool isSkipUntilHScene;  //是否Skip到H场景停止
        public bool isSkipUntilShoot;  //是否Skip到sj倒计时
        public int shootNumber;         //sj倒计时行数
        public int chapterIndex;        //已读的最大章节号
        public int maxDialogIndex;      //已读的最大文本序列
        public bool isContinuePlayCV;   //是否持续播放CV至结束
        public bool isChangeReadedTextColor;    //是否更改已读文本的颜色
        public int rightFunction;   //右键功能
        public int shootChoices;    //sj选项
    }
}