using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTextController : MonoBehaviour
{
    int score;
    int tmpScore;
    int speed = 1;

    private void Start()
    {
        score = 0;
        tmpScore = 0;

        SetText(score);
    }

    private void Update()
    {
        if (score == tmpScore)
        {
            return;
        }

        if (score > tmpScore)
        {
            tmpScore += speed;
        } else if (score < tmpScore)
        {
            tmpScore -= speed;
        }

        SetText(tmpScore);
    }

    void SetText(int value)
    {
        this.GetComponent<Text>().text = value.ToString();
    }

    public void SetScore(int value)
    {
        score = value;
    }
}
