using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    Button button;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //切换btn.sprite的方式，但这样由于各个btn的image放在不同的文件夹中，无法正常读取，也许需要将所有btn放在同一个文件夹，这样会比较乱吗？
        //string btnName = "save1";
        //button.GetComponent<Image>().sprite = Resources.Load<Sprite>(string.Format("Image/Dialog/Operation/{0}", btnName));
        button.GetComponent<Image>().color = Color.red;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        button.GetComponent<Image>().color = Color.white;
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log("In Down!!!!!!!!!");
    //    button.GetComponent<Image>().color = Color.red;
    //}

    public void OnPointerUp(PointerEventData eventData)
    {
        button.GetComponent<Image>().color = Color.white;
    }

        // Start is called before the first frame update
        void Start()
    {
        button = GetComponent<Button>();
    }

}
