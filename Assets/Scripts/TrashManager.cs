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
    public Queue<Trash> reusableTrashObjectPooling = new Queue<Trash>();//posle ces da dodas object pooling... samo gledaj gde si koristio destroy
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
        foreach (Trash trash in trashList)
        {
            Destroy(trash.gameObject);
        }
        trashList.Clear();
        foreach (Trash trash in reusableTrashObjectPooling)
        {
            Destroy(trash.gameObject);
        }
        reusableTrashObjectPooling.Clear();
    }
    public void ThrowTrash()
    {
        Trash newTrash = null;
        if (reusableTrashObjectPooling.Count == 0)
        {
            newTrash = SpawnNewTrash();
        }
        else
        {
            // todo
        }
        GameObject newGameObject = newTrash.gameObject;

        Throw throwInstance = newGameObject.AddComponent<Throw>();
        throwInstance.targetPosition = GetRandomPositionInJunkArea();
        trashList.Add(newTrash);

        if (trashList.Count > maxTrash)
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
        newGameObject.transform.position = new Vector2(DataGameplay.Instance.viewRightX + 10f,0);//ovo stavljam jer na 1 frame kad se kreira gameobject vidi se da mu je pozicija stavljena na 0,0
        //!!!!!!!!!!!!!!!!!! brisi todo
        newGameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1);

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
    public void DeleteTrash(Trash trash)
    {
        trashList.Remove(trash);
        Destroy(trash.gameObject);
    }
}
