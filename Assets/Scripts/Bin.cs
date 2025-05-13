using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RecyclingType
//kada dodajes novi trash ili recycle type u enum , treba se se doda i u gameplayData u trash_bin i bin_trashList dictionary... i u BinsManager u switch GetBinFromRecyclingType
{
    Paper,
    Plastic,
    Metal,
    Glass,
    Organic
}

public class Bin : MonoBehaviour //za bin sam koristion prefab i prefab varijante jer je fiksan broj... a za trash sam direktno iz runtime-a dodavao komponente jer ih spwanujem i tako mi je prirodnije i lakse(brze odradim tipove u odnosu da pravim varijante), a bins vec postoje
{
    public RecyclingType binType;
    public SpriteRenderer spriteRenderer;
    public PolygonCollider2D collider;
    public OutlineFx.OutlineFx outline;
    public int trashCount = 0;
    [SerializeField]
    public int maxTrashCount = 5;
    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        collider = gameObject.GetComponent<PolygonCollider2D>();
        outline = gameObject.GetComponent<OutlineFx.OutlineFx>();
    }
    public void PutInBin(Trash trashItem)
    {
        if (MatchesBin(trashItem))
        {
            if (trashCount < maxTrashCount)
            {
                IncreaseScore();
                trashCount++;
                Destroy(trashItem.gameObject);
                TrashManager.Instance.trashList.Remove(trashItem);
                if (trashCount == maxTrashCount)
                {
                    //shaking
                }
            }
            else
            {
                ReturnTrash(trashItem);
            }
        }
        else
        {
            ReturnTrash(trashItem);
        }
    }
    public void ReturnTrash(Trash trashItem)
    {

        SendToPosition sendTrashToPositionInstance = trashItem.gameObject.AddComponent<SendToPosition>();
        sendTrashToPositionInstance.targetPosition = TrashManager.Instance.GetRandomPositionInJunkArea();
        trashItem.ToggleRigidBody(false);
    }
    public bool MatchesBin(Trash trashItem)
    {
        return trashItem.GetRecyclingType() == binType;
    }
    public void IncreaseScore()
    {
        //to do
        string scoreString = DataGameplay.Instance.scoreText.text;
        int score = int.Parse(scoreString);
        score ++;
        DataGameplay.Instance.scoreText.text = score.ToString();
    }
    public void Select()
    {
        outline.enabled = true;
        transform.localScale = transform.localScale * 1.15f;
    }

    public void DeSelect()
    {
        outline.enabled = false;
        transform.localScale = transform.localScale / 1.15f;
    }
}
