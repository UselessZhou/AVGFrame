using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 序列化存储的数据格式，用于传输时序列化与反序列化，便于存储
/// </summary>
namespace Asserts.Scripts.Model
{
    [SerializeField]
    public class CGIndexModel
    {
        public int[] CG1;
        public int[] CG2;
        public int[] CG3;
        public int[] CG4;
        public int[] CG5;
        public int[] CG6;

    }
}
