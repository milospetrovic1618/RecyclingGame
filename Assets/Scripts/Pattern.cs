using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Pattern : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        //BOJAN: vec imas sracunat width i height ekrana preko BootMain? Koristi BootMain.Instance.viewWidth i viewHeight
        float curWidth = GetSpriteWidth(spriteRenderer);
        float curHeight = GetSpriteHeight(spriteRenderer);

        transform.localScale = new Vector2(transform.localScale.x * (BootMain.Instance.viewWidth / curWidth), transform.localScale.y * (BootMain.Instance.viewHeight / curHeight));
    }
    public float GetSpriteHeight(SpriteRenderer sr)
    {
        float spriteHeight = sr.sprite.bounds.size.y;
        float scaleY = sr.transform.lossyScale.y; // includes parent scaling
        return spriteHeight * scaleY;
    }
    public float GetSpriteWidth(SpriteRenderer sr)
    {
        float spriteWidth = sr.sprite.bounds.size.x;
        float scaleX = sr.transform.lossyScale.x; // includes parent scaling
        return spriteWidth * scaleX;
    }
}
