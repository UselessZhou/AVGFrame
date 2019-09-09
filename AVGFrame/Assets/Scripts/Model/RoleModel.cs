using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 角色数据类型（可支持3个角色）
/// </summary>
namespace Asserts.Scripts.Model
{
    [SerializeField]
    public class RoleModel
    {
        public Sprite rolePic;  //角色贴图
        public string roleName; //角色姓名
        public AudioClip roleVoice;    //角色配音
        public string roleLine;     //角色对话内容
        public string roleAction;   //角色动画
    }
}

