using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OutlineFx;

public enum TrashType
//kada dodajes novi trash ili recycle type u enum , treba se se doda i u gameplayData u trash_bin i bin_trashList dictionary
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
    void OnEnable()//zbog object poolinga
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
        return DataGameplay.trash_bin[trashType];
    }

    public TrashType GetRandomTrashType()
    {
        /*List<TrashType> keys = new List<TrashType>(DataGameplay.trash_bin.Keys);

        int randomIndex = UnityEngine.Random.Range(0, keys.Count);

        //Debug.Log(keys[randomIndex].ToString());
        return keys[randomIndex];*/
        RecyclingType randomBin = BinsManager.Instance.availableBins[Random.Range(0, BinsManager.Instance.availableBins.Length)].binType;

        // Get the trash list for that RecyclingType
        List<TrashType> trashList = DataGameplay.bin_trashList[randomBin];

        // Get a random TrashType from that list
        TrashType randomTrash = trashList[Random.Range(0, trashList.Count)];

        return randomTrash;
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
            rigidbody = gameObject.AddComponent<Rigidbody2D>(); //ovde dobijas bug kad se izbrisu svi objekti sa ekrana a ti i dalje drzis objekat.. to ces da resis tako sto ces da proveris da li i dalje postoji unutar trash list... sto proveravas ali player selection
        }
        else
        {
            Destroy(rigidbody);
        }
        //rigidbody.AddForce(Vector2.right * 2f, ForceMode2D.Impulse);
    }
    
    //animations
    /*ove funkcije ne mogu da budu ovde jer se aktiviraju tek u sledecem frame-u kad se pozovu externo, a kad se u istom frame
    public void Throw()
    {
        gameObject.AddComponent<Throw>();
    }
    public void SendToPosition(Vector3 target)
    {
        SendToPosition sendTrashToPositionInstance = gameObject.AddComponent<SendToPosition>();
        sendTrashToPositionInstance.targetPosition = target;
    }*/
}
