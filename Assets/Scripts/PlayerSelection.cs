using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSelection : MonoBehaviour
{
    public static PlayerSelection Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    public bool press = false;//sama rec kaze
    public bool pressedPrevFrame = false;//pressedPrevFrame je press u proslom frejmu... cini mi se da mu je jedina uloga prepoznavanje releasa
    public bool release;
    public Vector2 releasedWorldPosition;
    public Vector2 curWorldPosition;//nisi koristio mozda ce da ti treba
    public Vector2 curScreenPosition;//nisi koristio mozda ce da ti treba
    public string deviceType = ""; //BOJAN: da li je stvarno bolje string nego DeviceType enum? :)

    public float maxDistanceClickTrash = 0.6f;
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
        if (!IsOnUI())
        {
            Inputs();
        }
    }
    public bool IsOnUI()
    {
        //BOJAN: sa obzirom da ces imati 20ak itema na tabli max, ja bih ovo resio stavljanjem BoxCollider2D na svaki item, i na njegovu skriptu OnMouseDown / OnMouseUp, a za bin bih detektovao OnCollisionEnter2D / OnCollisionExit2D
        //BOJAN: ali nisam siguran sta je optimalnije, znam da samo izbegavam ovaj EventSystem bas jako zbog raznih nekih glitcheva
        if (EventSystem.current.IsPointerOverGameObject())//UI IMA SVOJ EVENT SYSTEM
        { 
            return true; 
        }
        return false;
    }
    
    //BOJAN: komentar za ovaj tvoj komentar ispod => mozes da koristis mozda #IF UNITY_STANDALONE || UNITY_EDITOR ili #IF UNITY_ANDROID || UNITY_IOS
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
        else if ( deviceType == "Mobile")
        {
            /*if (Input.touchCount == 1 &&
            (Input.GetTouch(0).Phase == TouchPhase.Moved || Input.GetTouch(0).Phase == TouchPhase.Stationary))
            {
                press = true;

                curScreenPosition = Input.GetTouch(0).position;
                curWorldPosition = Camera.main.ScreenToWorldPoint(curScreenPosition);
            }
            else
            {
                press = false;
            }*/
            if (Input.touchCount== 1)
            {
                Touch touch = Input.GetTouch(0);

                // Track world position always while touch exists
                curScreenPosition = touch.position;
                curWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(curScreenPosition.x, curScreenPosition.y, Camera.main.nearClipPlane));

                //BOJAN: zar nije lepse press = touch.Phase == TouchPhase.Moved || touch.Phase == TouchPhase.Stationary;
                //BOJAN: jedna linija koda, radi isto sto i ovaj dole kod :)
                //BOJAN: moglo bi da se optimizuje sa nekim toggle-ovanjem, da ne ispitujes stalno touch Phase, jer ovako u 
                // Set press only when appropriate

                press = true;
            }
        }

        if (pressedPrevFrame && !press)
        {
            release = true;
        }


        Activities();

        pressedPrevFrame = press;//mora da bude ispod activities

    }
    public void Activities() //moras da zamenis ako se objkti overlappuju
    {
        CheckTrashBinSelection();
    }
    
    //BOJAN: promenljive uvek na vrhu skripte :) i ja ovo volim da radim, da ostavim ovako promenljive iznad metode gde se zapravo koriste, pogotovo ako su private, ali 99% programera ce te mrzeti zbog ovoga :P
    Trash selectedTrash = null;
    Bin selectedBin = null;
    bool wasTrashHeldPrevFrame = false;
    public void CheckTrashBinSelection() //moras da zamenis ako se objkti overlappuju
    {

        if (press)
        {
            if (TrashManager.Instance.ReturnNullIfDestroyed(selectedTrash) != null) //ovo je isto kao selectedTrash != null
            {
                //Debug.Log(1);
                selectedTrash.transform.position = curWorldPosition;
                //Debug.Log((selectedTrash.rigidbody == null).ToString());
                selectedTrash.ToggleRigidBody(false);


                //BOJAN: ovaj layerMask koristis na vise mesta, mogao bi da ga setujes samo jednom na awake-u u private promenljivu
                LayerMask layerMask = LayerMask.GetMask(Layer.BinsSelectingColliders.ToString());
                RaycastHit2D hit = Physics2D.Raycast(curWorldPosition, Vector2.zero, float.MaxValue, layerMask);
                //Debug.Log("+++++++++==");

                if (hit.collider == null)
                {
                    if (selectedBin != null)
                    {
                        selectedBin.DeSelect();
                        selectedBin = null;
                    }
                }
                else
                {
                    /*if (selectedBin == null)
                    {
                        selectedBin = hit.collider.transform.parent.GetComponent<Bin>();//ovde je parent jer je na child-u collider za selecting (BinsSelectingColliders)
                        selectedBin.Select();
                    }
                    //selectedTrash.rigidbody.bodyType = RigidbodyType2D.Static;
                    */
                    Bin curSelectedBin = hit.collider.transform.parent.GetComponent<Bin>();
                    if (selectedBin != curSelectedBin)
                    {
                        if (selectedBin != null)
                        {
                            selectedBin.DeSelect();
                            selectedBin = null;
                        }
                        selectedBin = curSelectedBin;//ovde je parent jer je na child-u collider za selecting (BinsSelectingColliders)
                        selectedBin.Select();
                    }
                }
            }
            else
            {
                Trash trySelectTrash = null;
                float minDistance = maxDistanceClickTrash;

                foreach (Trash trash in TrashManager.Instance.trashList)
                {
                    float distance = Vector2.Distance(trash.gameObject.transform.position, curWorldPosition);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        trySelectTrash = trash;
                    }
                }

                if (trySelectTrash == null)
                {
                    if (selectedTrash != null)
                    {
                        selectedTrash.DeSelect();
                        selectedTrash = null;
                    }
                }
                else
                {
                    if (selectedTrash == null)
                    {
                        selectedTrash = trySelectTrash;
                        selectedTrash?.Select();//ne vidim razlog za ? ali dobijao sam null bug ovde
                        wasTrashHeldPrevFrame = true;
                    }
                    //selectedTrash.rigidbody.bodyType = RigidbodyType2D.Static;
                }

                /*LayerMask layerMask = LayerMask.GetMask(Layer.Trash.ToString());
                RaycastHit2D hit = Physics2D.Raycast(curWorldPosition, Vector2.zero, float.MaxValue, layerMask);

                if (hit.collider == null)
                {
                    if (selectedTrash != null)
                    {
                        selectedTrash.DeSelect();
                        selectedTrash = null;
                    }
                }
                else
                {
                    if (selectedTrash == null)
                    {
                        selectedTrash = hit.collider.gameObject.GetComponent<Trash>();
                        selectedTrash?.Select();//ne vidim razlog za ? ali dobijao sam null bug ovde
                        wasTrashHeldPrevFrame = true;
                    }
                    //selectedTrash.rigidbody.bodyType = RigidbodyType2D.Static;
                }*/
            }
            /*if (pressedPrevFrame == false)
            {
                LayerMask layerMask = LayerMask.GetMask(Layer.BinsSelectingColliders.ToString());
                RaycastHit2D hit = Physics2D.Raycast(curWorldPosition, Vector2.zero, float.MaxValue, layerMask);
                //Debug.Log("+++++++++==");

                if (hit.collider != null)
                {
                    Debug.Log(" ddddd " + pressedPrevFrame);
                    Bin bin = hit.collider.transform.parent.GetComponent<Bin>();
                    if (bin.trashCount >= bin.maxTrashCount)
                    {
                        BinsManager.Instance.ReplaceBin(bin);
                    }
                }
            }*/
        }
        else if (release )
        {
            if (!wasTrashHeldPrevFrame)//wasTrashHeldPrevFrame zato sto si imao bug da kad se privuce trash takodje aktivira usisavanje kante... a release i press nisu u istom frame-u nikad
            {
                //Debug.Log(" ppppppp " + pressedPrevFrame);
                LayerMask layerMask0 = LayerMask.GetMask(Layer.BinsSelectingColliders.ToString());
                RaycastHit2D hit0 = Physics2D.Raycast(curWorldPosition, Vector2.zero, float.MaxValue, layerMask0);
                //Debug.Log("+++++++++==");

                if (hit0.collider != null)
                {
                    //Debug.Log(" ddddd " + pressedPrevFrame);
                    Bin bin = hit0.collider.transform.parent.GetComponent<Bin>();
                    if (bin.trashCount >= Bin.maxTrashCount)
                    {
                        BinsManager.Instance.ReplaceBin(bin);
                    }
                }
            }
            wasTrashHeldPrevFrame = false;



            if (selectedTrash != null && selectedBin != null )
            {
                selectedBin.PutInBin(selectedTrash);//ovo ne radi ne znam zasto
            }
            if (curWorldPosition.x > TrashManager.Instance.maxXJankArea || 
                curWorldPosition.x < TrashManager.Instance.minXJankArea ||
                curWorldPosition.y > TrashManager.Instance.maxYJankArea ||
                curWorldPosition.y < TrashManager.Instance.minYJankArea)//rigid body se stavlja samo ako nije u trashArea
            {
                TrashManager.Instance.ReturnNullIfDestroyed(selectedTrash)?.ToggleRigidBody(true);// selectedTrash?.ToggleRigidBody(true);
            }
            TrashManager.Instance.ReturnNullIfDestroyed(selectedTrash)?.DeSelect();//inace ovde je bilo selectedTrash?.DeSelect()
            selectedTrash = null;

            //ovde treba da se odselektuje i kad nije curposition over
            selectedBin?.DeSelect();
            selectedBin = null;
        }


    }
}
