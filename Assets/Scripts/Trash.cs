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

    None,

    Cardboard,
    Paper,
    PaperBag,
    Paper3,

    BottlePlastic,
    Yogurt,
    FoodCan,
    SodaCan,

    BatteryAA,
    PhoneBroken,
    Charger,
    Earphones,

    BottleClear,
    BottleGreen,
    BottleBrown,
    Jar,

    AppleCore,
    BananaPeel,
    EggShells,
    TeaBag
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
    public Coroutine coroutineWithCallback; //vrv bi ti radilo i da imas StopAllCoroutines
    public string coroutineName;
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
    /*void On Enable()//zbog object poolinga
    {
        Set(GetRandomAvailableTrashType());
    }*/
    public void Set(TrashType trashType)
    {
        //spriteRenderer.sortingOrder = TrashManager.Instance.trashList.Count;//always on top
        //transform.SetAsLastSibling();//always on top
        DeSelect();
        ToggleRigidBody(false);
        if (this.trashType != trashType)
        {

            this.trashType = trashType;
            Sprite sprite = Resources.Load<Sprite>("Trash/" + trashType.ToString());
            spriteRenderer.sprite = sprite;

            if (sprite == null)
            {
                Debug.LogError(trashType);
            }
            if (spriteRenderer == null)
            {
                Debug.LogError("spriteRenderer null");
            }


            if (spriteRenderer.sprite == null || Resources.Load<Sprite>("Trash/" + trashType.ToString()) == null)
            {
                Debug.LogError(trashType);
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
    public bool IsInsideJunkArea()
    {
        return TrashManager.Instance.junkAreaRenderer.bounds.Contains(transform.position);
    }
    public void Select()
    {
        
        if (!outline.enabled)
        {

            spriteRenderer.sortingLayerName = SortingLayer.SelectedTrash.ToString();
            TrashManager.Instance.trashList.Remove(this);
            TrashManager.Instance.trashList.Add(this);//postavis ga na poslednje mesto
            TrashManager.Instance.UpdateSortingOrder();//bude na vrhu
                                                       //treba iznad if jer je moguce da naleti jos neko novo smece... ali ipak si resio ovo na drugaciji nacin, stavis na na drugi layer

            outline.enabled = true;
            transform.localScale = transform.localScale * 1.5f;

            if (coroutineName == "Throw")
            {
                StopCoroutine(movementCoroutine);//zaustavi throw ako se uhvati
                if (!TrashManager.Instance.initialSpawn)
                {
                    SaveSystem.Instance.Player.CatchTrashMidAir = true;
                }
            }
        }
    }

    public void DeSelect()
    {
        if (outline.enabled)
        {
            spriteRenderer.sortingLayerName = SortingLayer.Trash.ToString();
            outline.enabled = false;
            transform.localScale = transform.localScale / 1.5f;
        }
    }
    public void ToggleRigidBody(bool active)//za rigidbody nema enable disable
    {
        if (active)
        {
            if (gameObject.GetComponent<Rigidbody2D>() == null)
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
        

        Action myAction = () =>
        {
            //GameplayManager.Instance.CurrentScore++;ovo sam izbacio
            this.gameObject.SetActive(false);
            ToggleRigidBody(false);
            TrashManager.Instance.deactivatedTrashObjectPooling.Enqueue(this);
        };

        AssignMovementCoroutine(enumerator, myAction);
        //izbrisi iz curveMove da se rade stvari kao sto suu increase score nego da se to radi na kraju rutine
        TrashManager.Instance.RemoveFromList(this);//MORA DA SE IZBACI PRE DA NE PRAVI BUG
        //napravio si da kad lete prema kanti vec se podrazumeva kao poen i ne racuna za maksTrashCount (izbacio si ga iz liste)
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
        string rawName = enumerator.GetType().Name;
        coroutineName = rawName.Substring(rawName.IndexOf('<') + 1, rawName.IndexOf('>') - rawName.IndexOf('<') - 1); ; //r
        //Debug.Log(coroutineName);
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
        coroutineName = "";
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
