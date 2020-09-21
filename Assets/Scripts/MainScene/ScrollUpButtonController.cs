using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUpButtonController : MonoBehaviour
{
    bool clickFlag;

    void Start()
    {
        clickFlag = false;
    }

    public void PushDown()
    {
        clickFlag = true;
    }

    public void PushUp()
    {
        clickFlag = false;
    }

    public bool IsClick()
    {
        return clickFlag;
    }
}
