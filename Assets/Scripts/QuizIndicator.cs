using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using System.Collections;

public class QuizIndicator : MonoBehaviour
{
    TextMeshProUGUI frontText;
    RectTransform rectTransform;
    public void Initialize(bool correct, Vector2 uiSpawnPos)
    {
        gameObject.name = "QuizIndicator";
        gameObject.transform.SetParent(BootGameplay.Instance.canvas.transform);
        gameObject.transform.SetAsLastSibling();

        int fontSize = 80;
        string text = "";
        if (correct)
        {
            text = "Correct!";
        }
        else
        {
            text = "Incorrect!";
        }


        rectTransform = gameObject.AddComponent<RectTransform>();
        gameObject.AddComponent<MeshRenderer>();
        frontText = gameObject.AddComponent<TextMeshProUGUI>();
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
        /*frontText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0.6f);
        frontText.outlineWidth = 1;*/

        frontText.SetAllDirty();

        MeshRenderer frontRenderer = frontText.GetComponent<MeshRenderer>();
        frontRenderer.sortingLayerName = SortingLayer.Score.ToString();
        frontRenderer.sortingOrder = 1;


        rectTransform.position = uiSpawnPos;

        StartCoroutine(ScoreEnumerator(1f, 300f, 50f));
    }
    public IEnumerator ScoreEnumerator(float destroyDelay, float height, float amplitude)
    {
        float timer = 0f;
        float upSpeed = height / destroyDelay;

        bool sinDirection = Random.value > 0.5f;
        int numberOfSways = 2;

        Vector3 initialPosition = rectTransform.position;

        while (timer < destroyDelay)
        {
            timer += Time.deltaTime;


            //x movement
            float radians = (timer / destroyDelay) * numberOfSways * Mathf.PI;
            float xChange = amplitude * Mathf.Sin(radians) * (sinDirection ? 1f : -1f);

            //x and y
            rectTransform.position = new Vector3(initialPosition.x + xChange, initialPosition.y + upSpeed * timer, initialPosition.z);

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
