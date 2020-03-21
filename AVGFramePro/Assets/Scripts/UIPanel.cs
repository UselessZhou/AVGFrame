using UnityEngine;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    public RawImage charaAImg;
    public RawImage charaBImg;
    public Image contentBg;
    public Text contentTxt;
    public Canvas canvas;
    public GameObject btnA;
    public GameObject btnB;

    public void SetButtonNames(string nameA,string nameB)
    {
        btnA.name = nameA;
        btnB.name = nameB;
    }

    public void ShowButtons(bool value)
    {
        btnA.SetActive(value);
        btnB.SetActive(value);
    }

    public void SetButtonTexts(string txtA,string txtB)
    {
        Text tempText = btnA.GetComponentInChildren<Text>();
        tempText.text = txtA;
        tempText = btnB.GetComponentInChildren<Text>();
        tempText.text = txtB;
    }


    public void ShowCanvas(bool value)
    {
        canvas.enabled = value;
    }


    public void ShowCharaA(bool value)
    {
        charaAImg.enabled = value;
    }

    public void ShowCharaB(bool value)
    {
        charaBImg.enabled = value;

    }

    public void ShowContentBG(bool value)
    {
        contentBg.enabled = value;
    }

    public void ShowContentText(bool value)
    {
        contentTxt.enabled = value;
    }

    public void SetContentText(string value)
    {
        contentTxt.text = value;
    }

    public void ChangeCharaATexture(Texture tex)
    {
        charaAImg.texture = tex;
    }

    public void ChangeCharaBTexture(Texture tex)
    {
        charaBImg.texture = tex;
    }
}
