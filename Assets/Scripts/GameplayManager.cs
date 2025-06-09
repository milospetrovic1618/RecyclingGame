using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public enum SortingLayer
{
    JunkArea,
    Trash,
    Bins,
    Score
}
public enum Layer
{
    Trash,//trash ne iteraguje sa tresh
    BinsTrashColliders,//bins ne interaguje sa trash
    BinsDetectors,//interaguje sa tresh
    BinsSelectingColliders
}
public class GameplayManager: MonoBehaviour//ima podatke i funkcije koje se koriste u Gameplay sceni, imas i BootGameplay ali on ima samo podatke koji za instancirane objekte zbog executionOrdera
{
    public static GameplayManager Instance;
    //kada dodajes novi trash ili recycle type u enum , treba se se doda i u gameplayData u trash_bin i bin_fullTrashList dictionary
    public static Dictionary<TrashType, RecyclingType> trash_bin = new Dictionary<TrashType, RecyclingType>()
    {// Paper
    { TrashType.Cardboard, RecyclingType.Paper },
    { TrashType.Paper, RecyclingType.Paper },
    { TrashType.PaperBag, RecyclingType.Paper },
    { TrashType.Paper3, RecyclingType.Paper },

    // Plastic & Metal
    { TrashType.BottlePlastic, RecyclingType.PlasticMetal },
    { TrashType.Yogurt, RecyclingType.PlasticMetal },
    { TrashType.FoodCan, RecyclingType.PlasticMetal },
    { TrashType.SodaCan, RecyclingType.PlasticMetal },

    // Electronics & Batteries
    { TrashType.BatteryAA, RecyclingType.ElectronicsBatteries },
    { TrashType.PhoneBroken, RecyclingType.ElectronicsBatteries },
    { TrashType.Charger, RecyclingType.ElectronicsBatteries },
    { TrashType.Earphones, RecyclingType.ElectronicsBatteries },

    // Glass
    { TrashType.BottleClear, RecyclingType.Glass },
    { TrashType.BottleGreen, RecyclingType.Glass },
    { TrashType.BottleBrown, RecyclingType.Glass },
    { TrashType.Jar, RecyclingType.Glass },

    // Organic
    { TrashType.AppleCore, RecyclingType.Organic },
    { TrashType.BananaPeel, RecyclingType.Organic },
    { TrashType.EggShells, RecyclingType.Organic },
    { TrashType.TeaBag, RecyclingType.Organic }
    }; 

    private static Dictionary<RecyclingType, List<TrashType>> bin_fullTrashList = new Dictionary<RecyclingType, List<TrashType>>()
    {{ RecyclingType.Paper, new List<TrashType>
        {
            TrashType.Cardboard,
            TrashType.Paper,
            TrashType.PaperBag,
            TrashType.Paper3
        }
    },

    { RecyclingType.PlasticMetal, new List<TrashType>
        {
            TrashType.BottlePlastic,
            TrashType.Yogurt,
            TrashType.FoodCan,
            TrashType.SodaCan
        }
    },

    { RecyclingType.ElectronicsBatteries, new List<TrashType>
        {
            TrashType.BatteryAA,
            TrashType.PhoneBroken,
            TrashType.Charger,
            TrashType.Earphones
        }
    },

    { RecyclingType.Glass, new List<TrashType>
        {
            TrashType.BottleClear,
            TrashType.BottleGreen,
            TrashType.BottleBrown,
            TrashType.Jar
        }
    },

    { RecyclingType.Organic, new List<TrashType>
        {
            TrashType.AppleCore,
            TrashType.BananaPeel,
            TrashType.EggShells,
            TrashType.TeaBag
        }
    },

    { RecyclingType.MatchNever, new List<TrashType>() }
    };

    public static Dictionary<RecyclingType, List<TrashType>> bin_availableTrashList = new Dictionary<RecyclingType, List<TrashType>>()
    {
        { RecyclingType.Paper, new List<TrashType>() },
        { RecyclingType.PlasticMetal, new List<TrashType>()  },
        { RecyclingType.ElectronicsBatteries,new List<TrashType>() },
        { RecyclingType.Glass, new List<TrashType>() },
        { RecyclingType.Organic,  new List<TrashType>() },
        { RecyclingType.MatchNever, new List<TrashType>()  }
    };
    public static bool alwaysAddTrashTypesInSameOrder = false;
    public static void AddNewTrashType()
    {
        foreach (RecyclingType recyclingType in bin_fullTrashList.Keys)
        {
            List<TrashType> fullTrashList = bin_fullTrashList[recyclingType];
            if (fullTrashList.Count != 0)
            {
                List<TrashType> availableTrashList = bin_availableTrashList[recyclingType];
                int index = fullTrashList.Count - 1;//nije sa pocetka nego sa kraja jer je efikasnije za liste removeAt()
                if (!alwaysAddTrashTypesInSameOrder)
                {
                    index = UnityEngine.Random.Range(0, fullTrashList.Count);//random index in fulltrashlist
                }

                if (fullTrashList.Count != 0)
                {
                    availableTrashList.Add(fullTrashList[index]);
                    fullTrashList.RemoveAt(index);
                }
            }
        }
    }

    public Action<float> onScoreChanged;     
    private float currentScore = 0;
    private float CurrentScore
    {
        get => currentScore;
        set
        {
            if (currentScore == value) return;
            currentScore = value;
            onScoreChanged?.Invoke(currentScore);
            if (currentScore>SaveSystem.Instance.Player.HighScore)
            {
                SaveSystem.Instance.Player.HighScore = currentScore;
            }
        }
    }
    public void ScoreIncrease(RecyclingType recyclingType)
    {
        CurrentScore += SaveSystem.Instance.Player.ScoreIncrease(recyclingType,1);
    }

    public void ScoreIncrease(RecyclingType recyclingType, int count)
    {
        CurrentScore += SaveSystem.Instance.Player.ScoreIncrease(recyclingType, count);
    }

    private void Awake()
    {
        Instance = this; 

        onScoreChanged = score => BootGameplay.Instance.scoreOutlineText.text = BootGameplay.Instance.scoreText.text = ((int)score).ToString();

        AddNewTrashType();
    }
}
