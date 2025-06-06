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
using TMPro;

public class BootGameplay : MonoBehaviour//ima podatke koji su instancirni ovde pr camera
{
    public static BootGameplay Instance;
    //kante
    //junk Area
    //ui
    //gameplay script Holder
    public GameObject CameraPrefab;
    public GameObject EventSystemPrefab;

    public GameObject scoreHolder;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreOutlineText;
    public GameObject PauseUI;
    public GameObject PauseButton;
    public GameObject GameOverUI;
    public UnityEngine.UI.Button ContinueButton;
    public GameObject Quiz;
    public TextMeshProUGUI Quiz_Question;
    public GameObject Quiz1;
    public TextMeshProUGUI Quiz1_Button1;
    public TextMeshProUGUI Quiz1_Button2;
    public TextMeshProUGUI Quiz1_Button3;
    public GameObject Quiz2;
    public TextMeshProUGUI Quiz2_Button1;
    public TextMeshProUGUI Quiz2_Button2;
    public UnityEngine.UI.Image SoundToggle;
    public Sprite SoundOnTex;
    public Sprite SoundOffTex;
    public GameObject CameraInstantiated;
    public GameObject EventSystemInstantiated;

    public void Awake()
    {
        Instance = this;
        SoundManager.Instance?.PlayBGM(SoundManager.Instance.gameplayBackgroundMusic);

        CameraInstantiated = Instantiate(CameraPrefab, new Vector3(0, 0, -1), Quaternion.identity);
        CameraInstantiated.tag = "MainCamera";

        EventSystemInstantiated = Instantiate(EventSystemPrefab, Vector3.zero, quaternion.identity);

        //BOJAN: ovo za SoundToggle on/off na awake bi bolje bilo napraviti izolovanu skriptu komponentu i kaciti gde treba
        //BOJAN: nego trpati po boot-evima copy/paste koda
        

        Material scoreOutlineTextMat = new Material(scoreOutlineText.font.material);
        scoreOutlineText.fontMaterial = scoreOutlineTextMat;

        //  outline  
        scoreOutlineText.outlineWidth = 0.9f;
        scoreOutlineText.outlineColor = scoreOutlineText.color;
        /*
        scoreOutlineText.EnableKeyword("UNDERLAY_ON");
        scoreOutlineText.SetColor("_UnderlayColor", Color.black);
        scoreOutlineText.SetFloat("_UnderlaySoftness", 0.5f);
        scoreOutlineText.SetFloat("_UnderlayOffsetX", 0f);
        scoreOutlineText.SetFloat("_UnderlayOffsetY", 0f);*/

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
