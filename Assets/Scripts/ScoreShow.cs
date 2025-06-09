using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using static Unity.Burst.Intrinsics.X86.Avx;
using System.Xml;

public class ScoreShow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //TextMeshPro backText;
    TextMeshPro frontText;
    public void Initialize(float value, RecyclingType recyclingType, bool power)
    {
        gameObject.name = "score";

        int fontSize = 5;
        if (power)
        {
            fontSize = (int)(fontSize * 1.5f);
        }
        string text = "+" + ((int)value).ToString();


        GameObject frontTextObj = new GameObject("FrontText");
        frontTextObj.transform.SetParent(transform);
        frontTextObj.transform.localPosition = Vector3.zero;
        frontText = frontTextObj.AddComponent<TextMeshPro>();
        frontText.font = BootMain.Instance.LuckiestGuy; // assign font FIRST
        frontText.text = text;
        frontText.fontSize = fontSize;
        frontText.color = Color.white;
        frontText.alignment = TextAlignmentOptions.Center;
        frontText.textWrappingMode = TextWrappingModes.NoWrap;
        frontText.fontStyle = FontStyles.Bold;

        Material frontMat = new Material(frontText.font.material);
        frontText.fontMaterial = frontMat;

        //OUTLINE
        frontText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0.6f);
        frontText.outlineWidth = 1;

        frontText.SetAllDirty();

        MeshRenderer frontRenderer = frontText.GetComponent<MeshRenderer>();
        frontRenderer.sortingLayerName = SortingLayer.Score.ToString();
        frontRenderer.sortingOrder = 1;


        //  backText for outline
        /*GameObject backTextObj = new GameObject("BackText");
        backTextObj.transform.SetParent(transform);
        backTextObj.transform.localPosition = Vector3.zero;

        backText = backTextObj.AddComponent<TextMeshPro>();
        backText.font = BootMain.Instance.LuckiestGuy; 
        backText.text = text;
        backText.fontSize = fontSize;
        Color backTextColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        backText.color = backTextColor;
        backText.alignment = TextAlignmentOptions.Center;
        backText.textWrappingMode = TextWrappingModes.NoWrap;
        backText.fontStyle = FontStyles.Bold;

        //new material
        Material backMat = new Material(backText.font.material);
        backText.fontMaterial = backMat;

        //  outline  
        backText.outlineWidth = 1f;
        backMat.EnableKeyword("UNDERLAY_ON");
        backMat.SetColor("_UnderlayColor", Color.white);

        // Refresh  
        backText.SetAllDirty();

        MeshRenderer backRenderer = backText.GetComponent<MeshRenderer>();
        backRenderer.sortingLayerName = SortingLayer.Score.ToString();
        backRenderer.sortingOrder = 0;*/




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
            //backText.color = c;

            yield return null; 
        }

        Destroy(gameObject); 
    }
}
