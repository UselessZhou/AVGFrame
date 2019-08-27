using System;
using UnityEngine;

namespace Assets.Script.Title.Camera
{

    public class GameController : MonoBehaviour
    {
        public GameObject bgmAudioSource;

        private AudioClip bgmMusic;
        private AudioSource bgmAudio;

        private void Start()
        {
            //如何在Main Camera层增加Script来用代码控制其所要播放的BGM（也可直接通过新建Audio Source中的Clip属性设置）
            bgmMusic = (AudioClip)Resources.Load("Audio/TitleBGM", typeof(AudioClip));
            bgmAudio = bgmAudioSource.GetComponent<AudioSource>();
            bgmAudio.clip = bgmMusic;
            bgmAudio.loop = true;
            bgmAudio.playOnAwake = true;
            bgmAudio.Play();
        }
    }

}

