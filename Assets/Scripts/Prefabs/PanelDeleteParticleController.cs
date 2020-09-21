using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelDeleteParticleController : MonoBehaviour
{
    float pasteTime;
    const float DESTROY_LIMIT = 3f;

    // Start is called before the first frame update
    void Start()
    {
        pasteTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;

        CheckDestroy(deltaTime);
    }

    // 位置が合わないので無理やり合わせておく
    public void Initialze(float posX, float posY)
    {
        Vector2 pos = transform.localPosition;

        pos.x = posX;
        pos.y = posY;

        transform.localPosition = pos;
    }

    void CheckDestroy(float deltaTime)
    {
        pasteTime += deltaTime;
        if (pasteTime < DESTROY_LIMIT)
        {
            return;
        }

        Destroy(this.gameObject);
    }
}
