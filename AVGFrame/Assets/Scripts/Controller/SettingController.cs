using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    public static SettingController _isntance;
    public GameObject Bgm, Voice;
    public Slider BgmSlider, VoiceSlider, DialogSpeedSlider;
    // Use this for initialization
    void Awake()
    {
        _isntance = this;
        Bgm = GameObject.Find("BGVAudio");
        Voice = GameObject.Find("CVAudio");
    }



    public void ChangeBgmVolume()
    {
        Bgm.GetComponent<AudioSource>().volume = BgmSlider.value;
        Debug.Log(BgmSlider.value);
    }
    public void ChangeVoiceVolume()
    {
        Voice.GetComponent<AudioSource>().volume = VoiceSlider.value;
    }
    public void ChangeDialogSpeed()
    {
        ChapterController._instance.dialogSpeed = 0.35f - DialogSpeedSlider.value;
        Debug.Log(ChapterController._instance.dialogSpeed);
    }
}