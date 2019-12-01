using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingController : MonoBehaviour
{
    public static SettingController _isntance;
    public GameObject Bgm, Bgv, Voice;
    public Slider BgmSlider, BgvSlider, DialogSpeedSlider, SkipSpeedSlider, 
        DialogTransparentSlider, FirstVoiceSlider, SecondVoiceSlider, ThirdVoiceSlider, 
        FourthVoiceSlider, SEVoiceSlider, HSEVoiceSlider;
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
    }

    public void ChangeSEVolume()
    {
        ChapterController._instance.seAudioSource.volume = SEVoiceSlider.value;
    }

    public void ChangeHSEVolume()
    {
        ChapterController._instance.hseAudioSource.volume = HSEVoiceSlider.value;
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
    }

    public void ChangeDialogTransparent()
    {
        ChapterController._instance.lineContainer.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, DialogTransparentSlider.value);
    }

    public void ChangeFirstVoiceVolume()
    {
        GameController._instance.settingDatas.charactersVolume[0] = FirstVoiceSlider.value;
    }

    public void ChangeSecondVoiceVolume()
    {
        GameController._instance.settingDatas.charactersVolume[1] = SecondVoiceSlider.value;
    }

    public void ChangeThirdVoiceVolume()
    {
        GameController._instance.settingDatas.charactersVolume[2] = ThirdVoiceSlider.value;
    }

    public void ChangeFourthVoiceVolume()
    {
        GameController._instance.settingDatas.charactersVolume[3] = FourthVoiceSlider.value;
    }
}