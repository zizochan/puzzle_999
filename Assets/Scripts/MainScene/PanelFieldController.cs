using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelFieldController : MonoBehaviour
{
    float newXPosition;
    float newYPosition;
    float moveSpeed = Data.PANEL_FIELD_MOVE_SPEED;
    bool moveFlag = false;

    void Start()
    {
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        if (!Data.CanPanelMove())
        {
            return;
        }

        Move(deltaTime);
    }

    // TODO: この辺Panelのコピペ。可能なら直す
    public void MoveTo(float x, float y)
    {
        Vector2 pos = transform.position;
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
}
