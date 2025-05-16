using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OutlineFx;
using System;
using static UnityEngine.GraphicsBuffer;

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
    public Coroutine movementCoroutine;
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
            if (spriteRenderer.sprite == null)
            {
                Debug.Log(trashType);
            }

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
        return GameplayManager.trash_bin[trashType];
    }

    public TrashType GetRandomTrashType()
    {
        /*List<TrashType> keys = new List<TrashType>(GameplayManager.trash_bin.Keys);

        int randomIndex = UnityEngine.Random.Range(0, keys.Count);

        //Debug.Log(keys[randomIndex].ToString());
        return keys[randomIndex];*/
        RecyclingType randomBin = BinsManager.Instance.availableBins[UnityEngine.Random.Range(0, BinsManager.Instance.availableBins.Length)].binType;

        // Get the trash list for that RecyclingType
        List<TrashType> trashList = GameplayManager.bin_trashList[randomBin];

        // Get a random TrashType from that list
        TrashType randomTrash = trashList[UnityEngine.Random.Range(0, trashList.Count)];

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
    public void ReturnToJunkArea()
    {
        MoveToPosition(TrashManager.Instance.GetRandomPositionInJunkArea(), false);
    }
    public Bin GetBin()
    {
        return BinsManager.Instance.GetBinFromRecyclingType(GetRecyclingType());
    }
    public void FlyToBin()
    {
        IEnumerator enumerator = MovementsCoroutines.Instance.CurveMoveFollow(GetBin().transform, transform);
        Action myAction = () =>
        {
            GameplayManager.Instance.CurrentScore++;
            TrashManager.Instance.DeactivateTrash(this);
        };

        AssignMovementCoroutine(MovementsCoroutines.Instance.StartCoroutineWithCallback(enumerator, myAction));
        //izbrisi iz curveMove da se rade stvari kao sto suu increase score nego da se to radi na kraju rutine
    }
    public void MoveToPosition(Vector2 target, bool toggleRigidBody)
    {
        ToggleRigidBody(false);

        //isto treba i na kraju da se pozove turn of mora ovako jer kad se draguje u kantu a rigidBody se reaktivira objekat padne, umesto da se fiksira
        Action toggleRigidBodyAction = () =>
        {
            ToggleRigidBody(toggleRigidBody);
        };

        IEnumerator enumerator = MovementsCoroutines.Instance.MoveToPosition(target, this.transform, 0.5f);
        AssignMovementCoroutine(MovementsCoroutines.Instance.StartCoroutineWithCallback(enumerator, toggleRigidBodyAction));

        //izbrisi iz curveMove da se rade stvari kao sto suu increase score nego da se to radi na kraju rutine
    }
    public void AssignMovementCoroutine(Coroutine newCoroutine) //hteo sam da stavim ove korutine direktno na trash ali ovako mi urednije i lakse
    {
        StopCoroutineMovement();//posto kod trash moze samo jedno pomeranje
        movementCoroutine = newCoroutine;
    }

    public void Throw(Vector2 target)
    {
        IEnumerator enumerator = MovementsCoroutines.Instance.Throw(target, this.transform);
        //Debug.Log("nooooonooooooooolapolizia");
        AssignMovementCoroutine(StartCoroutine(enumerator));
    }

    // Stops the movement and destroys the component
    public void StopCoroutineMovement()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }
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
