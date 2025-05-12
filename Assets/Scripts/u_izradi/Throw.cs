using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    public float maxRadiusAddition = 2.5f;//random se dodaje od 0 do maxRadiusAddition na radius kruga po kom treba da se krece
    public float radius;
    public float minSpeed = 80f;   // At top (90°)
    public float maxSpeed = 170f;   // At sides (0° and 180°)
    public bool clockwise = false;

    private Vector2 centerPosition;
    private float angleDegrees = 90f;
    private Coroutine movementCoroutine;

    void Start()
    {
        clockwise = Random.Range(0, 2) == 0;
        //radius = Random.Range(minRadius, maxRadius);

        float radiusAdditon = Random.Range(0, maxRadiusAddition);
        float minRadius = Mathf.Abs((clockwise ? GameplayData.Instance.viewLeftX : GameplayData.Instance.viewRightX) - transform.position.x) + 1f;

        radius = minRadius + radiusAdditon;
        //radius = GameplayData.Instance.viewWidth + 1;


        centerPosition = new Vector2(transform.position.x + (clockwise ? -radius : radius), transform.position.y);
        StartMovement();
    }

    // Starts the movement
    public void StartMovement()
    {
        if (movementCoroutine == null)
        {
            movementCoroutine = StartCoroutine(MoveInCircle());
        }
    }

    // Stops the movement and destroys the component
    public void StopMovement()//ovo treba i kad se doda rigid body, tj kad se klikne na objekat
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }

        Destroy(this);  // Destroys the component itself
    }

    // Coroutine that handles circular movement
    private IEnumerator MoveInCircle()//simulira bacanje
    {
        while (true)
        {
            float angleRadians = angleDegrees * Mathf.Deg2Rad;

            // Smooth speed variation: min at 90°, max at 0° & 180°
            float speed = Mathf.Lerp(minSpeed, maxSpeed, Mathf.Abs(Mathf.Cos(angleRadians)));

            // Direction adjustment
            float directionMultiplier = clockwise ? -1f : 1f;

            // Apply variable speed
            angleDegrees += speed * directionMultiplier * Time.deltaTime;

            if (angleDegrees >= 360f) angleDegrees -= 360f;
            if (angleDegrees < 0f) angleDegrees += 360f;

            if (angleDegrees >= 180 && angleDegrees <= 360)
            {
                StopMovement();
            }


            float x = Mathf.Cos(angleRadians) * radius;
            float y = Mathf.Sin(angleRadians) * radius;

            transform.position = centerPosition + new Vector2(x, y);

            yield return null;
        }
    }
    //add rotation
}