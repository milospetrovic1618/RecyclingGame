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
    public Queue<Bin> nextBinOrder = new Queue<Bin>();

    public Vector2[] visibleRowPositions = new Vector2[3];
    public Vector2[] appearFromPositions = new Vector2[3];
    public Vector2[] hidePositions = new Vector2[3];
    public Vector2 notAvailableBinsPosition;
    public float binHeight;
    public float binWidth;

    void Start()
    {
        Instance = this;

        binHeight = AssignBinHeight(PaperBin);//example paperbin
        binWidth = AssignBinWidth(PaperBin);

        nextBinOrder.Enqueue(GetBinFromRecyclingType(RecyclingType.Paper));
        nextBinOrder.Enqueue(GetBinFromRecyclingType(RecyclingType.Glass));
        nextBinOrder.Enqueue(GetBinFromRecyclingType(RecyclingType.Plastic));
        nextBinOrder.Enqueue(GetBinFromRecyclingType(RecyclingType.Metal));
        nextBinOrder.Enqueue(GetBinFromRecyclingType(RecyclingType.Organic));

        availableBins[0] = GetNextBin();
        availableBins[1] = GetNextBin();
        availableBins[2] = GetNextBin();


        float partWidth = DataGameplay.Instance.viewWidth / 3f;
        float firstX = DataGameplay.Instance.viewLeftX + partWidth * 0.7f;
        float secondX = DataGameplay.Instance.viewLeftX + partWidth * 1.5f;
        float thirdX = DataGameplay.Instance.viewLeftX + partWidth * 2.3f;
        Debug.Log("___ "+ partWidth + " " + DataGameplay.Instance.viewLeftX);

        float visibleRowY = DataGameplay.Instance.viewBottomY + 0.15f;//-0.15 je da je kanta malo iznad bottom-a ekrana
        float hiddenRowY = DataGameplay.Instance.viewBottomY - binHeight - 0.4f;//- 0.4 je da se osigura da se ne vidi, cak i kad ima outline

        visibleRowPositions = new Vector2[3]{ new Vector2(firstX, visibleRowY), new Vector2(secondX, visibleRowY),new Vector2(thirdX, visibleRowY)};

        //ako hoces jedno ispod drugo

        hidePositions = new Vector2[3] { new Vector2(firstX , hiddenRowY), new Vector2(secondX, hiddenRowY), new Vector2(thirdX, hiddenRowY) };
        appearFromPositions = new Vector2[3] { new Vector2(DataGameplay.Instance.viewLeftX - binWidth, visibleRowY), new Vector2(secondX, hiddenRowY), new Vector2(DataGameplay.Instance.viewRightX + binWidth, visibleRowY) };

        notAvailableBinsPosition = new Vector2(0, DataGameplay.Instance.viewBottomY - (AssignBinHeight(PaperBin) + 0.4f)*2.1f);

        SetInitialPositions();
        Debug.Log(AssignBinHeight(PaperBin) + " +++++++");
    }
    public void SetInitialPositions()
    {
        for(int i = 0; i < availableBins.Length; i++)
        {
            availableBins[i].transform.position = visibleRowPositions[i];
        }
        foreach (Bin bin in nextBinOrder)
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
        return nextBinOrder.Dequeue();
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
    public float AssignBinHeight(Bin bin)//ovde treba da zamenis da nema argumenti i da se stavlja reference sprite, ali ako imas i onaj otvorena kanta... i samo jednom iskoristi funkciju da assignujes value
    {
        SpriteRenderer sr = bin.gameObject.GetComponent<SpriteRenderer>();
        float spriteHeight = sr.sprite.bounds.size.y;
        float scaleY = bin.transform.lossyScale.y; // Includes parent scaling
        return spriteHeight * scaleY;
    }
    public float AssignBinWidth(Bin bin)//ovde treba da zamenis da nema argumenti i da se stavlja reference sprite, ali ako imas i onaj otvorena kanta... i samo jednom iskoristi funkciju da assignujes value
    {
        SpriteRenderer sr = bin.gameObject.GetComponent<SpriteRenderer>();
        float spriteWidth = sr.sprite.bounds.size.x;
        float scaleX = bin.transform.lossyScale.x; // Includes parent scaling
        return spriteWidth * scaleX;
    }

    public int GetPositionIndex(Bin bin)//ovaj index odgovara iz hide i za appear i za appear from... ovaj indeks je taj koji ih povezuje, on odredjuje i odakle se pojavljuje i gde se krije, i gde treba da se pojavi
    {
        for (int i = 0; i < availableBins.Length; i++)
        {
            if (availableBins[i] == bin)
            {
                return i;
            }
        }
        return -1;
    }
    public void ReplaceBin(Bin bin)
    {
        int positionIndex = GetPositionIndex(bin);

        bin.StopShaking();
        bin.Hide(hidePositions[positionIndex]);

        Bin nextBin = GetNextBin();
        nextBin.Appear(appearFromPositions[positionIndex], visibleRowPositions[positionIndex]);

        availableBins[positionIndex] = nextBin;
        nextBinOrder.Enqueue(bin);

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
            trash.FlyToBin();
        }

        //ugasi shaking
    }
}
