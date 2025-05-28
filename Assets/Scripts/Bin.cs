using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public enum RecyclingType
//kada dodajes novi trash ili recycle type u enum , treba se se doda i u gameplayData u trash_bin i bin_fullTrashList dictionary... i u BinsManager u switch GetBinFromRecyclingType
{
    Paper,
    PlasticMetal,
    ElectronicsBatteries,
    Glass,
    Organic,
    MatchNever
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
    public Coroutine movementCoroutine;
    public Coroutine coroutineWithCallback;
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
                SoundManager.Instance.PlaySFX(SoundManager.Instance.enterBin);
                TrashManager.Instance.AddNewPlayerInterval();
                GameplayManager.Instance.ScoreIncrease(binType);
                trashCount++;

                TrashManager.Instance.DeactivateTrash(trashItem);
                if (trashCount == maxTrashCount)
                {
                    Shake();
                }
            }
            else
            {
                trashItem.ReturnToJunkArea();
            }
        }
        else
        {
            trashItem.ReturnToJunkArea();
        }
    }
    public bool MatchesBin(Trash trashItem)
    {
        return trashItem.GetRecyclingType() == binType;
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
    public void ToggleOpenBin(bool toggle)
    {
        //change sprite
    }
    public void Appear(Vector2 appearFromPosition, Vector2 moveToPosition)
    {
        transform.position = appearFromPosition;
        MoveToPosition(moveToPosition, true, 1.2f);
    }
    public void Hide(Vector2 hidePosition)
    {
        MoveToPosition(hidePosition, false, 0.4f);
    }
    public void MoveToPosition(Vector2 target, bool toggleOpenBin, float moveDuration)
    {
        ToggleOpenBin(false);

        //isto treba i na kraju da se pozove turn of mora ovako jer kad se draguje u kantu a rigidBody se reaktivira objekat padne, umesto da se fiksira
        Action toggleOpenBinAction = () =>
        {
            ToggleOpenBin(toggleOpenBin);
        };

        IEnumerator enumerator = MovementsCoroutines.Instance.MoveToPosition(target, this.transform, moveDuration);
        AssignMovementCoroutine(enumerator, toggleOpenBinAction);

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
    public void Shake()
    {
        IEnumerator enumerator = MovementsCoroutines.Instance.ShakingIndefinitely(this.transform);
        AssignMovementCoroutine(enumerator);
    }
    public void StopShaking()
    {
        StopCoroutineMovement(); 

        //posto naglo prekines shaking coroutinu ali je pozicija moze da bude pomerena jer je shaking
        transform.position = BinsManager.Instance.visibleRowPositions[BinsManager.Instance.GetPositionIndex(this)];
    }

    public void AssignMovementCoroutine(IEnumerator enumerator, Action callback = null)
    {
        StopCoroutineMovement(); // Stop existing one
        movementCoroutine = callback == null
            ? StartCoroutine(enumerator)
            : StartCoroutine(RunCoroutineAndCallback(enumerator, callback));
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
}
