using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

public class TwitterButtonController : MonoBehaviour
{
    string HASH_TAG = "PUZZLE999";

    // 新規ウィンドウを開く 参考: https://isemito.hatenablog.com/entry/2018/08/08/203017
    [DllImport("__Internal")]
    private static extern void OpenToBlankWindow(string _url);

    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnClickTwitterButton()
    {
        string url = GetTwitterURL();
        OpenUrl(url);
    }

    string GetTwitterURL()
    {
        List<string> texts = new List<string>();

        texts.Add("「" + Data.GAME_TITLE + "」");
        texts.Add("");
        texts.Add("HIGH SCORE: " + Data.highScore);
        texts.Add("");
        texts.Add("https://unityroom.com/games/puzzle_999");

        string text = string.Join("\n", texts);
        string esctext = UnityWebRequest.EscapeURL(text);
        string esctag = UnityWebRequest.EscapeURL(HASH_TAG);

        return "https://twitter.com/intent/tweet?text=" + esctext + "&hashtags=" + esctag;
    }

    void OpenUrl(string url)
    {
#if UNITY_EDITOR
        Application.OpenURL(url);
#elif UNITY_WEBGL
        OpenToBlankWindow(url);
#else
        Application.OpenURL(url);
#endif
    }
}
