using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementsCoroutines : MonoBehaviour
{
    public static MovementsCoroutines Instance;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }
    /*public void StartMovement() //hteo sam da stavim ove korutine direktno na trash ali ovako mi urednije i lakse
    {
        if (movementCoroutine == null)
        {
            movementCoroutine = StartCoroutine(Throw());
        }
    }

    // Stops the movement and destroys the component
    public void StopCoroutineMovement()//ovo treba i kad se doda rigid body, tj kad se klikne na objekat
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }

        //Destroy(this);  // Destroys the component itself
    }*/

    // Coroutine that handles circular movement
    public IEnumerator Throw(Vector2 targetPosition, Transform transformToAnimate)//simulira bacanje.. ovo je zapravo Throw ali delom
    {
        Debug.Log("uslo je");
        float maxRadiusAddition = 2.5f;//random se dodaje od 0 do maxRadiusAddition na radius kruga po kom treba da se krece
        float minSpeed = 60f;   // At top (90°)
        float maxSpeed = 130f;   // At sides (0° and 180°)
        bool clockwise = false;
        float rotationSpeed = 360f;
        Vector2 centerPosition;
        float angleDegrees = 90f;


        clockwise = UnityEngine.Random.Range(0, 2) == 0;
        float directionMultiplier = clockwise ? -1f : 1f;
        //radius = Random.Range(minRadius, maxRadius);

        float radiusAdditon = UnityEngine.Random.Range(0, maxRadiusAddition);
        float minRadius = Mathf.Abs((clockwise ? DataGameplay.Instance.viewLeftX : DataGameplay.Instance.viewRightX) - targetPosition.x) + 1f;

        float radius = minRadius + radiusAdditon;
        //radius = DataGameplay.Instance.viewWidth + 1;

        centerPosition = new Vector2(targetPosition.x + (clockwise ? -radius : radius), targetPosition.y);
        //Debug.Log(centerPosition.ToString());

        transformToAnimate.Rotate(0f, 0f, UnityEngine.Random.Range(0f, 360f));//ovo je pocetna rotacija koje je random
        rotationSpeed = UnityEngine.Random.Range(360f, 720f);
        //Debug.Log(angleDegrees);
        while (angleDegrees < 180)
        {
            //Debug.Log("1");
            transformToAnimate.Rotate(0f, 0f, (clockwise ? -rotationSpeed : rotationSpeed) * Time.deltaTime);


            float angleRadians = angleDegrees * Mathf.Deg2Rad;

            // Smooth speed variation: min at 90°, max at 0° & 180°
            float speed = Mathf.Lerp(minSpeed, maxSpeed, Mathf.Abs(Mathf.Cos(angleRadians)));

            // Direction adjustment

            // Apply variable speed
            angleDegrees += speed * directionMultiplier * Time.deltaTime;

            if (angleDegrees >= 360f) angleDegrees -= 360f;
            if (angleDegrees < 0f) angleDegrees += 360f;

            //if (angleDegrees >= 180 && angleDegrees <= 360)
            //{
            //    StopCoroutineMovement();
            //}


            float x = Mathf.Cos(angleRadians) * radius;
            float y = Mathf.Sin(angleRadians) * radius;

            transformToAnimate.position = centerPosition + new Vector2(x, y);

            yield return null;
        }
    }
    public IEnumerator CurveMove(Vector2 endPos, Transform transformToAnimate)
    {
        Vector2 startPos = transformToAnimate.position;
        float duration = 0.7f;

        float curveHeightReference = 4f;
        float curveHeightCameraSizeReference = 10f;
        float curveHeight = curveHeightReference * BootGameplay.Instance.camera.orthographicSize / curveHeightCameraSizeReference;
        
        bool clockwise = UnityEngine.Random.value < 0.5f;


        Vector2 direction = (endPos - startPos).normalized;
        Vector2 perpendicular = clockwise ? new Vector2(-direction.y, direction.x) : new Vector2(direction.y, -direction.x); // 90 stepeni rotira
        Vector2 control = (startPos + endPos) / 2 + perpendicular * curveHeight;

        //napravi da se izabere Vector2 perpendicular = new Vector2(-direction.y, direction.x);  ili new Vector2(direction.y, -direction.x)

        float elapsed = 0f;
        //Vector2 endPos = PickingResourcesManager.Instance.GetUIIconWorldPos(resourceDropType);

        while (elapsed < duration)//neke stvari bi bile iznad da je je endPos fiksiran
        {
            float t = Mathf.Clamp01(elapsed / duration);


            // Quadratic Bezier interpolation
            Vector2 position = Mathf.Pow(1 - t, 2) * startPos +
                               2 * (1 - t) * t * control +
                               Mathf.Pow(t, 2) * endPos;

            transformToAnimate.position = position;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transformToAnimate.position = endPos;

    }
    /*public void StartCoroutineWithCallback(IEnumerator coroutine, Action<object[]> callback, params object[] args)
    {
        StartCoroutine(RunCoroutineAndCallback(coroutine, callback, args));
    }

    private IEnumerator RunCoroutineAndCallback(IEnumerator coroutine, Action<object[]> callback, object[] args)
    {
        yield return StartCoroutine(coroutine);
        callback?.Invoke(args);
    }*/
    public Coroutine StartCoroutineWithCallback(IEnumerator enumerator, Action callback)
    {
        return StartCoroutine(RunCoroutineAndCallback(enumerator, callback));
    }

    private IEnumerator RunCoroutineAndCallback(IEnumerator coroutine, Action callback )
    {
        yield return StartCoroutine(coroutine);
        callback?.Invoke();
    }
    public IEnumerator MoveToPosition(Vector2 targetPosition, Transform transformToAnimate, float moveDuration)
    {
        Vector3 startPosition;
        float elapsedTime;

        startPosition = transformToAnimate.position;
        elapsedTime = 0f;

        float t = 0;
        while (t < 1f)
        {
            elapsedTime += Time.deltaTime;
            t = Mathf.Clamp01(elapsedTime / moveDuration);
            transformToAnimate.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

    }
    public IEnumerator ShakingIndefinitely(Transform transformToAnimate)
    {
        float magnitude = 0.1f; 
        float delay = 0.05f;//da se ne shakuje previse brzo

        Vector3 originalPosition = transformToAnimate.localPosition;

        float elapsed = 0f;

        while (true)
        {
            float offsetX = UnityEngine.Random.Range(-0.5f, 0.5f) * magnitude;
            float offsetY = UnityEngine.Random.Range(-0.5f, 0.5f) * magnitude;

            transformToAnimate.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return new WaitForSeconds(delay);
        }
    }

    public IEnumerator CurveMoveFollow(Transform transformToFollow, Transform transformToAnimate)
    {
        Vector2 startPos = transformToAnimate.position;
        float duration = 0.7f;

        float curveHeightReference = 4f;
        float curveHeightCameraSizeReference = 10f;
        float curveHeight = curveHeightReference * BootGameplay.Instance.camera.orthographicSize / curveHeightCameraSizeReference;

        bool clockwise = UnityEngine.Random.value < 0.5f;


        Vector2 direction = ((Vector2)transformToFollow.position - startPos).normalized;
        Vector2 perpendicular = clockwise ? new Vector2(-direction.y, direction.x) : new Vector2(direction.y, -direction.x); // 90 stepeni rotira
        Vector2 control = (startPos + (Vector2)transformToFollow.position) / 2 + perpendicular * curveHeight;

        //napravi da se izabere Vector2 perpendicular = new Vector2(-direction.y, direction.x);  ili new Vector2(direction.y, -direction.x)

        float elapsed = 0f;
        //Vector2 endPos = PickingResourcesManager.Instance.GetUIIconWorldPos(resourceDropType);

        while (elapsed < duration)//neke stvari bi bile iznad da je je endPos fiksiran
        {
            float t = Mathf.Clamp01(elapsed / duration);


            // Quadratic Bezier interpolation
            Vector2 position = Mathf.Pow(1 - t, 2) * startPos +
                               2 * (1 - t) * t * control +
                               Mathf.Pow(t, 2) * (Vector2)transformToFollow.position;

            //ovu proveru si morao da dodas zbog buga gde se izbrise tah gameobject sa tim transformom
            if (transformToAnimate != null)
            {
                transformToAnimate.position = position;
            }
            else
            {
                break;
            }    

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (transformToAnimate != null)
        //ovu proveru si morao da dodas zbog buga gde se izbrise tah gameobject sa tim transformom
        {
            transformToAnimate.position = (Vector2)transformToFollow.position;
        }

    }
}
