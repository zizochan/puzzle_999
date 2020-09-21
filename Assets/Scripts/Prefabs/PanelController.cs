using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    public int number;
    public bool isClick;

    public Sprite[] numberSprites;
    public Sprite[] numberMinusSprites;
    public Sprite[] numberMaginificationSprites;
    public Sprite numberBlankSprite;

    SpriteRenderer numberSpriteRenderer;
    SpriteRenderer backgroundSpriteRenderer;
    GameController gameController;

    float newXPosition;
    float newYPosition;
    float moveSpeed = Data.BLOCK_DROP_SPEED;
    bool moveFlag = false;

    public int fieldX;
    public int fieldY;
    public int kind;

    Color colorNormal = Color.white;
    Color colorBlank = new Color(1f, 1f, 1f, 0.5f);
    Color colorClick = Color.yellow;
    Color baseColor;

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        if (!Data.CanPanelMove())
        {
            return;
        }

        Move(deltaTime);
    }

    public void Initialze(GameController _gameController, int _number, int _kind, int x, int y)
    {
        gameController = _gameController;
        isClick = false;

        SetFieldPosition(x, y);
        SetInstances();

        InitialzeNumber(_number, _kind);
    }

    void InitialzeNumber(int _number, int _kind)
    {
        number = _number;
        kind = _kind;

        SetNumberSprite();
        SetBaseColor();
        ChangeToBaseColor();
    }

    void SetInstances()
    {
        numberSpriteRenderer = transform.Find("Number").gameObject.GetComponent<SpriteRenderer>();
        backgroundSpriteRenderer = transform.Find("Background").gameObject.GetComponent<SpriteRenderer>();
    }

    void SetNumberSprite()
    {
        int abs = Mathf.Abs(number);
        Sprite sprite;

        switch (kind)
        {
            case Data.BLOCK_KIND_MINUS:
                sprite = numberMinusSprites[abs];
                break;
            case Data.BLOCK_KIND_MAGNIFICATION:
                sprite = numberMaginificationSprites[abs];
                break;
            case Data.BLOCK_KIND_BLANK:
                sprite = numberBlankSprite;
                break;
            default:
                sprite = numberSprites[abs];
                break;
        }

        numberSpriteRenderer.sprite = sprite;
    }

    public void OnClick()
    {
        if (!Data.IsGamePlay())
        {
            return;
        }

        if (kind == Data.BLOCK_KIND_BLANK)
        {
            return;
        }

        gameController.ClickPanel(this);
    }

    public void ChangeStatusToClick()
    {
        isClick = true;
        backgroundSpriteRenderer.color = colorClick;
    }

    public void ChangeStatusToCancel()
    {
        isClick = false;
        ChangeToBaseColor();
    }

    void ChangeToBaseColor()
    {
        backgroundSpriteRenderer.color = baseColor;
    }

    public void MoveTo(float x, float y)
    {
        Vector2 pos = transform.localPosition;
        if (pos.x == x && pos.y == y)
        {
            return;
        }

        moveFlag = true;
        newXPosition = x;
        newYPosition = y;
    }

    void Move(float deltaTime)
    {
        if (!moveFlag)
        {
            return;
        }

        Vector2 pos = transform.localPosition;
        float speed = moveSpeed * deltaTime;

        pos.x = CalcPosition(pos.x, newXPosition, speed);
        pos.y = CalcPosition(pos.y, newYPosition, speed);

        transform.localPosition = pos;

        if (pos.x == newXPosition && pos.y == newYPosition)
        {
            moveFlag = false;
            gameController.ReduceDroppingPanelCount();
        }
    }

    public void MoveImmediately(float x, float y)
    {
        Vector2 pos = transform.localPosition;

        pos.x = x;
        pos.y = y;

        moveFlag = false;

        transform.localPosition = pos;
    }

    float CalcPosition(float currentPosition, float targetPosition, float speed)
    {
        if (targetPosition > currentPosition)
        {
            currentPosition += speed;
            if (currentPosition > targetPosition)
            {
                currentPosition = targetPosition;
            }
        }
        else if (targetPosition < currentPosition)
        {
            currentPosition -= speed;
            if (currentPosition < targetPosition)
            {
                currentPosition = targetPosition;
            }
        }

        return currentPosition;
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    // ドロップ時の数値変動
    public void ChangeNumberByDrop()
    {
        switch (kind)
        {
            case Data.BLOCK_KIND_NORMAL:
                AddNumber();
                break;
            case Data.BLOCK_KIND_MINUS:
                MinusNumber();
                break;
            case Data.BLOCK_KIND_BLANK:
                ChangeToNormalBlock();
                break;
        }
    }

    void AddNumber()
    {
        number++;

        if (number > Data.BLOCK_NUMBER_MAX)
        {
            number = Data.BLOCK_NUMBER_MAX;
        }

        SetNumberSprite();
    }

    void MinusNumber()
    {
        number--;

        if (number < Data.BLOCK_MINUS_NUMBER_MAX)
        {
            number = Data.BLOCK_MINUS_NUMBER_MAX;
        }

        SetNumberSprite();
    }

    public void SetFieldPosition(int x, int y)
    {
        fieldX = x;
        fieldY = y;
    }

    void SetBaseColor()
    {
        switch (kind)
        {
            case Data.BLOCK_KIND_BLANK:
                baseColor = colorBlank;
                break;
            default:
                baseColor = colorNormal;
                break;
        }
    }

    void ChangeToNormalBlock()
    {
        int newNumber = gameController.ChoiceNomalBlockNumber();
        InitialzeNumber(newNumber, Data.BLOCK_KIND_NORMAL);
    }
}
