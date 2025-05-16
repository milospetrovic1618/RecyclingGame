using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TrashManager : MonoBehaviour
{
    public static TrashManager Instance;
    public GameObject JunkArea;
    public float maxYJankArea;
    public float minXJankArea;
    public float maxXJankArea;
    public float minYJankArea;
    
    public List<Trash> trashList = new List<Trash>();
    public Queue<Trash> deactivatedTrashObjectPooling = new Queue<Trash>();//posle ces da dodas object pooling... samo gledaj gde si koristio destroy
    public int maxTrash = 20;
    private float offset = 0.3f;

    void Start()
    {
        Instance = this;
        offset = 0.3f;
        SpriteRenderer sr = JunkArea.GetComponent<SpriteRenderer>();
        Bounds bounds = sr.bounds;

        minXJankArea = bounds.min.x + offset;
        minYJankArea = bounds.min.y + offset;
        maxXJankArea = bounds.max.x - offset;
        maxYJankArea = bounds.max.y - offset;

        for (int i = 0; i < 10; i ++)
        {
            ThrowTrash();
        }
    }

    float timer = 0;
    void Update()
    {
        if (timer > 1f)
        {
            timer = 0;
            ThrowTrash();
            
        }
        timer += Time.deltaTime;
    }
    public void GameOver()
    {
        List<Trash> toRemove = new List<Trash>(trashList);
        foreach (Trash trash in toRemove)
        {
            DeactivateTrash(trash);
        }
    }
    public void ThrowTrash()
    {
        Trash newTrash = null;
        if (deactivatedTrashObjectPooling.Count == 0)
        {
            newTrash = SpawnNewTrash();
        }
        else
        {
            newTrash = deactivatedTrashObjectPooling.Dequeue();
            newTrash.gameObject.SetActive(true);
        }

        newTrash.gameObject.transform.position = new Vector2(GameplayManager.Instance.viewRightX + 10f, 0);//da se ne bi videlo u screenu
        newTrash.Throw(GetRandomPositionInJunkArea());
        trashList.Add(newTrash);

        if (trashList.Count > maxTrash)//> a ne >= zato sto spavnuje trash van vidokruga i player uopste ne moze da reaguje 
        {
            GameOver();
        }
    }
    public Vector2 GetRandomPositionInJunkArea()
    {
        float randomX = Random.Range(minXJankArea, maxXJankArea);
        float randomY = Random.Range(minYJankArea, maxYJankArea);
        Vector2 randomPosition = new Vector2(randomX, randomY);
        return randomPosition;
    }
    public Trash SpawnNewTrash()
    {
        GameObject newGameObject = new GameObject();
        Trash newTrash = newGameObject.AddComponent<Trash>();
        //ovo stavljam jer na 1 frame kad se kreira gameobject vidi se da mu je pozicija stavljena na 0,0
        //!!!!!!!!!!!!!!!!!! brisi todo
        newGameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1);

        return newTrash;
    }
    public Trash SpawnReusableTrash()
    {
        return null;
    }
    public Trash ReturnNullIfDestroyed(Trash trash)
    // Onaj problem sto si imao za null mozda mozes da resis tako sto proveravas da li postoji u listi posto nekad nije dovoljno da proveravas null jer tek u sledecem rame-u postaju null
    {
        if (trash == null)
        {
            return null;
        }
        if (trashList.Contains(trash))
        {
            return trash;
        }
        return null;
    }
    public void DeactivateTrash(Trash trash)
    {
        if (trashList.Contains(trash))//da ga ne sadrzi znacilo bi da je izbrisano
        {
            trashList.Remove(trash);
            trash.gameObject.SetActive(false);
            deactivatedTrashObjectPooling.Enqueue(trash);
            //Destroy(trash.gameObject);
        }
    }
}
