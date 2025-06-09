using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTap : MonoBehaviour
{
    public static TutorialTap Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    public UnityEngine.UI.Image show;
    public TextMeshProUGUI tutorialFinishedText;
    public Sprite tap;
    public Sprite release;//postavi da im pivot bude bas na vrhu prsta
    public Coroutine movementCoroutine;
    // Update is called once per frame
    public float noPressTime = 0f;
    public float activationTime = 1f;
    bool enabled = false;
    private void Start()
    {
        Disable();
    }
    void Update()
    {
        //citaj stvari iz playerSelection kao sto je press
        if (noPressTime > activationTime && !enabled)
        {
            Enable();
        }
        noPressTime += Time.deltaTime;
        if (PlayerSelection.Instance.press)
        {
            noPressTime = 0f;
            Disable();
        }


        /*pressTime += Time.deltaTime;
        //citaj stvari iz playerSelection kao sto je press
        if (pressTime > deactivationTime)
        {
            if (enabled) Disable();
        }

        if (!PlayerSelection.Instance.press && !enabled)
        {
            pressTime = 0f;
            Enable();
        }*/
    }
    public void Disable()
    {
        enabled = false;
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }
        show.color = new Color(1f, 1f, 1f, 0f);
    }
    public void Enable()//enable se uvek zove na kraju coroutine, zove se isto kad se nista ne pipa 1 sekundu
    {
        enabled = true;
        if (!GuideEmptyBin())
        {
            GuideTrash();
        }
    }
    public void GuideTrash()
    {
        /*Trash lowestTrash = null;//pocetna pozicija
        float minY = float.MaxValue;
        foreach (Trash trash in TrashManager.Instance.trashList)//spriteRenderer.sortingOrder
        {
            if (trash.transform.position.y < minY && trash.IsInsideJunkArea())//string.IsNullOrEmpty(trash.coroutineName))//OVO ZNACI DA SE NE POMERA string.IsNullOrEmpty(trash.coroutineName)
            {
                minY = trash.transform.position.y;
                lowestTrash = trash;
            }
        }
        if (lowestTrash != null)
        {
            Bin bin = lowestTrash.GetBin();
            movementCoroutine = StartCoroutine(AnimateGuideTrash(lowestTrash,bin));
        }*/
        /*Trash trashOnTop = null;//pocetna pozicija
        float maxSortingOrder = float.MinValue;
        foreach (Trash trash in TrashManager.Instance.trashList)//spriteRenderer.sortingOrder
        {
            if (trash.spriteRenderer.sortingOrder > maxSortingOrder && trash.IsInsideJunkArea())//string.IsNullOrEmpty(trash.coroutineName))//OVO ZNACI DA SE NE POMERA string.IsNullOrEmpty(trash.coroutineName)
            {
                maxSortingOrder = trash.spriteRenderer.sortingOrder;
                trashOnTop = trash;
            }
        }*/
        Trash trashOnTop = null;
        for (int i = TrashManager.Instance.trashList.Count - 1; i >= 0; i--)
        {
            Trash trash = TrashManager.Instance.trashList[i];

            if (trash.IsInsideJunkArea())
            {
                trashOnTop = trash;
                break;
            }
        }
        if (trashOnTop != null)
        {
            Bin bin = trashOnTop.GetBin();
            movementCoroutine = StartCoroutine(AnimateGuideTrash(trashOnTop, bin));
        }
    }
    public bool GuideEmptyBin()
    {
        foreach(Bin bin in BinsManager.Instance.availableBins)
        {
            if (bin.trashCount >= Bin.maxTrashCount)//moguce je da bude samo ==
            {
                //AnimateGuideEmptyBin
                movementCoroutine = StartCoroutine(AnimateGuideEmptyBin(bin));
                return true;
            }
        }
        return false;
    }
    public IEnumerator AnimateGuideTrash(Trash trash, Bin bin)
    {
        show.color = new Color(1f, 1f, 1f, 1f);
        Vector2 worldStart = trash.transform.position;
        Vector2 worldEnd = bin.transform.position + new Vector3(0f, BinsManager.Instance.binHeight/2, 0f);


        Vector2 startPosition = Camera.main.WorldToScreenPoint(worldStart);
        Vector2 endPosition = Camera.main.WorldToScreenPoint(worldEnd);
        RectTransform rectTransform = show.rectTransform;
        rectTransform.position = startPosition;

        float releaseDuration = 0.4f;
        show.sprite = release;
        yield return new WaitForSeconds(releaseDuration);
        show.sprite = tap;
        yield return new WaitForSeconds(releaseDuration);

        float moveDuration = 1.2f;
        float timer = 0f;
        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / moveDuration);
            rectTransform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        float tapDuration = 0.6f;
        show.sprite = release;
        yield return new WaitForSeconds(tapDuration);
        Enable();
    }
    public IEnumerator AnimateGuideEmptyBin(Bin bin)
    {
        show.color = new Color(1f, 1f, 1f, 1f);
        show.rectTransform.position = Camera.main.WorldToScreenPoint(bin.transform.position + new Vector3 (0f, BinsManager.Instance.binHeight / 2, 0f));

        float releasePhaseDuration = 0.3f;
        float tapPhaseDuration = 0.5f;
        float disappearPhaseDuration = 0.3f;
        show.sprite = release;
        yield return new WaitForSeconds(releasePhaseDuration);
        show.sprite = tap;
        yield return new WaitForSeconds(tapPhaseDuration);
        show.color = new Color(1f, 1f, 1f, 0f);
        yield return new WaitForSeconds(disappearPhaseDuration);

        Enable();
    }
    public IEnumerator TutorialFinished()
    {
        float duration = 2f;
        float fadeOutDuration = duration / 2;
        float fadeInDuration = duration / 2;
        float elapsed = 0f;
        List<SpriteRenderer> renderers = new List<SpriteRenderer>();

        // Gather all sprite renderers
        foreach (Bin bin in BinsManager.Instance.availableBins)
        {
            renderers.Add(bin.spriteRenderer);
            renderers.Add(bin.openLidRenderer);
        }

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);

            foreach (var sr in renderers)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }
            tutorialFinishedText.color = new Color (1,1,1,1 - alpha);

            yield return null;
        }
        elapsed = 0f;
        renderers[0].sprite = Resources.Load<Sprite>("Bins/Paper");
        renderers[1].sprite = Resources.Load<Sprite>("Lids/Paper");
        renderers[2].sprite = Resources.Load<Sprite>("Bins/Glass");
        renderers[3].sprite = Resources.Load<Sprite>("Lids/Glass");
        renderers[4].sprite = Resources.Load<Sprite>("Bins/PlasticMetal");
        renderers[5].sprite = Resources.Load<Sprite>("Lids/PlasticMetal");
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);

            foreach (var sr in renderers)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }
            tutorialFinishedText.color = new Color(1, 1, 1, 1 - alpha);

            yield return null;
        }

        SaveSystem.Instance.Player.TutorialFinished = true;
    }
}
