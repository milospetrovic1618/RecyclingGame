using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSelection : MonoBehaviour
{
    public bool press = false;//sama rec kaze
    //public bool pressedPrevFrame = false;//pressedPrevFrame je press u proslom frejmu... cini mi se da mu je jedina uloga prepoznavanje releasa
    public bool release;
    public Vector2 releasedWorldPosition;
    public Vector2 curWorldPosition;//nisi koristio mozda ce da ti treba
    public Vector2 curScreenPosition;//nisi koristio mozda ce da ti treba
    public string deviceType = "";
    void Start()
    {
        deviceType = WhichDeviceType();
    }

    public string WhichDeviceType()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            return "PC";//Desktop
        }

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            return "Mobile";
        }

        if (SystemInfo.deviceType == DeviceType.Console)
        {
            return "Console";
        }
        return "Unknown";
    }
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())//ovo je za ako je preko ui
            return;
        Inputs();
    }

    //temp, izbrisi kasnije kada onemogucis da u isto vreme bude u building inputMode , ovo je za onaj donji if skroz, mozes ti posebno svaki inputMode da odradis ali ne bi bilo lose da napravis prioritetet i tako smanjis broj if-ova tako sto koristis if else
    public void Inputs()
    {
        release = false;
        press = false;

        if (deviceType == "PC")
        {
            press = Input.GetMouseButton(0);
            if (press)
            {
                curScreenPosition = Input.mousePosition;
                curWorldPosition = Camera.main.ScreenToWorldPoint(curScreenPosition);
            }
        }
        else if (deviceType == "Mobile")
        {
            if (Input.touchCount == 1 &&
            (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary))
            {
                press = true;

                curScreenPosition = Input.GetTouch(0).position;
                curWorldPosition = Camera.main.ScreenToWorldPoint(curScreenPosition);
            }
            else
            {
                press = false;
            }
        }

        /*if (pressedPrevFrame && !press) ovo mi ne treba
        {
            release = true;
        }

        pressedPrevFrame = press;*/

        CheckPressOverTrash();
    }
    Trash curentlyHovering = null;
    public void CheckPressOverTrash() //moras da zamenis ako se objkti overlappuju
    {
        if (press)
        {
            LayerMask layerMask = LayerMask.GetMask(Layer.Trash.ToString());
            RaycastHit2D hit = Physics2D.Raycast(curWorldPosition, Vector2.zero, float.MaxValue, layerMask);

            if (hit.collider == null)
            {
                if (curentlyHovering != null)
                {
                    curentlyHovering.DeSelect();
                    curentlyHovering = null;
                }
            }
            else
            {
                if (curentlyHovering == null)
                {
                    curentlyHovering = hit.collider.gameObject.GetComponent<Trash>();
                    curentlyHovering.Select();
                }
                curentlyHovering.transform.position = curWorldPosition;
                curentlyHovering.rigidbody.bodyType = RigidbodyType2D.Static;

            }
        }
        
    }
}
