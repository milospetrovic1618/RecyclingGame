using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using TMPro;

public class Unlocked : MonoBehaviour
{
    public GameObject achievement;
    public RectTransform Cover;
    public RectTransform initialTransform;
    public Coroutine animation;
    public TextMeshProUGUI multiplierText;
    private int _phase;
    public int Phase
    {
        get => _phase;
        set
        {
            _phase = value%3;
            Debug.Log("PHASE "+ _phase);
            switch (_phase)
            {
                case 0:
                    NextUnlock();
                    break;
                case 1:
                    animation = StartCoroutine(AnimateCover());
                    break;
                case 2:
                    //FINISH ANIMATION
                    Cover.position = finalPosition;
                    if (animation != null)
                    {
                        StopCoroutine(animation);
                    }
                    if (multiplierText.text != "") multiplierText.gameObject.SetActive(true);
                    break;
            }
            //0- view new covered achievement
            //1 - ANIMATION
            //2 - view uncovered achievement

            //0- view covered achievement
            //1 - cover start animation
            //2 - cover animation ended, pure view on achievement
            //3 - view uncovered achievement
            //4 - view 
        }
    }
    public IEnumerator AnimateCover ()
    {
        float enlargeDuration = 0.5f;
        float targetSize = 1.3f;
        float elapsed = 0; 
        
        Vector3 originalScale = Cover.localScale;
        Vector3 targetScale = originalScale * targetSize;

        while (elapsed < enlargeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / enlargeDuration;
            Cover.localScale = Vector3.Lerp(originalScale, targetScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0;
        float rotateDuration = 1f;

        Vector2 relativeStart = (Vector2)Cover.position - rotateReference;

        while (Mathf.Pow(elapsed, 1.5f) < rotateDuration)
        {
            float t = Mathf.Pow(elapsed,1.5f) / rotateDuration;//pow na 1.5 da se ubrazava
            float angle = Mathf.Lerp(0f, 90f, t) * Mathf.Deg2Rad;

            // Rotation matrix:
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            Vector2 rotated = new Vector2(
                relativeStart.x * cos - relativeStart.y * sin,
                relativeStart.x * sin + relativeStart.y * cos
            );

            Vector2 finalPos = rotated + rotateReference;

            // Move object if available
            Cover.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            Cover.position = finalPos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Phase++;
        //Cover is rectTransorm
        //rotate it counterclockwise by 90 degreees for rotateDuratoon in reference to RotateReference
    }
    public Vector2 finalPosition;
    public Vector2 rotateReference;
    public void Click()
    {
        Debug.Log("Clicked");
        SoundManager.Instance.PlayButtonClick();
        Phase++;
    }
    void Start()
    {
        CopyRectTransform(Cover, initialTransform);

        rotateReference = new Vector2(Camera.main.WorldToScreenPoint(new Vector2(BootMain.Instance.viewRightX, 0)).x + 2*(Cover.rect.width * Cover.lossyScale.x), Cover.position.y);//2* da bi bilo dalje
        Vector2 translated = (Vector2)Cover.position - rotateReference;
        Vector2 rotated = new Vector2(-translated.y, translated.x);
        finalPosition = rotated + rotateReference;

        Phase = 0;
    }
    public static void CopyRectTransform(RectTransform source, RectTransform target)
    {
        target.anchoredPosition = source.anchoredPosition;
        target.sizeDelta = source.sizeDelta;
        target.anchorMin = source.anchorMin;
        target.anchorMax = source.anchorMax;
        target.pivot = source.pivot;
        target.localScale = source.localScale;
        target.localRotation = source.localRotation;
        target.localPosition = source.localPosition;
    }
    public void NextUnlock()
    {
        if (BootMain.Instance.unlocked.Count == 0)
        {
            if (BootMain.Instance.IsSceneLoaded(Scenes.Gameplay))
            {
                BootMain.Instance.blockingSpawn = false;
            }
            BootMain.Instance.UnloadUnlocked();
            return;
        }

        CopyRectTransform(initialTransform, Cover);

        AchievementData achievementData = BootMain.Instance.unlocked[BootMain.Instance.unlocked.Count - 1];

        multiplierText.gameObject.SetActive(false);

        multiplierText.text = GetMultiplierText(achievementData.name);

        AchievementUI achievementUI = new AchievementUI(achievement, BootMain.Instance.unlocked[BootMain.Instance.unlocked.Count-1]);
        BootMain.Instance.unlocked.RemoveAt(BootMain.Instance.unlocked.Count - 1 );

        //korutina animacije da se skloni, ako ide jos jedan k

        //sad stavi da se Unlock 
    }
    public string GetMultiplierText(string name)
    {
        float multiplier = 0;
        string type = "";
        switch (name)
        {
            case "TotalCount":
                type = "ALL";
                multiplier = SaveSystem.Instance.Player.GetTotalMultiplier();
                break;
            case "PlasticMetalCount":
                type = "Plastic & Metal";
                multiplier = SaveSystem.Instance.Player.GetBinMultiplier(SaveSystem.Instance.Player.PlasticMetalCount);
                break;
            case "PaperCount":
                type = "Paper";
                multiplier = SaveSystem.Instance.Player.GetBinMultiplier(SaveSystem.Instance.Player.PaperCount);
                break;
            case "GlassCount":
                type = "Glass";
                multiplier = SaveSystem.Instance.Player.GetBinMultiplier(SaveSystem.Instance.Player.GlassCount);
                break;
            case "ElectronicsBatteriesCount":
                type = "Electronics & Batteries";
                multiplier = SaveSystem.Instance.Player.GetBinMultiplier(SaveSystem.Instance.Player.ElectronicsBatteriesCount);
                break;
            case "OrganicCount":
                type = "Organic";
                multiplier = SaveSystem.Instance.Player.GetBinMultiplier(SaveSystem.Instance.Player.OrganicCount);
                break;
            default: return "";
        }
        return type + "\n" + multiplier + "x MULTIPLIER";
    }
}
