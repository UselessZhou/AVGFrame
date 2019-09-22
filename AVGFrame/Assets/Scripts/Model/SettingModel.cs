using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asserts.Scripts.Model
{
    [SerializeField]
    public class SettingModel
    {
        public int cgIndex; //CG解锁个数
        public float VoiceVolume;   //CV声音
        public float BGMVolume;     //BGM声音
        public float dialogSpeed;   //文本速度
    }
}