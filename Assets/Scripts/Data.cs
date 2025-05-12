using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
public static class Data
{
    public static Dictionary<TrashType, RecyclingType> trashItem_bin = new Dictionary<TrashType, RecyclingType>()
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
}
