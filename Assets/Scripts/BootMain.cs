using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.EditorTools;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Mathematics;

public enum Scenes
{
    Menu,
    Gameplay,
    Boot
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
    public Camera cam;
    public GameObject EventSystemInstantiated;
    public GameObject SoundManagerInstantiated;

    public float viewWidth;
    public float viewHeight;
    public float viewRightX;
    public float viewLeftX;
    public float viewBottomY;
    public float viewTopY;
    public void Awake()
    {
        Instance = this;

        List<GameObject> peristantGameObjects = new List<GameObject>();

        CameraInstantiated = Instantiate(CameraPrefab, new Vector3(0, 0, -1), Quaternion.identity);
        CameraInstantiated.tag = "MainCamera";
        cam = CameraInstantiated.GetComponent<Camera>();//mora camera instatiated da bi dobilo tacan width
        peristantGameObjects.Add(CameraInstantiated);

        EventSystemInstantiated = Instantiate(EventSystemPrefab, Vector3.zero, quaternion.identity);
        peristantGameObjects.Add(EventSystemInstantiated);

        SoundManagerInstantiated = new GameObject();
        soundManager = SoundManagerInstantiated.gameObject.AddComponent<SoundManager>();
        soundManager.Initialize(menuBackgroundMusic,gameplayBackgroundMusic,buttonClick, enterBin, SoundManagerInstantiated.AddComponent<AudioSource>(), SoundManagerInstantiated.AddComponent<AudioSource>());
        peristantGameObjects.Add(SoundManagerInstantiated);

        Vector2 viewBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector2 viewTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
        viewLeftX = viewBottomLeft.x;
        viewRightX = viewTopRight.x;
        viewBottomY = viewBottomLeft.y;
        viewTopY = viewTopRight.y;
        viewWidth = viewRightX - viewLeftX;
        viewHeight = viewBottomY - viewTopY;

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

    public void LoadSceneFromBoot(Scenes scene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene.ToString());
    }
}
