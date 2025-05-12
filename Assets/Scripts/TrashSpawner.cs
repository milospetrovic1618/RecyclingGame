using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TrashSpawner : MonoBehaviour
{
    public static TrashSpawner Instance;
    public GameObject JunkArea;
    private float maxY;
    private float minX;
    private float maxX;
    private float minY;
    private float lauchingMaxX;
    private float lauchingMinX;
    
    public List<Trash> trashList = new List<Trash>();
    public Queue<Trash> reusableGameObjects = new Queue<Trash>();//posle ces da dodas object pooling... samo gledaj gde si koristio destroy
    public int maxTrash = 20;
    private float offset = 0.3f;
    private float launchingOffset;

    void Start()
    {
        Instance = this;
        offset = 0.3f;
        launchingOffset = 2f;
        SpriteRenderer sr = JunkArea.GetComponent<SpriteRenderer>();
        Bounds bounds = sr.bounds;

        minX = bounds.min.x + offset;
        minY = bounds.min.y + offset;
        maxX = bounds.max.x - offset;
        maxY = bounds.max.y - offset;
        lauchingMaxX = bounds.max.x + launchingOffset;
        lauchingMinX = bounds.min.x - launchingOffset;

        for (int i = 0; i < 10; i ++)
        {
            SpawnTrash();
        }
    }

    float timer = 0;
    void Update()
    {
        if (timer > 2f)
        {
            timer = 0;
            SpawnTrash();
            
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
        foreach (Trash trash in reusableGameObjects)
        {
            Destroy(trash.gameObject);
        }
        reusableGameObjects.Clear();
    }
    public void SpawnTrash()
    {
        if (reusableGameObjects.Count == 0)
        {
            SpawnNewGameObject();
        }
        else
        {
            // todo
        }

        if (trashList.Count > maxTrash)
        {
            GameOver();
        }
    }
    public void SpawnNewGameObject()
    {
        GameObject gameObject = new GameObject();
        Trash newTrash = gameObject.AddComponent<Trash>();
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        //float launchingX = Random.value < 0.5f ? lauchingMinX : lauchingMaxX;
        Vector2 randomPosition = new Vector2(randomX, randomY);
        gameObject.transform.position = randomPosition;
        //newTrash.LaunchTrashProjectile(new Vector2(randomX,randomY), 45);

        gameObject.AddComponent<Throw>();

        trashList.Add(newTrash);

        //!!!!!!!!!!!!!!!!!! brisi todo
        gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1);
    }
    public void SpawnReusableGameObject()
    {

    }
}
