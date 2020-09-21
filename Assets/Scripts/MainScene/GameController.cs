using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    int mapX = Data.MAP_X;
    int mapY = Data.MAP_Y;
    int question;
    int answer;
    int score;
    int deleteCount;
    int gameLevel;
    int droppingPanelCount = 0;

    GameObject panelObject;
    PanelFieldController panelField;
    GameObject panelDeleteParticle;

    PanelController[,] panels;

    TextController questionText;
    TextController answerText;
    TextController levelText;
    ScoreTextController scoreText;

    ScrollUpButtonController scrollUpButton;
    MainBackgroundController mainBackground;

    List<int[]> clickedBlocks;
    public CameraShake shake;

    const int REMOVE_NONE = 0; // 消えない
    const int REMOVE_ANSWER_CORRECT = 1; // 正解でブロックが消える
    const int REMOVE_999_BONUS = 2; // 999ボーナス

    float life;
    float scrollSpeed;
    bool alertFlag = false;

    private AudioSource audioSource;
    public AudioClip bombSound;
    public AudioClip blockClickSound;
    public AudioClip blockDeleteSound;
    public AudioClip blockDelete999Sound;
    public AudioClip voice999Bonus;
    public AudioClip normalBGM;
    public AudioClip alertBGM;

    float soundVolume = 1f;

    void Start()
    {
        Data.ChangeStatus(Data.STATUS_INITIAL);

        SetInstances();
        ResetGame();
        PutPanels();
        MovePanelField(true);

        PlayBgm(normalBGM);

        Data.StatusToPlay();
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        if (Data.status == Data.STATUS_PANEL_DROP)
        {
            UpdateForPanelDrop();
        }

        if (Data.IsGamePlay())
        {
            UpdateForGamePlay(deltaTime);
        }
    }

    void UpdateForGamePlay(float deltaTime)
    {
        ReduceLifeByTime(deltaTime);

        if (IsGameOver())
        {
            ExecuteGameOver();
            return;
        }

        ChangeAlertStatus();

        MovePanelField();
    }

    void SetInstances()
    {
        panelObject = (GameObject)Resources.Load("Panel");
        panelField = GameObject.Find("PanelField").GetComponent<PanelFieldController>();
        panelDeleteParticle = (GameObject)Resources.Load("PanelDeleteParticle");

        questionText = GameObject.Find("QuestionText").GetComponent<TextController>();
        answerText = GameObject.Find("AnswerText").GetComponent<TextController>();
        scoreText = GameObject.Find("ScoreText").GetComponent<ScoreTextController>();
        levelText = GameObject.Find("LevelText").GetComponent<TextController>();

        scrollUpButton = GameObject.Find("ScrollUpButton").GetComponent<ScrollUpButtonController>();
        mainBackground = GameObject.Find("MainBackground").GetComponent<MainBackgroundController>();

        audioSource = GetComponent<AudioSource>();
    }

    void ResetGame()
    {
        alertFlag = false;
        panels = new PanelController[mapX, mapY];

        ResetLife();
        ResetClickedBlocks();

        score = 0;
        AddScore(0);

        deleteCount = 0;
        gameLevel = 1;
        SetGameLevelText();

        ResetDroppingPanelCount();
        ResetQuestion();
        SetScrollSpeed();
    }

    void PutPanels()
    {
        float forceNormalBlockY = mapY * Data.INITIAL_LIFE_RATE;

        for (var x = 0; x < mapX; x++)
        {
            for (var y = 0; y < mapY; y++)
            {
                bool forceNormalBlock = false;
                if (y >= forceNormalBlockY)
                {
                    forceNormalBlock = true;
                }

                CreateNewPanel(x, y, forceNormalBlock);
            }
        }
    }

    void CreateNewPanel(int x, int y, bool forceNormalBlock = false)
    {
        GameObject panel = PutPanel(x, y, forceNormalBlock);
        panels[x, y] = panel.GetComponent<PanelController>();
    }

    GameObject PutPanel(int x, int y, bool forceNormalBlock = false)
    {
        float posX = GetPanelXPosition(x);
        float posY = GetPanelYPosition(y);

        int blockKind;
        if (forceNormalBlock)
        {
            blockKind = Data.BLOCK_KIND_NORMAL;
        }
        else
        {
            blockKind = ChoiceBlockKind();
        }

        int number = ChoiceBlockNumber(blockKind);

        string objectName = "Panel_" + x.ToString() + "_" + y.ToString();

        GameObject panel = CreateObject(panelObject, posX, posY, objectName);
        panel.GetComponent<PanelController>().Initialze(this, number, blockKind, x, y);

        panel.transform.parent = panelField.gameObject.transform;

        return panel;
    }

    float GetPanelXPosition(int x)
    {
        return (x - Data.MAP_X_HALF + Data.PANEL_X_HALF_SIZE) * Data.PANEL_X_SIZE;
    }

    float GetPanelYPosition(int y)
    {
        return (y - Data.MAP_Y_HALF + Data.PANEL_Y_HALF_SIZE) * Data.PANEL_Y_SIZE;
    }

    private GameObject CreateObject(GameObject baseObject, float x, float y, string objectName = null, float scaleX = 1f, float scaleY = 1f, float rotation = 0f)
    {
        Quaternion rote = Quaternion.Euler(0f, 0f, rotation);
        GameObject instance = (GameObject)Instantiate(baseObject, new Vector2(x, y), rote);
        instance.transform.localScale = new Vector2(scaleX, scaleY);

        if (objectName != null)
        {
            instance.name = objectName;
        }

        return instance;
    }

    void ResetQuestion()
    {
        SetQuestion();
        ResetAnswer();
    }

    void SetQuestion()
    {
        question = Random.Range(Data.QUESTION_NUMBER_MIN, Data.QUESTION_NUMBER_MAX);

        questionText.SetText(question.ToString());
    }

    void ResetAnswer()
    {
        answer = 0;
        UpdateAnswerText();
    }

    void UpdateAnswerText()
    {
        string text = "";

        if (clickedBlocks.Count > 0)
        {
            text = answer.ToString();
        }

        SetAnswerText(text);
    }

    void SetAnswerText(string text)
    {
        answerText.SetText(text);
    }

    public void ClickPanel(PanelController panel)
    {
        if (panel.isClick)
        {
            // キャンセル
            ExecutePanelClickCancel(panel);
        }
        else
        {
            // クリック実行
            ExecutePanelClick(panel);
        }

        RefreshAnswer();

        int removeType = CheckBlockRemove();
        if (removeType == REMOVE_NONE)
        {
            PlaySound(blockClickSound);
        }
        else
        {
            StartPanelDrop();
            ExecuteAnswerCorrect(removeType);
        }
    }

    int CheckBlockRemove()
    {
        if (Is999Bonus())
        {
            return REMOVE_999_BONUS;
        }

        if (IsAnswerCorrect())
        {
            return REMOVE_ANSWER_CORRECT;
        }

        return REMOVE_NONE;
    }

    bool IsAnswerCorrect()
    {
        return answer == question;
    }

    void ExecuteAnswerCorrect(int removeType)
    {
        if (removeType == REMOVE_999_BONUS)
        {
            AddScore999Bonus();
            PlaySound(blockDelete999Sound);
            PlaySound(voice999Bonus);
        }
        else
        {
            AddScoreFromRemoveBlocks();
            PlaySound(blockDeleteSound);
        }

        RemoveUsedPanels();

        ResetClickedBlocks();

        DropAllPanels();

        AlignmentAllPanels();
    }

    void RemoveUsedPanels()
    {
        foreach (int[] position in clickedBlocks)
        {
            int x = position[0];
            int y = position[1];

            RemovePanel(x, y);
        }
    }

    void RemovePanel(int x, int y)
    {
        PanelController panel = panels[x, y];
        if (panel == null) {
            return;
        }

        if (!panel.isClick)
        {
            return;
        }

        panels[x, y] = null;
        panel.Destroy();

        IncreaseDeleteCount();

        CreatePanelDeleteParticle(x, y);
    }

    void DropAllPanels()
    {
        ResetDroppingPanelCount();

        for (var x = 0; x < mapX; x++)
        {
            DropYPanels(x);
        }
    }

    void DropYPanels(int x)
    {
        for (var y = 0; y < mapY - 1; y++)
        {
            PanelController panel = panels[x, y];
            if (panel != null)
            {
                continue;
            }

            // 空きスペースを詰める
            for (var y2 = y + 1; y2 < mapY; y2++)
            {
                PanelController newPanel = panels[x, y2];
                if (newPanel != null)
                {
                    newPanel.ChangeNumberByDrop();
                    droppingPanelCount++;

                    ReplacePanelPosition(newPanel, x, y, x, y2);
                    break;
                }
            }
        }
    }

    // 全てのパネルの位置を正しく並べ替える
    void AlignmentAllPanels(bool isImmediately = false)
    {
        for (var x = 0; x < mapX; x++)
        {
            for (var y = 0; y < mapY; y++)
            {
                AlignmentPanel(x, y, isImmediately);
            }
        }
    }

    void AlignmentPanel(int x, int y, bool isImmediately)
    {
        PanelController panel = panels[x, y];
        if (panel == null)
        {
            return;
        }

        float posX = GetPanelXPosition(x);
        float posY = GetPanelYPosition(y);

        if (isImmediately)
        {
            panel.MoveImmediately(posX, posY);
        }
        else
        {
            panel.MoveTo(posX, posY);
        }
    }

    void AddScoreWithGameLevel(int difference)
    {
        difference += (int)(difference * (gameLevel - 1) * Data.SCORE_PER_GAME_LEVEL);
        AddScore(difference);
    }

    void AddScore(int difference)
    {
        score += difference;
        scoreText.SetScore(score);
    }

    // まとめて消すほど、大きい数字を消すほどスコアは高くなる
    void AddScoreFromRemoveBlocks()
    {
        List<int> clickedNumbers = GetClickedBlocksNumbers();
        clickedNumbers.Sort();

        float tmp = 0;
        int i = 0;
        foreach (int v in clickedNumbers)
        {
            float rate = 1 + Data.SCORE_RANSA_RATE * i;
            tmp += Mathf.Abs(v) * rate;
            i++;
        }

        int value = (int)tmp;

        if (value > Data.BONUS_999_SCORE)
        {
            value = Data.BONUS_999_SCORE;
        }

        AddScoreWithGameLevel(value);
    }

    List<int> GetClickedBlocksNumbers()
    {
        List<int> numbers = new List<int>();
        foreach (int[] position in clickedBlocks)
        {
            PanelController panel = panels[position[0], position[1]];
            numbers.Add(panel.number);
        }

        return numbers;
    }

    bool IsGameOver()
    {
        if (Data.CHEAT_MUTEKI)
        {
            return false;
        }

        return life <= 0;
    }

    void RiseUpAllPanels()
    {
        for (var x = 0; x < mapX; x++)
        {
            // 1段引き上げて、一番下に新しいブロックを挟む
            RiseUpYPanels(x);
            CreateNewPanel(x, 0);
        }
    }

    void RiseUpYPanels(int x)
    {
        for (var y = mapY; y > 0; y--)
        {
            int y2 = y - 1;

            PanelController panel = panels[x, y2];
            if (panel == null)
            {
                continue;
            }

            ReplacePanelPosition(panel, x, y, x, y2);
        }
    }

    void MovePanelField(bool isImmediately = false)
    {

        float yPosition = GetYPositionFromLife();

        if (isImmediately)
        {
            panelField.MoveImmediately(0f, yPosition);
        }
        else
        {
            panelField.MoveTo(0f, yPosition);
        }
    }

    // life100でY=-10, life0でY=0になる
    float GetYPositionFromLife()
    {
        float leftLifeRate = life / Data.LIFE_MAX;
        float maxPosition = -10f;
        float minPosition = 0f;

        return (maxPosition - minPosition) * leftLifeRate;
    }

    void ExecuteGameOver()
    {
        Data.ChangeStatus(Data.STATUS_GAMEOVER);

        PlaySound(bombSound);
        shake.Shake(3f, 0.5f);

        Data.tmpScore = score;
        Data.UpdateHighScore(score);

        FadeManager.FadeOut("GameOverScene", 2f);
    }

    void ExecutePanelClick(PanelController panel)
    {
        AddClickedBlock(panel.fieldX, panel.fieldY);
        panel.ChangeStatusToClick();
    }

    void ExecutePanelClickCancel(PanelController panel)
    {
        RemoveClickedBlock(panel.fieldX, panel.fieldY);
        panel.ChangeStatusToCancel();
    }

    void AddClickedBlock(int x, int y)
    {
        int[] positions = new int[2] { x, y };
        clickedBlocks.Add(positions);
    }

    void RemoveClickedBlock(int x, int y)
    {
        int i = 0;
        foreach (int[] position in clickedBlocks)
        {
            if (position[0] == x && position[1] == y)
            {
                clickedBlocks.RemoveAt(i);
                return;
            }
            i++;
        }
    }

    // パネルの座標をずらす処理
    void ReplacePanelPosition(PanelController panel, int newX, int newY, int beforeX, int beforeY)
    {
        panel.SetFieldPosition(newX, newY);
        panels[newX, newY] = panel;
        panels[beforeX, beforeY] = null;

        if (panel.isClick)
        {
            RemoveClickedBlock(beforeX, beforeY);
            AddClickedBlock(newX, newY);
        }
    }

    // テスト用output
    void OutputClickedBlocks()
    {
        string text = "";
        foreach (int[] v in clickedBlocks)
        {
            text += v[0].ToString() + "_" + v[1].ToString();
            text += ",";
        }
        Debug.Log(text);
    }

    void ResetClickedBlocks()
    {
        clickedBlocks = new List<int[]>();
    }

    void RefreshAnswer()
    {
        int sum = 0;
        bool isNumberUpdated = false;
        int magnification = 1;

        foreach (int[] position in clickedBlocks)
        {
            PanelController panel = panels[position[0], position[1]];
            int number = panel.number;

            switch (panel.kind)
            {
                case Data.BLOCK_KIND_MINUS:
                    sum += number;
                    isNumberUpdated = true;
                    break;
                case Data.BLOCK_KIND_MAGNIFICATION:
                    magnification *= number;
                    break;
                case Data.BLOCK_KIND_BLANK:
                    break;
                default:
                    sum += number;
                    isNumberUpdated = true;
                    break;
            }

            if (magnification > 1 && isNumberUpdated)
            {
                sum *= magnification;
                magnification = 1;
            }
        }

        answer = sum;
        UpdateAnswerText();
    }

    bool Is999Bonus()
    {
        // 中身が999であることを確認する
        List<int> clickedNumbers = GetClickedBlocksNumbers();

        if (clickedNumbers.Count != Data.BONUS_999_COUNT)
        {
            return false;
        }

        foreach (int number in clickedNumbers)
        {
            if (number != Data.BONUS_999_NUMBER)
            {
                return false;
            }
        }

        // 座標が縦一列か横一列なことを確認する
        bool isLine = IsContinuousValuesLine();

        return isLine;
    }

    // 1列に連続した座標か？
    bool IsContinuousValuesLine()
    {
        List<int> allX = new List<int>();
        List<int> allY = new List<int>();

        foreach (int[] position in clickedBlocks)
        {
            allX.Add(position[0]);
            allY.Add(position[1]);
        }

        if (IsSameValues(allX) && IsContinuousValues(allY))
        {
            return true;
        }

        if (IsSameValues(allY) && IsContinuousValues(allX))
        {
            return true;
        }

        return false;
    }

    bool IsSameValues(List<int> values)
    {
        for (int i = 0; i < values.Count - 1; i++)
        {
            if (values[i] != values[i + 1])
            {
                return false;
            }
        }

        return true;
    }

    bool IsContinuousValues(List<int> values)
    {
        values.Sort();

        for (int i = 0; i < values.Count - 1; i++)
        {
            if (values[i] + 1 != values[i + 1])
            {
                return false;
            }
        }

        return true;
    }

    void AddScore999Bonus()
    {
        int value = Data.BONUS_999_SCORE;
        AddScoreWithGameLevel(value);

        float rate = Data.LIFE_UP_PER_BONUS_999;
        AddLife(value * rate);
    }

    int ChoiceBlockKind()
    {
        int rand = GetChoiceBlockKindRandomValue();

        if (rand <= Data.BLOCK_RATE_BLANK)
        {
            return Data.BLOCK_KIND_BLANK;
        }
        rand -= Data.BLOCK_RATE_BLANK;

        if (rand <= Data.BLOCK_RATE_MINUS)
        {
            return Data.BLOCK_KIND_MINUS;
        }
        rand -= Data.BLOCK_RATE_MINUS;

        if (rand <= Data.BLOCK_RATE_MAGNIFICATION)
        {
            return Data.BLOCK_KIND_MAGNIFICATION;
        }
        rand -= Data.BLOCK_RATE_MAGNIFICATION;

        return Data.BLOCK_KIND_NORMAL;
    }

    int GetChoiceBlockKindRandomValue()
    {
        int rangeMax = 100 + 1 - (gameLevel - 1) * Data.SPECIAL_BLOCK_RATE_PER_GAME_LEVEL;

        if (rangeMax < 50)
        {
            rangeMax = 50;
        }

        return Random.Range(1, rangeMax);
    }

    int ChoiceBlockNumber(int blockKind)
    {
        int number;

        switch (blockKind)
        {
            case Data.BLOCK_KIND_MINUS:
                number = ChoiceNomalBlockMinus();
                break;
            case Data.BLOCK_KIND_MAGNIFICATION:
                number = ChoiceNomalBlockMaginification();
                break;
            case Data.BLOCK_KIND_BLANK:
                number = 0;
                break;
            default:
                number = ChoiceNomalBlockNumber();
                break;
        }

        return number;
    }

    public int ChoiceNomalBlockNumber()
    {
        return Random.Range(Data.BLOCK_NUMBER_MIN, Data.BLOCK_NUMBER_MAX);
    }

    int ChoiceNomalBlockMinus()
    {
        int number = ChoiceNomalBlockNumber();
        return number * -1;
    }

    int ChoiceNomalBlockMaginification()
    {
        int rand = Random.Range(1, 100);

        if (rand <= Data.BLOCK_MAGNIFICATION_3_RATE)
        {
            return 3;
        }

        return 2;
    }

    void RiseUpAllPanelsContinuous()
    {
        int i = 0;

        while(true)
        {
            // 天井が全て開いてるか？
            if (!IsEmptyLine(mapY - 1))
            {
                break;
            }

            RiseUpAllPanels();

            // 1段だけ表示を下げる
            float value = Data.LIFE_MAX / mapY;
            AddLife(value);

            // 無限ループ防止
            i++;
            if (i > 100)
            {
                break;
            }
        };

        if (i > 0)
        {
            AlignmentAllPanels(true);
            MovePanelField(true);
        }
    }

    bool IsEmptyLine(int y)
    {
        for (var x = 0; x < mapX; x++)
        {
            PanelController panel = panels[x, y];
            if (panel != null)
            {
                return false;
            }
        }

        return true;
    }

    void ResetLife()
    {
        // 最初は中途半端に始まる
        life = (int)(Data.LIFE_MAX * Data.INITIAL_LIFE_RATE);
    }

    void ReduceLife(float difference)
    {
        ChangeLife(difference * -1);
    }

    void AddLife(float difference)
    {
        ChangeLife(difference);
    }

    void ChangeLife(float difference)
    {
        life += difference;

        if (life < 0)
        {
            life = 0;
        }

        if (life > Data.LIFE_MAX)
        {
            life = Data.LIFE_MAX;
        }
    }

    void ReduceLifeByTime(float deltaTime)
    {
        float speed;

        if (Input.GetKey(KeyCode.UpArrow) || scrollUpButton.IsClick())
        {
            speed = Data.BLOCK_SCROLL_HIGH_SPEED;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && Data.CHEAT_ENABLE)
        {
            speed = Data.BLOCK_SCROLL_HIGH_SPEED * -1;
        }
        else if (Data.CHEAT_BLOCK_STOP)
        {
            speed = 0f;
        } else
        {
            speed = GetScrollSpeed();
        }

        ReduceLife(speed * deltaTime);
    }

    float GetScrollSpeed()
    {
        float speed = scrollSpeed;

        float gameLevelRate = (gameLevel - 1) * Data.SPEED_UP_PER_GAME_LEVEL;
        speed += speed * gameLevelRate;

        if (alertFlag)
        {
            speed *= Data.ALERT_SCROLL_SPEED_RATE;
        }

        return speed;
    }

    void StartPanelDrop()
    {
        Data.ChangeStatus(Data.STATUS_PANEL_DROP);
        SetAnswerText("");
    }

    void ResetDroppingPanelCount()
    {
        droppingPanelCount = 0;
    }

    public void ReduceDroppingPanelCount()
    {
        droppingPanelCount--;
    }

    void UpdateForPanelDrop()
    {
        MovePanelField();

        // TODO: もしかしたら無限ループ防止が必要かもね
        if (droppingPanelCount > 0)
        {
            return;
        }

        FinishPanelDrop();
        Data.StatusToPlay();
    }

    void FinishPanelDrop()
    {
        RiseUpAllPanelsContinuous();
        ResetQuestion();
    }

    void SetScrollSpeed()
    {
        scrollSpeed = Data.BLOCK_SCROLL_SPEED;
    }

    void ChangeAlertStatus()
    {
        bool isAlert = IsAlertLife();

        if (alertFlag)
        {
            if (!isAlert)
            {
                StopAlert();
            }
        } else
        {
            if (isAlert)
            {
                StartAlert();
            }
        }
    }

    bool IsAlertLife()
    {
        return life <= (Data.LIFE_MAX * Data.ALERT_LIFE_BORDER);
    }

    void StartAlert()
    {
        alertFlag = true;
        PlayBgm(alertBGM);
        mainBackground.StartAlertAnimation();
    }

    void StopAlert()
    {
        alertFlag = false;
        PlayBgm(normalBGM);
        mainBackground.StopAlertAnimation();
    }

    void PlayBgm(AudioClip clip)
    {
        if (Data.IsBgmStop())
        {
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();
    }

    void PlaySound(AudioClip clip)
    {
        if (Data.IsBgmStop())
        {
            return;
        }

        audioSource.PlayOneShot(clip, soundVolume);
    }

    void CreatePanelDeleteParticle(int x, int y)
    {
        float posX = GetPanelXPosition(x);
        float posY = GetPanelYPosition(y);

        string name = "PanelDeleteParticle_" + x.ToString() + "_" + y.ToString();
        GameObject particle = CreateObject(panelDeleteParticle, posX, posY, name);

        particle.transform.parent = panelField.gameObject.transform;
        particle.GetComponent<PanelDeleteParticleController>().Initialze(posX, posY);
    }

    void IncreaseDeleteCount()
    {
        deleteCount++;
        gameLevel = GetGameLevel();
        SetGameLevelText();
    }

    int GetGameLevel()
    {
        return deleteCount / Data.GAME_LEVEL_PER_DELETE_COUNT + 1;
    }

    void SetGameLevelText()
    {
        levelText.SetText(gameLevel.ToString());
    }
}