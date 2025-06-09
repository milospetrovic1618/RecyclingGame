using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.EditorTools;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using TMPro;

public enum Scenes
{
    Menu,
    Gameplay,
    Boot,
    Test,
    Achievements,
    Options,
}
//redosled u executionOrder - prvo boot, pa Data, pa ostalo
//binsManager pre TrashManager  jer se koristi za random
public class BootMain : MonoBehaviour
{
    public static BootMain Instance;

    public GameObject CameraPrefab;
    public GameObject EventSystemPrefab;
    public AudioClip menuBackgroundMusic;
    public AudioClip gameplayBackgroundMusic;
    public AudioClip buttonClick;
    public AudioClip enterBin;

    public SoundManager soundManager;
    public GameObject CameraInstantiated;
    public GameObject EventSystemInstantiated;
    public GameObject SoundManagerInstantiated;
    public Camera cam;

    private int lastScreenWidth;
    private int lastScreenHeight;

    public float viewWidth;
    public float viewHeight;
    public float viewRightX;
    public float viewLeftX;
    public float viewBottomY;
    public float viewTopY;

    public TMP_FontAsset LuckiestGuy;
    public void Awake()
    {
        Instance = this;

        List<GameObject> peristantGameObjects = new List<GameObject>();

        CameraInstantiated = Instantiate(CameraPrefab, new Vector3(0, 0, -1), Quaternion.identity);
        CameraInstantiated.tag = "MainCamera";
        cam = CameraInstantiated.GetComponent<Camera>();//mora camera instatiated da bi dobilo tacan width

        EventSystemInstantiated = Instantiate(EventSystemPrefab, Vector3.zero, quaternion.identity);

        SoundManagerInstantiated = new GameObject();
        soundManager = SoundManagerInstantiated.gameObject.AddComponent<SoundManager>();
        soundManager.Initialize(menuBackgroundMusic,gameplayBackgroundMusic,buttonClick, enterBin, SoundManagerInstantiated.AddComponent<AudioSource>(), SoundManagerInstantiated.AddComponent<AudioSource>());
        peristantGameObjects.Add(SoundManagerInstantiated);

        SetBounds();

        SetPersistant(peristantGameObjects);

        LoadSceneFromBoot(Scenes.Menu);
    }
    public void SetPersistant(List<GameObject> list)//reference ne cine da objekti opstanu, ali ako su child opstace i izmedju scena ako je parent DontDestroyOnLoad
    {
        foreach (GameObject obj in list)
        {
            obj.transform.parent = this.transform;
            //DontDestroyOnLoad(obj);
        }
        DontDestroyOnLoad(this);
    }
    public void SetBounds()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        Rect safeArea = Screen.safeArea;
        Vector2 bottomLeftNormalized = new Vector2(
            safeArea.xMin / Screen.width,
            safeArea.yMin / Screen.height
        );
        Vector2 topRightNormalized = new Vector2(
            safeArea.xMax / Screen.width,
            safeArea.yMax / Screen.height
        );

        Vector2 viewBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(bottomLeftNormalized.x, bottomLeftNormalized.y, Camera.main.nearClipPlane));
        Vector2 viewTopRight = Camera.main.ViewportToWorldPoint(new Vector3(topRightNormalized.x, topRightNormalized.y, Camera.main.nearClipPlane));

        viewLeftX = viewBottomLeft.x;
        viewRightX = viewTopRight.x;
        viewBottomY = viewBottomLeft.y;
        viewTopY = viewTopRight.y;
        viewWidth = viewRightX - viewLeftX;
        viewHeight = viewTopY - viewBottomY;
    }

    public void LoadSceneFromBoot(Scenes scene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene.ToString());
    }

    void Update()
    {
        //BOJAN: za mobile bi bilo dobro da se racuna Screen.safeArea zbog notch-eva (kad su prednja kamera i dugmici deo ekrana)
        //BOJAN: za mobile ce uglavnom biti fiksno width i height (mada noviji telefoni mogu i ovo da poremete)
        //BOJAN: ali za PC/browser ce igraci moci da menjaju (pre svega resize browsera...)
        //BOJAN: pa ne bi bilo lose u neki Update ubaciti da proverava da li je doslo do promene
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            SetBounds();
        }
    }
}
