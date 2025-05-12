using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OutlineFx;

public enum TrashType
{
    CrumpledPaper,
    Cardboard,
    Newspaper,

    Bottle,
    Cup,
    Straw,

    BottleCap,
    FoodCan,
    SodaCan,

    Jar,
    ClearBottle,
    GreenBottle,

    AppleCore,
    BananaPeel,
    StaleBread
}


public class Trash : MonoBehaviour
{
    // Start is called before the first frame update
    public TrashType trashType;
    public SpriteRenderer spriteRenderer;
    public PolygonCollider2D collider;
    public Rigidbody2D rigidbody;
    public OutlineFx.OutlineFx outline;
    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer(Layer.Trash.ToString());
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = SortingLayer.Trash.ToString();
        collider = gameObject.AddComponent<PolygonCollider2D>();
        outline = gameObject.AddComponent<OutlineFx.OutlineFx>();
        //ToggleRigidBody(true);
        DeSelect();
    }
    void OnEnable()
    {
        Set(GetRandomTrashType());
    }
    public void Set(TrashType trashType)
    {
        if (this.trashType != trashType)
        {
            this.trashType = trashType;
            spriteRenderer.sprite = Resources.Load<Sprite>("Trash/" + trashType.ToString());

            /*PolygonCollider2D[] colliders = collider.GetComponents<PolygonCollider2D>();
            foreach (PolygonCollider2D collider in colliders)
            {
                UnityEngine.Object.Destroy(collider);
            }
            collider = gameObject.AddComponent<PolygonCollider2D>();*/

            UnityEngine.Object.Destroy(collider);
            collider = gameObject.AddComponent<PolygonCollider2D>();

            //posto menjam collider postoji mogucnost da ga rigidBody ne updatuje pa zbog toga
            if (rigidbody != null)
            {
                rigidbody.simulated = false;
                rigidbody.simulated = true;
            }
        }
    }
    public RecyclingType GetRecyclingType()
    {
        return Data.trashItem_bin[trashType];
    }

    public TrashType GetRandomTrashType()
    {
        List<TrashType> keys = new List<TrashType>(Data.trashItem_bin.Keys);

        int randomIndex = UnityEngine.Random.Range(0, keys.Count);

        Debug.Log(keys[randomIndex].ToString());
        return keys[randomIndex];
    }
    public void Select()
    {
        outline.enabled = true;
        transform.localScale = transform.localScale * 1.5f;
    }

    public void DeSelect()
    {
        outline.enabled = false;
        transform.localScale = transform.localScale / 1.5f;
    }
    public void ToggleRigidBody(bool active)//za rigidbody nema enable disable
    {
        if (active)
        { 
            rigidbody = gameObject?.AddComponent<Rigidbody2D>(); 
        }
        else
        {
            Destroy(rigidbody);
        }
        //rigidbody.AddForce(Vector2.right * 2f, ForceMode2D.Impulse);
    }

    /*
    public void LaunchTrashProjectile(Vector2 goal, float angleDegrees)
    {
        Vector2 start = this.transform.position;
        float gravity = Mathf.Abs(Physics2D.gravity.y);
        float angleRad = angleDegrees * Mathf.Deg2Rad;

        float distance = Vector2.Distance(start, goal);
        float dx = goal.x - start.x;
        float dy = goal.y - start.y;

        float cosAngle = Mathf.Cos(angleRad);
        float sinAngle = Mathf.Sin(angleRad);

        float speedSquared = (gravity * distance * distance) /
                             (2 * (dy - Mathf.Tan(angleRad) * dx) * cosAngle * cosAngle);

        if (speedSquared <= 0) return; // no valid solution

        float speed = Mathf.Sqrt(speedSquared);

        Vector2 dir = new Vector2(cosAngle, sinAngle).normalized;
        Vector2 velocity = dir * speed;

        rigidbody.velocity = velocity;
    }*/
}
