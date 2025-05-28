using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashRotator : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        float rotationSpeed = 150f * Time.deltaTime;
        other.transform.Rotate(0, 0, rotationSpeed);
    }
}
