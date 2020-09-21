using System.Collections.Generic;

public static class Data
{
    // other
    public static string VERSION = "1.2.0";

    // ゲーム設定
    public const string GAME_TITLE = "PUZZLE 999";
    public const int BLOCK_NUMBER_MIN = 1;
    public const int BLOCK_NUMBER_MAX = 9;
    public const int BLOCK_MINUS_NUMBER_MIN = BLOCK_NUMBER_MIN * -1;
    public const int BLOCK_MINUS_NUMBER_MAX = BLOCK_NUMBER_MAX * -1;
    public const int QUESTION_NUMBER_MIN = 11;
    public const int QUESTION_NUMBER_MAX = 20;
    public const int MAP_X = 6;
    public const int MAP_Y = 10;
    public const float PANEL_X_SIZE = 1f;
    public const float PANEL_Y_SIZE = 1f;
    public const int BONUS_999_NUMBER = 9;
    public const int BONUS_999_COUNT = 3;

    // スコア関連
    public const int BONUS_999_SCORE = 99;
    public const float LIFE_UP_PER_BONUS_999 = 0.25f;
    public const float SCORE_RANSA_RATE = 0.5f;

    // 速さ関連
    public const float BLOCK_SCROLL_SPEED = 0.9f;
    public const float BLOCK_SCROLL_HIGH_SPEED = 25f;
    public const float BLOCK_DROP_SPEED = 5f;
    public const float PANEL_FIELD_MOVE_SPEED = 5f;

    // ライフ関連
    public const float LIFE_MAX = 100f;
    public const float INITIAL_LIFE_RATE = 0.7f; // 開始時のライフ
    public const float ALERT_LIFE_BORDER = 0.15f;
    public const float ALERT_SCROLL_SPEED_RATE = 0.3f;

    // 計算用
    public const float MAP_X_HALF = MAP_X / 2f;
    public const float MAP_Y_HALF = MAP_Y / 2f;
    public const float PANEL_X_HALF_SIZE = PANEL_X_SIZE / 2;
    public const float PANEL_Y_HALF_SIZE = PANEL_Y_SIZE / 2;

    // フラグ管理
    public static int status;
    public const int STATUS_INITIAL = 0; // 初期状態
    public const int STATUS_PLAY = 10; // プレイ中
    public const int STATUS_PANEL_DROP = 20; // パネル落下中
    public const int STATUS_GAMEOVER = 99; // ゲームオーバー

    // ブロック種類
    public const int BLOCK_KIND_NORMAL = 0;
    public const int BLOCK_KIND_MINUS = 1;
    public const int BLOCK_KIND_MAGNIFICATION = 2;
    public const int BLOCK_KIND_BLANK = 3;

    // ブロック出現率
    public const int BLOCK_RATE_MINUS = 5;
    public const int BLOCK_RATE_MAGNIFICATION = 5;
    public const int BLOCK_RATE_BLANK = 10;
    public const int BLOCK_MAGNIFICATION_3_RATE = 20; // x3ブロックが出現する確率

    // スコア記録関連
    public static int tmpScore = 0; // シーン移動に使う
    public static int highScore = 0;
    public static bool isHighScoreUpdated = false;

    // ゲームレベル関連
    public const int GAME_LEVEL_PER_DELETE_COUNT = 10;
    public const float SPEED_UP_PER_GAME_LEVEL = 0.3f;
    public const float SCORE_PER_GAME_LEVEL = 0.3f;
    public const int SPECIAL_BLOCK_RATE_PER_GAME_LEVEL = 5;

    // チート
    public static bool CHEAT_ENABLE = false;  // 各種チートが使えるようになる
    public static bool CHEAT_MUTEKI = false;  // ゲームオーバーにならない
    public static bool CHEAT_BLOCK_STOP = false; // ブロックが自動で上昇しない

    // ゲーム設定
    public static bool CONFIG_SOUND_PLAY = true;

    static Data()
    {
        Start();
        ResetAllData();
    }

    // ゲーム開始時に一度だけ呼ばれる
    static void Start()
    {
        highScore = 0;
    }

    public static void ResetAllData()
    {
        tmpScore = 0;
        isHighScoreUpdated = false;

        ChangeStatus(STATUS_INITIAL);
    }

    public static void ChangeStatus(int value)
    {
        status = value;
    }

    public static void StatusToPlay()
    {
        ChangeStatus(STATUS_PLAY);
    }

    public static bool IsGamePlay()
    {
        return status == STATUS_PLAY;
    }

    public static bool CanPanelMove()
    {
        return status == STATUS_PLAY || status == STATUS_PANEL_DROP;
    }

    public static bool IsBgmStop()
    {
        return CONFIG_SOUND_PLAY == false;
    }

    public static void UpdateHighScore(int score)
    {
        if (score <= highScore)
        {
            return;
        }

        isHighScoreUpdated = true;
        highScore = score;
    }
}
