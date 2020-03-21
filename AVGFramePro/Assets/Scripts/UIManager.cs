using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public AVGMachine machine;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            machine.StartAVG();
        }
        if (Input.GetMouseButtonDown(0))
        {
            machine.UserClicked();
        }
    }
}
