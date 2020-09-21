using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverSceneController : MonoBehaviour
{
    int score;
    int highScore;
    bool isHighScoreUpdated;

    TextController scoreText;
    TextController highScoreText;
    TextController highScoreUpdateText;

    // Start is called before the first frame update
    void Start()
    {
        FadeManager.FadeIn(2f);

        SetInstances();
        SetScore();
        SetScoreTexts();

        // フラグだけ戻しておく
        Data.ChangeStatus(Data.STATUS_INITIAL);
    }

    void SetScore()
    {
        score = Data.tmpScore;
        highScore = Data.highScore;
        isHighScoreUpdated = Data.isHighScoreUpdated;
    }

    void SetInstances()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<TextController>();
        highScoreText = GameObject.Find("HighScoreText").GetComponent<TextController>();
        highScoreUpdateText = GameObject.Find("HighScoreUpdateText").GetComponent<TextController>();
    }

    void SetScoreTexts()
    {
        string text1 = "SCORE: " + score.ToString();
        scoreText.SetText(text1);

        string text2 = "HIGH SCORE: " + highScore.ToString();
        highScoreText.SetText(text2);

        if (!isHighScoreUpdated)
        {
            highScoreUpdateText.gameObject.SetActive(false);
        }
    }
}
