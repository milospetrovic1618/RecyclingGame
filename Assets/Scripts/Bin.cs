using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public Rigidbody2D rigidbody;
    private void Awake()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        collider = gameObject.AddComponent<PolygonCollider2D>();
        rigidbody = gameObject.AddComponent<Rigidbody2D>();
    }
    public bool MatchesBin(Trash trashItem)
    {
        return trashItem.GetRecyclingType() == binType;
    }
}
