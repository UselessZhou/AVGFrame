using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    public static SettingController _isntance;
    public GameObject Bgm, Bgv, Voice;
    public Slider BgmSlider, BgvSlider, VoiceSlider, DialogSpeedSlider, SkipSpeedSlider, DialogTransparentSlider;
    // Use this for initialization
    void Awake()
    {
        _isntance = this;
        Bgm = GameObject.Find("BGMAudio");
        Voice = GameObject.Find("CVAudio");
        Bgv = GameObject.Find("BGVAudio");
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
    }

    public void ChangeSkipSpeed()
    {
        ChapterController._instance.skipSpeed = 0.35f - SkipSpeedSlider.value;
    }

    public void ChangeBgvVolume()
    {
        Bgv.GetComponent<AudioSource>().volume = BgvSlider.value;
        Debug.Log(BgvSlider.value);
    }

    public void ChangeDialogTransparent()
    {
        ChapterController._instance.lineContainer.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, DialogTransparentSlider.value);
        Debug.Log(DialogTransparentSlider.value);
    }
    
}