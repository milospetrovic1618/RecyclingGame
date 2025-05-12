using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public enum SortingLayer
{
    JunkArea,
    Trash
}
public enum Layer
{
    Trash,//trash ne iteraguje sa tresh
    BinsTrashColliders,//bins ne interaguje sa trash
    BinsDetectors,//interaguje sa tresh
    BinsSelectingColliders
}
public enum Scenes
{
    Menu,
    Gameplay,
    Boot
}
public class GameplayData: MonoBehaviour
{
    public static GameplayData Instance;
    //kada dodajes novi trash ili recycle type u enum , treba se se doda i u gameplayData u trash_bin i bin_trashList dictionary
    public static Dictionary<TrashType, RecyclingType> trash_bin = new Dictionary<TrashType, RecyclingType>()
    {
        { TrashType.CrumpledPaper, RecyclingType.Paper },
        { TrashType.Cardboard, RecyclingType.Paper },
        { TrashType.Newspaper, RecyclingType.Paper },

        { TrashType.Bottle, RecyclingType.Plastic },
        { TrashType.Cup, RecyclingType.Plastic },
        { TrashType.Straw, RecyclingType.Plastic },

        { TrashType.BottleCap, RecyclingType.Metal },
        { TrashType.FoodCan, RecyclingType.Metal },
        { TrashType.SodaCan, RecyclingType.Metal },

        { TrashType.Jar, RecyclingType.Glass },
        { TrashType.ClearBottle, RecyclingType.Glass },
        { TrashType.GreenBottle, RecyclingType.Glass },

        { TrashType.AppleCore, RecyclingType.Organic },
        { TrashType.BananaPeel, RecyclingType.Organic },
        { TrashType.StaleBread, RecyclingType.Organic }
    }; 

    public static Dictionary<RecyclingType, List<TrashType>> bin_trashList = new Dictionary<RecyclingType, List<TrashType>>()
    {
        { RecyclingType.Paper, new List<TrashType> { TrashType.CrumpledPaper, TrashType.Cardboard, TrashType.Newspaper } },
        { RecyclingType.Plastic, new List<TrashType> { TrashType.Bottle, TrashType.Cup, TrashType.Straw } },
        { RecyclingType.Metal, new List<TrashType> { TrashType.BottleCap, TrashType.FoodCan, TrashType.SodaCan } },
        { RecyclingType.Glass, new List<TrashType> { TrashType.Jar, TrashType.ClearBottle, TrashType.GreenBottle } },
        { RecyclingType.Organic, new List<TrashType> { TrashType.AppleCore, TrashType.BananaPeel, TrashType.StaleBread } }
    };

    public Text scoreText;

    public float viewWidth;
    public float viewRightX;
    public float viewLeftX;

    private void Start()
    {
        Instance = this; 

        scoreText = GameObject.Find("Score").GetComponent<Text>();

        Camera cam = BootGameplay.Instance.CameraInstantiated.GetComponent<Camera>();//mora camera instatiated da bi dobilo tacan width
        Vector2 screenBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector2 screenTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
        viewLeftX = screenBottomLeft.x;
        viewRightX = screenTopRight.x;
        viewWidth = screenTopRight.x - screenBottomLeft.x;
    }
}
