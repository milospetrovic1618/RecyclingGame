using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.EditorTools;
#endif
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
public class BootGameplay : MonoBehaviour//ima podatke koji su instancirni ovde pr camera
{
    public static BootGameplay Instance;
    //kante
    //junk Area
    //ui
    //gameplay script Holder
    public Text scoreText;
    public GameObject PauseUI;
    public GameObject PauseButton;
    public GameObject GameOverUI;
    public UnityEngine.UI.Image SoundToggle;
    public Sprite SoundOnTex;
    public Sprite SoundOffTex;

    public void Awake()
    {
        Instance = this;
        SoundManager.Instance.PlayBGM(SoundManager.Instance.gameplayBackgroundMusic);

        //BOJAN: ovo za SoundToggle on/off na awake bi bolje bilo napraviti izolovanu skriptu komponentu i kaciti gde treba
        //BOJAN: nego trpati po boot-evima copy/paste koda
        if (SoundManager.Instance.IsSilent())
        {
            SoundToggle.sprite = SoundOffTex;
        }
        else
        {
            SoundToggle.sprite = SoundOnTex;
        }
    }

    /* ne koristim
    public float wallThickness = 1f;
    public float zPosition = 0f;
    public void CreateWalls(camera cam)
    {
        Vector2 screenBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector2 screenTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        float width = screenTopRight.x - screenBottomLeft.x;
        float height = screenTopRight.y - screenBottomLeft.y;

        CreateWall(new Vector2(0, screenTopRight.y + wallThickness / 2), new Vector2(width, wallThickness)); // Top
        CreateWall(new Vector2(0, screenBottomLeft.y - wallThickness / 2), new Vector2(width, wallThickness)); // Bottom
        CreateWall(new Vector2(screenBottomLeft.x - wallThickness / 2, 0), new Vector2(wallThickness, height)); // Left
        CreateWall(new Vector2(screenTopRight.x + wallThickness / 2, 0), new Vector2(wallThickness, height)); // Right
    }
    void CreateWall(Vector2 position, Vector2 size)
    {
        GameObject wall = new GameObject("Wall");
        wall.transform.position = new Vector3(position.x, position.y, zPosition);

        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.size = size;

        wall.layer = LayerMask.NameToLayer("Default");
    }*/
}
