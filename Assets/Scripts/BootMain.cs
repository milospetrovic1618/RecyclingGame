using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.EditorTools;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using TMPro;
using System;

public enum Scenes
{
    Menu,
    Gameplay,
    Boot,
    Test,
    Achievements,
    Options,
    Unlocked,
}
//redosled u executionOrder - prvo boot, pa Data, pa ostalo
//binsManager pre TrashManager  jer se koristi za random
public class BootMain : MonoBehaviour
{
    public static BootMain Instance;

    public GameObject CameraPrefab;
    public GameObject EventSystemPrefab;
    /*public AudioClip menuBackgroundMusic;
    public AudioClip gameplayBackgroundMusic;
    public AudioClip buttonClick;
    public AudioClip enterBin;

    public SoundManager soundManager;*/
    public GameObject CameraInstantiated;
    public GameObject EventSystemInstantiated;
    //public GameObject SoundManagerInstantiated;
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

        /*SoundManagerInstantiated = new GameObject();
        soundManager = SoundManagerInstantiated.gameObject.AddComponent<SoundManager>();
        soundManager.Initialize(menuBackgroundMusic,gameplayBackgroundMusic,buttonClick, enterBin, SoundManagerInstantiated.AddComponent<AudioSource>(), SoundManagerInstantiated.AddComponent<AudioSource>());
        peristantGameObjects.Add(SoundManagerInstantiated);*/

        SetBounds();

        SetPersistant(peristantGameObjects);

    }
    private void Start()
    {
        //wait for savesystem to initialize
        /*if (SaveSystem.Instance.Player == null)
        {
            StartCoroutine(NextFrameLoadScene(Scenes.Menu));
        }
        else
        {
            LoadSceneFromBoot(Scenes.Menu);
        }*/
    }
    private IEnumerator NextFrameLoadScene(Scenes scene)
    {
        yield return null;

        LoadSceneFromBoot(scene); 
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
        /* SAFE AREA - STVARA PROBLEM IMA RUPE NA EKRANU lastScreenWidth = Screen.width;
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
        viewHeight = viewTopY - viewBottomY;*/
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        Vector2 viewBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.nearClipPlane));
        Vector2 viewTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.nearClipPlane));

        viewLeftX = viewBottomLeft.x;
        viewRightX = viewTopRight.x;
        viewBottomY = viewBottomLeft.y;
        viewTopY = viewTopRight.y;
        viewWidth = viewRightX - viewLeftX;
        viewHeight = viewTopY - viewBottomY;
    }
    public bool blockingSpawn = true;
    public void LoadSceneFromBoot(Scenes scene)
    {
        Time.timeScale = 1f;
        StartCoroutine(UnlockSceneLogic(scene));//zato sto savesystem nema inicijalizovanog playera jos uvek



        SceneManager.LoadScene(scene.ToString());

        //CHATGPT will this code be called after awakes

    }
    private IEnumerator UnlockSceneLogic(Scenes scene)
    {
        yield return null; // Wait one frame, as your divine command requests
        Debug.Log("gggggg gggggg");
        switch (scene)
        {
            case Scenes.Gameplay:
                if (UnlockedChecked())
                {
                    StartCoroutine(SettingBlockSpawn());
                    SceneManager.LoadScene(Scenes.Unlocked.ToString(), LoadSceneMode.Additive);
                }
                break;

            case Scenes.Menu:
                if (UnlockedChecked())
                {
                    SceneManager.LoadScene(Scenes.Unlocked.ToString(), LoadSceneMode.Additive);
                }
                break;
        }
    }
    public IEnumerator SettingBlockSpawn()
    {
        while (blockingSpawn)
        {
            if (TrashManager.Instance != null)
                TrashManager.Instance.blockSpawn = true;
            yield return null;
        }
        blockingSpawn = true;
        if (TrashManager.Instance != null)
            TrashManager.Instance.blockSpawn = false;
    }
    public void LoadOptions()
    {
        SceneManager.LoadScene(Scenes.Options.ToString(), LoadSceneMode.Additive);
    }
    public void UnloadOptions()
    {
        SceneManager.UnloadSceneAsync(Scenes.Options.ToString());
    }

    public void UnloadUnlocked()
    {
        SceneManager.UnloadSceneAsync(Scenes.Unlocked.ToString());
    }
    public bool IsSceneLoaded(Scenes sceneEnum)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneEnum.ToString() && scene.isLoaded)
            {
                return true;
            }
        }
        return false;
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
    public List<AchievementData> unlocked;
    
    public bool UnlockedChecked()
    {
        unlocked = new List<AchievementData>();
        //UPDATE UNLOCKED GET, AND multipliers
        if (SaveSystem.Instance.Player.totalMultiplier != SaveSystem.Instance.Player.GetTotalMultiplier())
        {
            SaveSystem.Instance.Player.totalMultiplier = SaveSystem.Instance.Player.GetTotalMultiplier();
            unlocked.Add(new AchievementData("TotalCount"));
        }

        if (SaveSystem.Instance.Player.paperMultiplier != SaveSystem.Instance.Player.GetBinMultiplier(SaveSystem.Instance.Player.PaperCount))
        {
            SaveSystem.Instance.Player.paperMultiplier = SaveSystem.Instance.Player.GetBinMultiplier(SaveSystem.Instance.Player.PaperCount);
            unlocked.Add(new AchievementData("PaperCount"));
        }
        if (SaveSystem.Instance.Player.plasticMetalMultiplier != SaveSystem.Instance.Player.GetBinMultiplier(SaveSystem.Instance.Player.PlasticMetalCount))
        {
            SaveSystem.Instance.Player.plasticMetalMultiplier = SaveSystem.Instance.Player.GetBinMultiplier(SaveSystem.Instance.Player.PlasticMetalCount);
            unlocked.Add(new AchievementData("PlasticMetalCount"));
        }
        if (SaveSystem.Instance.Player.glassMultiplier != SaveSystem.Instance.Player.GetBinMultiplier(SaveSystem.Instance.Player.GlassCount))
        {
            SaveSystem.Instance.Player.glassMultiplier = SaveSystem.Instance.Player.GetBinMultiplier(SaveSystem.Instance.Player.GlassCount);
            unlocked.Add(new AchievementData("GlassCount"));
        }
        if (SaveSystem.Instance.Player.organicMultiplier != SaveSystem.Instance.Player.GetBinMultiplier(SaveSystem.Instance.Player.OrganicCount))
        {
            SaveSystem.Instance.Player.organicMultiplier = SaveSystem.Instance.Player.GetBinMultiplier(SaveSystem.Instance.Player.OrganicCount);
            unlocked.Add(new AchievementData("OrganicCount"));
        }
        if (SaveSystem.Instance.Player.electronicsBatteriesMultiplier != SaveSystem.Instance.Player.GetBinMultiplier(SaveSystem.Instance.Player.ElectronicsBatteriesCount))
        {
            SaveSystem.Instance.Player.electronicsBatteriesMultiplier = SaveSystem.Instance.Player.GetBinMultiplier(SaveSystem.Instance.Player.ElectronicsBatteriesCount);
            unlocked.Add(new AchievementData("ElectronicsBatteriesCount"));
        }


        if (SaveSystem.Instance.Player.lastRankCountChangeBins != SaveSystem.Instance.Player.GetRankCountChangeBins())
        {
            SaveSystem.Instance.Player.lastRankCountChangeBins = SaveSystem.Instance.Player.GetRankCountChangeBins();
            unlocked.Add(new AchievementData("CountChangeBins"));
        }
        if (SaveSystem.Instance.Player.lastRankTriviaHashset != SaveSystem.Instance.Player.GetRankTrivia())
        {
            SaveSystem.Instance.Player.lastRankTriviaHashset = SaveSystem.Instance.Player.GetRankTrivia();
            unlocked.Add(new AchievementData("Trivia"));
        }
        if (SaveSystem.Instance.Player.lastRankCatchTrashMidAir != SaveSystem.Instance.Player.GetRankCatchMidAir())
        {
            SaveSystem.Instance.Player.lastRankCatchTrashMidAir = SaveSystem.Instance.Player.GetRankCatchMidAir();
            unlocked.Add(new AchievementData("CatchTrashMidAir"));
        }

        SaveSystem.Instance.Flag(SaveSystem.Instance.Player);

        //test 
        /*unlocked.Clear();
        unlocked = new List<AchievementData> { 
            new AchievementData("CatchTrashMidAir"),
            new AchievementData("Trivia"),
            new AchievementData("OrganicCount"),
            new AchievementData("TotalCount")

        };*/

        unlocked.Reverse();
        Debug.Log("sssssssssss " + unlocked.Count);
        return unlocked.Count != 0;
    }
}
