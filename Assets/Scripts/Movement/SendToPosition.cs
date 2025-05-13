using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendToPosition : MonoBehaviour
{
    //assign from outside
    public Vector3 targetPosition;

    public float moveDuration = 0.5f;

    private Vector3 startPosition;
    private float elapsedTime;

    void Start()
    {
        startPosition = transform.position;
        elapsedTime = 0f;
    }

    void Update()
    {

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / moveDuration);
        transform.position = Vector3.Lerp(startPosition, targetPosition, t);

        if (t >= 1f)
        {
            Destroy(this);
        }
    }
}
