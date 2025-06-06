using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OutlineFx;
using System;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;

public enum TrashType
//kada dodajes novi trash ili recycle type u enum , treba se se doda i u gameplayData u trash_bin i bin_fullTrashList dictionary
{
    Cardboard,
    Paper,
    PaperBag,
    Paper3,

    BottlePlastic,
    YogurtLogo,
    YogurtNoLogo,
    PlasticMetal3,

    BatteryAA,
    PhoneBroken,
    Electronics1,
    Electronics2,

    BottleClear,
    BottleGreen,
    BottleBrown,
    Glass1,

    AppleCore,
    BananaPeel,
    EggShells,
    Organic1
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
    public Coroutine coroutineWithCallback;
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
        List<TrashType> trashList = GameplayManager.bin_availableTrashList[randomBin];

        // Get a random TrashType from that list
        TrashType randomTrash = trashList[UnityEngine.Random.Range(0, trashList.Count)];

        return randomTrash;
    }
    public void Select()
    {
        if (!outline.enabled)
        {
            outline.enabled = true;
            transform.localScale = transform.localScale * 1.5f;
        }
    }

    public void DeSelect()
    {
        if (outline.enabled)
        {
            outline.enabled = false;
            transform.localScale = transform.localScale / 1.5f;
        }
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
        ToggleRigidBody(false);//zbog buga da kad djubre pada ima rigidbody pa aktivira onu kantu koja vraca nazad

        IEnumerator enumerator = MovementsCoroutines.Instance.CurveMoveFollow(GetBin().transform.Find("Rotator"), transform, new Vector2(0,BinsManager.Instance.binHeight + 0.5f));//GetBin().transform.Find("Rotator") ovo je velika greska?

        // zbog prikaza skora iznad kanti ne moze ovako GameplayManager.Instance.ScoreIncrease(GetRecyclingType());//mora ovde da ne bi pravilo bug
        TrashManager.Instance.RemoveFromList(this);//MORA DA SE IZBACI PRE DA NE PRAVI BUG
        //napravio si da kad lete prema kanti vec se podrazumeva kao poen i ne racuna za maksTrashCount (izbacio si ga iz liste)

        Action myAction = () =>
        {
            //GameplayManager.Instance.CurrentScore++;ovo sam izbacio
            this.gameObject.SetActive(false);
            ToggleRigidBody(false);
            TrashManager.Instance.deactivatedTrashObjectPooling.Enqueue(this);
        };

        AssignMovementCoroutine(enumerator, myAction);
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

        IEnumerator enumerator = MovementsCoroutines.Instance.MoveToPosition(target, this.transform, 0.3f);
        AssignMovementCoroutine(enumerator, toggleRigidBodyAction);

        //izbrisi iz curveMove da se rade stvari kao sto suu increase score nego da se to radi na kraju rutine
    }
    public Coroutine StartCoroutineWithCallback(IEnumerator enumerator, Action callback)
    {
        return StartCoroutine(RunCoroutineAndCallback(enumerator, callback)); ;
    }

    private IEnumerator RunCoroutineAndCallback(IEnumerator coroutine, Action callback)
    {
        coroutineWithCallback = StartCoroutine(coroutine); // Save reference to the actual routine
        yield return coroutineWithCallback;                // Wait for it to finish
        callback?.Invoke();                         // Then invoke callback
    }
    public void AssignMovementCoroutine(IEnumerator enumerator, Action callback = null)
    {
        StopCoroutineMovement(); // Stop existing one
        movementCoroutine = callback == null
            ? StartCoroutine(enumerator)
            : StartCoroutine(RunCoroutineAndCallback(enumerator, callback));
    }

    public void Throw(Vector2 target)
    {
        IEnumerator enumerator = MovementsCoroutines.Instance.Throw(target, this.transform);
        //Debug.Log("nooooonooooooooolapolizia");
        AssignMovementCoroutine(enumerator);
    }

    // Stops the movement and destroys the component
    public void StopCoroutineMovement()
    {
        if (movementCoroutine != null)//ako izbrises ovu proveru desice se zbrka
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }

        if (coroutineWithCallback != null)
        {
            StopCoroutine(coroutineWithCallback);
            coroutineWithCallback = null;
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
