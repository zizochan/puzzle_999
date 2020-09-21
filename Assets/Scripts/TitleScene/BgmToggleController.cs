using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgmToggleController : MonoBehaviour
{
    public Toggle bgmToggle;

    // Start is called before the first frame update
    void Start()
    {
        bgmToggle = GetComponent<Toggle>();
        bgmToggle.isOn = Data.CONFIG_SOUND_PLAY;
    }

    public void OnToggleChanged()
    {
        Data.CONFIG_SOUND_PLAY = bgmToggle.isOn;
    }
}
