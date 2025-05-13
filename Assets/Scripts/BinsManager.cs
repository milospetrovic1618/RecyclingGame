using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BinsManager : MonoBehaviour
{
    public Bin PaperBin;
    public Bin GlassBin;
    public Bin PlasticBin;
    public Bin MetalBin;
    public Bin OrganicBin;
    public static BinsManager Instance;
    public Bin[] availableBins = new Bin[3];
    public int neededBins = 3;//za tutorijale treba 1 i 2

    private float availableBinPositionFromBottom = 0.4f;
    public Queue<Bin> binOrder = new Queue<Bin>();

    public Vector2[] visibleRowPositions = new Vector2[3];
    public Vector2[] hiddenRowPositions = new Vector2[3];
    public Vector2 notAvailableBinsPosition;

    void Start()
    {
        Instance = this;


        binOrder.Enqueue(GetBinFromRecyclingType(RecyclingType.Paper));
        binOrder.Enqueue(GetBinFromRecyclingType(RecyclingType.Glass));
        binOrder.Enqueue(GetBinFromRecyclingType(RecyclingType.Plastic));
        binOrder.Enqueue(GetBinFromRecyclingType(RecyclingType.Metal));
        binOrder.Enqueue(GetBinFromRecyclingType(RecyclingType.Organic));

        availableBins[0] = GetNextBin();
        availableBins[1] = GetNextBin();
        availableBins[2] = GetNextBin();


        float partWidth = DataGameplay.Instance.viewWidth / 4f;
        float firstX = DataGameplay.Instance.viewLeftX + partWidth;
        float secondX = DataGameplay.Instance.viewLeftX + partWidth * 2;
        float thirdX = DataGameplay.Instance.viewLeftX + partWidth * 3;
        Debug.Log("___ "+ partWidth + " " + DataGameplay.Instance.viewLeftX);

        float visibleRowY = DataGameplay.Instance.viewBottomY + 0.15f;//-0.15 je da je kanta malo iznad bottom-a ekrana
        float hiddenRowY = DataGameplay.Instance.viewBottomY - GetObjectHeight(PaperBin) - 0.15f;//-0.15 je da se osigura da se ne vidi

        visibleRowPositions = new Vector2[3]{ new Vector2(firstX, visibleRowY), new Vector2(secondX, visibleRowY),new Vector2(thirdX, visibleRowY)};
        hiddenRowPositions = new Vector2[3] { new Vector2(firstX, hiddenRowY), new Vector2(secondX, hiddenRowY), new Vector2(thirdX, hiddenRowY) };
        notAvailableBinsPosition = new Vector2(0, DataGameplay.Instance.viewBottomY - (GetObjectHeight(PaperBin) + 0.15f)*2.1f);

        SetInitialPositions();
        Debug.Log(GetObjectHeight(PaperBin) + " +++++++");
    }
    public void SetInitialPositions()
    {
        for(int i = 0; i < availableBins.Length; i++)
        {
            availableBins[i].transform.position = visibleRowPositions[i];
        }
        foreach (Bin bin in binOrder)
        {
            bin.transform.position = notAvailableBinsPosition;
        }
    }
    public Bin GetBinFromRecyclingType(RecyclingType recyclingType)
    {
        switch(recyclingType)
        {
            case RecyclingType.Paper: return PaperBin;
            case RecyclingType.Glass: return GlassBin;
            case RecyclingType.Plastic: return PlasticBin;
            case RecyclingType.Metal: return MetalBin;
            case RecyclingType.Organic: return OrganicBin;
            default:return null;
        }
    }
    public Bin GetNextBin()
    {
        return binOrder.Dequeue();
    }

    /*public void SetRandomBins()
    {
        List<RecyclingType> list = new List<RecyclingType>((RecyclingType[])System.Enum.GetValues(typeof(RecyclingType)));
       
        for (int i = 0, randomIndex; i < availableBins.Length;i++)
        {
            randomIndex = UnityEngine.Random.Range(0, list.Count);
            availableBins[i] = list[randomIndex];
            list.RemoveAt(randomIndex);
        }
    }*/
    public float GetObjectHeight(Bin bin)
    {
        SpriteRenderer sr = bin.gameObject.GetComponent<SpriteRenderer>();
        float spriteHeight = sr.sprite.bounds.size.y;
        float scaleY = bin.transform.lossyScale.y; // Includes parent scaling
        return spriteHeight * scaleY;
    }

    public void ReplaceBin(Bin bin)
    {
        int binIndex = 0;
        for(int i = 0; i < availableBins.Length;i++)
        {
            if (availableBins[i] == bin)
            {
                binIndex = i;
                break;
            }
        }
        SendToPosition sendToPositionInstance = bin.gameObject.AddComponent<SendToPosition>();
        sendToPositionInstance.targetPosition = hiddenRowPositions[binIndex];
        sendToPositionInstance.moveDuration = 0.5f;

        Bin nextBin = GetNextBin();
        nextBin.gameObject.transform.position = hiddenRowPositions[binIndex];

        SendToPosition sendToPositionInstance1 = nextBin.gameObject.AddComponent<SendToPosition>();
        sendToPositionInstance1.targetPosition = visibleRowPositions[binIndex];
        sendToPositionInstance1.moveDuration = 1.2f;

        availableBins[binIndex] = nextBin;
        binOrder.Enqueue(bin);

        bin.trashCount = 0;

        List<Trash> toRemove = new List<Trash>();
        foreach (Trash trash in TrashManager.Instance.trashList)
        {
            if (DataGameplay.trash_bin[trash.trashType] == bin.binType)
            {
                toRemove.Add(trash);
            }
        }
        foreach (Trash trash in toRemove)
        {
            TrashManager.Instance.trashList.Remove(trash);
            Destroy(trash.gameObject);
        }

        //ugasi shaking
    }
}
