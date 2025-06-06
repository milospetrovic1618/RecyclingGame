using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using static Unity.Burst.Intrinsics.X86.Avx;

public class ScoreShow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    TextMeshPro backText;
    TextMeshPro frontText;
    public void Initialize(float value, RecyclingType recyclingType, bool power)
    {
        int fontSize = 5;
        if (power)
        {
            fontSize = (int)(fontSize * 1.5f);
        }
        string text = "+" + ((int)value).ToString();

        //  backText for outline
        GameObject backTextObj = new GameObject("BackText");
        backTextObj.transform.SetParent(transform);
        backTextObj.transform.localPosition = Vector3.zero;

        backText = backTextObj.AddComponent<TextMeshPro>();
        backText.font = BootMain.Instance.LuckiestGuy; 
        backText.text = text;
        backText.fontSize = fontSize;
        backText.color = Color.black;
        backText.alignment = TextAlignmentOptions.Center;
        backText.textWrappingMode = TextWrappingModes.NoWrap;
        backText.fontStyle = FontStyles.Bold;

        //new material
        Material backMat = new Material(backText.font.material);
        backText.fontMaterial = backMat;

        //  outline  
        backText.outlineWidth = 0.7f;
        backMat.EnableKeyword("UNDERLAY_ON");
        backMat.SetColor("_UnderlayColor", Color.black);
        backMat.SetFloat("_UnderlaySoftness", 0.5f);
        backMat.SetFloat("_UnderlayOffsetX", 0f);
        backMat.SetFloat("_UnderlayOffsetY", 0f);

        // Refresh  
        backText.SetAllDirty();

        MeshRenderer backRenderer = backText.GetComponent<MeshRenderer>();
        backRenderer.sortingLayerName = SortingLayer.Score.ToString();
        backRenderer.sortingOrder = 0;


        frontText = gameObject.AddComponent<TextMeshPro>();
        frontText.font = BootMain.Instance.LuckiestGuy; // assign font FIRST
        frontText.text = text;
        frontText.fontSize = fontSize;
        frontText.color = Color.white;
        frontText.alignment = TextAlignmentOptions.Center;
        frontText.textWrappingMode = TextWrappingModes.NoWrap;
        frontText.fontStyle = FontStyles.Bold;

        Material frontMat = new Material(frontText.font.material);
        frontText.fontMaterial = frontMat;

        frontText.SetAllDirty();

        MeshRenderer frontRenderer = frontText.GetComponent<MeshRenderer>();
        frontRenderer.sortingLayerName = SortingLayer.Score.ToString();
        frontRenderer.sortingOrder = 1;


        Vector2 scorePosition = BinsManager.Instance.GetBinFromRecyclingType(recyclingType).transform.position;
        scorePosition.y += BinsManager.Instance.binHeight * 1.15f;
        float xOffset = BinsManager.Instance.binWidth / 6;
        scorePosition.x += UnityEngine.Random.Range(-xOffset, xOffset);//na srednjoj trecini kante, ako je iznad podeljeno sa 6
        transform.position = scorePosition;

        StartCoroutine(ScoreEnumerator(1f, BinsManager.Instance.binHeight/2f, BinsManager.Instance.binWidth / 8f));
    }
    public IEnumerator ScoreEnumerator(float destroyDelay, float height, float amplitude)
    {
        float timer = 0f; 
        float upSpeed = height / destroyDelay;

        bool sinDirection = Random.value > 0.5f;
        int numberOfSways = 2;

        Vector3 initialPosition = transform.position;

        while (timer < destroyDelay)
        {
            timer += Time.deltaTime;


            //x movement
            float radians = (timer / destroyDelay) * numberOfSways * Mathf.PI;
            float xChange = amplitude * Mathf.Sin(radians) * (sinDirection ? 1f : -1f);

            //x and y
            transform.position = new Vector3(initialPosition.x + xChange, initialPosition.y + upSpeed * timer, initialPosition.z);

            float alpha = 1f - (timer / destroyDelay);
            Color c = frontText.color;
            c.a = alpha;
            frontText.color = c;
            backText.color = c;

            yield return null; 
        }

        Destroy(gameObject); 
    }
}
