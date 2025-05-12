using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RecyclingType
{
    Paper,
    Plastic,
    Metal,
    Glass,
    Organic
}

public class Bin : MonoBehaviour
{
    public RecyclingType binType;
    public SpriteRenderer spriteRenderer;
    public PolygonCollider2D collider;
    public OutlineFx.OutlineFx outline;
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
            AddScore(1);
        }
        else
        {
            AddScore(-1);
        }
        TrashSpawner.Instance.trashList.Remove(trashItem);
        Destroy(trashItem.gameObject);
    }
    public bool MatchesBin(Trash trashItem)
    {
        return trashItem.GetRecyclingType() == binType;
    }
    public void AddScore(int points)
    {
        //to do
        string scoreString = BootGameplay.Instance.scoreText.text;
        int score = int.Parse(scoreString);
        score += points;
        BootGameplay.Instance.scoreText.text = score.ToString();
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
