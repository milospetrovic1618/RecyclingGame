using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashDetector : MonoBehaviour
{
    // Start is called before the first frame update
    public Bin bin;
    void Start()
    {
        bin = transform.parent.GetComponent<Bin>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(" TrashDetector ");
        Trash trash = other.GetComponent<Trash>();
        if (trash != null)
        {
            //Debug.Log(bin.MatchesBin(trash));
            bin.PutInBin(trash);
        }
    }
}
