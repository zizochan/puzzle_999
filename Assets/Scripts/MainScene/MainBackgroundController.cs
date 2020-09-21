using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBackgroundController : MonoBehaviour
{
    Animation alertAnimation;

    SpriteRenderer backgroundSprite;
    Color defaultColor;

    // Start is called before the first frame update
    void Start()
    {
        alertAnimation = GetComponent<Animation>();
        backgroundSprite = gameObject.GetComponent<SpriteRenderer>();

        defaultColor = backgroundSprite.color;
    }

    public void StartAlertAnimation()
    {
        alertAnimation.Play();
    }

    public void StopAlertAnimation()
    {
        if (!alertAnimation.isPlaying)
        {
            return;
        }

        alertAnimation.Stop();

        // 色の戻し方がわからないので無理やりリセットする
        ResetBackgroundColor();
    }

    void ResetBackgroundColor()
    {
        backgroundSprite.color = defaultColor;
    }
}
