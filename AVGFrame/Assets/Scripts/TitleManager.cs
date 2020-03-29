using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    private GameObject titleContainer;

    public static TitleManager instance;
    private void Awake()
    {
        instance = this;
        
    }
    void Start()
    {

    }

    public void Init()
    {
        titleContainer.SetActive(true);
    }
    
    public void ShowTitleContainer(bool value)
    {
        if (value)
        {
            Init();
        }
        else
        {
            titleContainer.SetActive(value);
        }
    }

}
