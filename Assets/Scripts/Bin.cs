using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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
    public Coroutine movementCoroutine;
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
                GameplayManager.Instance.CurrentScore++;
                trashCount++;
                Destroy(trashItem.gameObject);
                TrashManager.Instance.trashList.Remove(trashItem);
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
        AssignMovementCoroutine(MovementsCoroutines.Instance.StartCoroutineWithCallback(enumerator, toggleOpenBinAction));

        //izbrisi iz curveMove da se rade stvari kao sto suu increase score nego da se to radi na kraju rutine
    }
    public void Shake()
    {
        IEnumerator enumerator = MovementsCoroutines.Instance.ShakingIndefinitely(this.transform);
        AssignMovementCoroutine(StartCoroutine(enumerator));
    }
    public void StopShaking()
    {
        StopCoroutineMovement(); 

        //posto naglo prekines shaking coroutinu ali je pozicija moze da bude pomerena jer je shaking
        transform.position = BinsManager.Instance.visibleRowPositions[BinsManager.Instance.GetPositionIndex(this)];
    }
    public void AssignMovementCoroutine(Coroutine newCoroutine) //hteo sam da stavim ove korutine direktno na trash ali ovako mi urednije i lakse
    {
        StopCoroutineMovement();//posto kod trash moze samo jedno pomeranje
        movementCoroutine = newCoroutine;
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
}
