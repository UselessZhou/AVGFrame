using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private void Start()
    {
        //avgController.ShowTargetScene(AVGState.Title);
    }

    // 这里用于获取玩家的点击，再交由AVGController
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    avgController.UserClicked();
        //}
    }

    public void GoChapter()
    {
        AVGController.instance.ShowTargetScene(AVGState.Chapter);
    }

    public void GoSave()
    {
        AVGController.instance.ShowTargetScene(AVGState.Save);
    }
    public void GoLoad()
    {
        AVGController.instance.ShowTargetScene(AVGState.Load);
    }
}
