using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionButtonController : MonoBehaviour
{
    DescriptionSceneController descriptionSceneController;

    // Start is called before the first frame update
    void Start()
    {
        descriptionSceneController = GameObject.Find("DescriptionSceneController").GetComponent<DescriptionSceneController>();
    }

    public void OnRightClick()
    {
        descriptionSceneController.OnRightClick();
    }

    public void OnLeftClick()
    {
        descriptionSceneController.OnLeftClick();
    }
}
