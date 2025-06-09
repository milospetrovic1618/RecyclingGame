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
    public Bin MissedBottom;
    public Bin MissedRight;
    public Bin MissedLeft;
    public static BinsManager Instance;
    public Bin[] availableBins = new Bin[3];
    public int neededBins = 3;//za tutorijale treba 1 i 2

    public int ReplaceCountForTutorial = 0;

    //private float availableBinPositionFromBottom = 0.4f;
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

        


        float partWidth = BootMain.Instance.viewWidth / 3f;
        float firstX = BootMain.Instance.viewLeftX + partWidth * 0.6f;
        float secondX = BootMain.Instance.viewLeftX + partWidth * 1.5f;
        float thirdX = BootMain.Instance.viewLeftX + partWidth * 2.4f;
        Debug.Log("___ "+ partWidth + " " + BootMain.Instance.viewLeftX);

        float visibleRowY = BootMain.Instance.viewBottomY + 0.15f;//-0.15 je da je kanta malo iznad bottom-a ekrana
        float hiddenRowY = BootMain.Instance.viewBottomY - binHeight - 0.4f;//- 0.4 je da se osigura da se ne vidi, cak i kad ima outline

        visibleRowPositions = new Vector2[3]{ new Vector2(firstX, visibleRowY), new Vector2(secondX, visibleRowY),new Vector2(thirdX, visibleRowY)};

        //ako hoces jedno ispod drugo

        hidePositions = new Vector2[3] { new Vector2(firstX , hiddenRowY), new Vector2(secondX, hiddenRowY), new Vector2(thirdX, hiddenRowY) };
        appearFromPositions = new Vector2[3] { new Vector2(BootMain.Instance.viewLeftX - binWidth, visibleRowY), new Vector2(secondX, hiddenRowY), new Vector2(BootMain.Instance.viewRightX + binWidth, visibleRowY) };

        notAvailableBinsPosition = new Vector2(0, BootMain.Instance.viewBottomY - (binHeight + 0.4f)*2.1f);

        MissedBottom.transform.localScale = new Vector2(3* BootMain.Instance.viewWidth, 1);//3 * da se osigura da je dovoljno dugo
        MissedLeft.transform.localScale = new Vector2(1, 3 * BootMain.Instance.viewHeight);
        MissedRight.transform.localScale = new Vector2(1, 3 * BootMain.Instance.viewHeight);

        SetInitial();
        //Debug.Log(AssignBinHeight(PaperBin) + " +++++++");
    }
    public void SetInitial()//Positions i order
    {
        nextBinOrder.Clear();
        nextBinOrder.Enqueue(GetBinFromRecyclingType(RecyclingType.Paper));
        nextBinOrder.Enqueue(GetBinFromRecyclingType(RecyclingType.Glass));
        nextBinOrder.Enqueue(GetBinFromRecyclingType(RecyclingType.PlasticMetal));
        nextBinOrder.Enqueue(GetBinFromRecyclingType(RecyclingType.ElectronicsBatteries));
        nextBinOrder.Enqueue(GetBinFromRecyclingType(RecyclingType.Organic));

        availableBins[0] = GetNextBin();
        availableBins[1] = GetNextBin();
        availableBins[2] = GetNextBin();

        for (int i = 0; i < availableBins.Length; i++)
        {
            availableBins[i].transform.position = visibleRowPositions[i];
        }
        foreach (Bin bin in nextBinOrder)
        {
            bin.transform.position = notAvailableBinsPosition;
        }

        MissedBottom.transform.position = visibleRowPositions[1];//stavi ga na sredinu
        MissedLeft.transform.position = new Vector2(BootMain.Instance.viewLeftX + TrashManager.offset, 0);
        MissedRight.transform.position = new Vector2(BootMain.Instance.viewRightX - TrashManager.offset, 0);
    }
    public Bin GetBinFromRecyclingType(RecyclingType recyclingType)
    {
        switch(recyclingType)
        {
            case RecyclingType.Paper: return PaperBin;
            case RecyclingType.Glass: return GlassBin;
            case RecyclingType.PlasticMetal: return PlasticBin;
            case RecyclingType.ElectronicsBatteries: return MetalBin;
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
            if (GameplayManager.trash_bin[trash.trashType] == bin.binType)
            {
                toRemove.Add(trash);
            }
        }
        int count = 0;
        foreach (Trash trash in toRemove)
        {
            count++;
            trash.FlyToBin();
        }
        if (count != 0) GameplayManager.Instance.ScoreIncrease(bin.binType, count);

        SaveSystem.Instance.Player.CountChangeBins++;

        if (!SaveSystem.Instance.Player.TutorialFinished)
        {
            ReplaceCountForTutorial++;
            
            if (ReplaceCountForTutorial > 2)
            {

            } else if (ReplaceCountForTutorial == 2)
            {
                TrashManager.Instance.SpawnAllTrashOfType(nextBin.binType);
                TrashManager.Instance.SpawnAllTrashOfType(nextBin.binType);
            }
            else 
            {
                TrashManager.Instance.SpawnAllTrashOfType(nextBin.binType);
                TrashManager.Instance.SpawnAllTrashOfType(availableBins[UnityEngine.Random.Range(0, availableBins.Length)].binType);
            }
        }

        //ugasi shaking
    }

}
