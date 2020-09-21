using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionSceneController : MonoBehaviour
{
    int index;

    const int MIN_INDEX = 1;
    const int MAX_INDEX = 4;

    TextController descriptionText;
    TextController descriptionTitle;

    AudioSource audioSourceSe;
    public AudioClip clickSound;

    // Start is called before the first frame update
    void Start()
    {
        index = MIN_INDEX;

        descriptionText = GameObject.Find("DescriptionText").GetComponent<TextController>();
        descriptionTitle = GameObject.Find("DescriptionTitle").GetComponent<TextController>();

        audioSourceSe = GetComponent<AudioSource>();

        UpdateDescriptionText();
    }

    void UpdateDescriptionText()
    {
        string title = GetDescriptionTitle();
        descriptionTitle.SetText(title);

        string text = GetDescriptionText();
        descriptionText.SetText(text);
    }

    string GetDescriptionText()
    {
        List<string> texts = new List<string>();

        switch (index)
        {
            case 2:
                texts.Add("浮いているブロックが下に落下するとき、");
                texts.Add("そのブロックの<color='red'>数字が増えて</color>しまいます。");
                texts.Add("");
                texts.Add("消すのが難しくなりますが、数字が大きいほど");
                texts.Add("消した時の点数も高くなります。");
                texts.Add("");
                texts.Add("ピンチはチャンスです！");
                break;
            case 3:
                texts.Add("これは超重要です！！");
                texts.Add("");
                texts.Add("縦か横に<color='red'>９のブロックが３つ並んでる</color>時には、");
                texts.Add("なんと特別ルールで消すことができます。");
                texts.Add("");
                texts.Add("超高得点で、さらに他にも良いことが！？");
                texts.Add("９９９ボーナスが攻略の鍵です！");
                break;
            case 4:
                texts.Add("他にも色々出てきますが、後はぜひプレイして下さい。");
                texts.Add("シンプルなゲームなので、一度遊んでみれば");
                texts.Add("分かると思います。");
                texts.Add("");
                texts.Add("ここまで読んでくれてありがとう。");
                texts.Add("感想待ってます！");
                break;
            default:
                texts.Add("選んだブロックの数値の合計が、");
                texts.Add("<color='red'>画面右上の数字と同じ</color>になれば消すことができます。");
                texts.Add("");
                texts.Add("まとめてたくさん消す、もしくは");
                texts.Add("大きな数字のブロックを消すほど高得点！");
                texts.Add("");
                texts.Add("ブロックが上まで詰まるとゲームオーバーです。");
                break;
        }

        return string.Join("\n", texts);
    }

    string GetDescriptionTitle()
    {
        string title;

        switch (index)
        {
            case 2:
                title = "ルール② 数字が増える！";
                break;
            case 3:
                title = "ルール③ ９９９ボーナス！";
                break;
            case 4:
                title = "その他";
                break;
            default:
                title = "ルール① ブロックの消し方";
                break;
        }

        return title;
    }

    public void OnRightClick()
    {
        PlayClickSound();
        AddIndex();
        UpdateDescriptionText();
    }

    public void OnLeftClick()
    {
        PlayClickSound();
        ReduceIndex();
        UpdateDescriptionText();
    }

    void AddIndex()
    {
        index++;

        if (index > MAX_INDEX)
        {
            index = MIN_INDEX;
        }
    }

    void ReduceIndex()
    {
        index--;

        if (index < MIN_INDEX)
        {
            index = MAX_INDEX;
        }
    }

    void PlayClickSound()
    {
        if (Data.IsBgmStop())
        {
            return;
        }

        if (clickSound == null)
        {
            return;
        }

        audioSourceSe.PlayOneShot(clickSound);
    }
}
