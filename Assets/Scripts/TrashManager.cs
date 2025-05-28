using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Networking.UnityWebRequest;

public class TrashManager : MonoBehaviour
{
    public static TrashManager Instance;
    public GameObject JunkArea;
    public float maxYJankArea;
    public float minXJankArea;
    public float maxXJankArea;
    public float minYJankArea;
    public List<Trash> trashList { get; private set; } = new List<Trash>();

    public Queue<Trash> deactivatedTrashObjectPooling = new Queue<Trash>();//posle ces da dodas object pooling... samo gledaj gde si koristio destroy
    public int maxTrash = 20;
    public static  float offset = 0.3f;

    public float referenceSpawnInterval;
    float minInterval = 0.55f;//posto su deca u pitanju trebalo bi da stavimo malo vecu brojku
    float maxInterval = 2.3f;
    public float actualSpawnInterval;
    float maxReferenceIntervalIncrease = 1f;//... - 100% max usporenje u odnosu na brzinu
    float maxReferenceIntervalDecrease = 1f;//... - 100% max ubrzavanje u odnosu na brzinu, 
    //ali to ce zavisiti od toga koliko je referenceInterval blizu Min interval, sto je blize to je manje povecanje
    float decayRate;
    float targetTimeToReachMin = 100f;//180f;
    float howCloseToConsiderReferenceReachedMin = 0.01f;
    public float averagePlayerInterval;//prosecan interval ubacivanja
    int numberForAverage = 5;
    public float[] lastPlayerIntervals;//OVO TREBA DA SE updatuje U PUT IN BIN, ali u inicijalizacije treba da mu se dodale pet vrednosti koje su maxInterval...
    float timerForSpawn = 0;
    float totalTime = 0;
    float currentPlayerInterval = 0f;
    //dodatne stvari da se uzmu u obzir:
    //deca su sporija, target group
    //ako vise elemenata ima to je teze
    //takodje na difficulty utice i koliko se brzo pune kante tj. koliko brzo se aktivira power up
    //mozda da se dodaje jos vise bodova sto je timer blizi targetTimeToReachMin
    //mozda ce lepse da izgleda i vise cartoony ako staklo bude plavo i plastika a ne transparentno... i bolje radi polygon collider tj. bolje radi klik na object
    //a ono sto ce neki itemi da donose vise poena moze da se odradi i da score bude float a da se prikazuje kao zaokruzeni int

    /*+dodatne stvari da se uzmu u obzir: 
    -deca su sporija, target group 
    -trenutno ima 5 itema, ako ih ima vise bice dosta teze
    -a ono sto ce neki itemi da donose vise poena moze da se odradi i da score bude broj sa decimalama(float) a da se prikazuje kao zaokruzeni celi broj(int), pa da imamo item koji dodaje npr. 1.5 poena, 1.7,  2    2.5
    -takodje na difficulty utice i koliko se brzo pune kante tj.koliko brzo se aktivira power up, trenutno je 5
   -mozda da se dodaje jos vise bodova sto je brzina spawna bliza maksimalnoj brzini*/


    float timerAddNewTypes = 0;
    float delayAddNewTypes = 20f;


    public float TimedExponentialDecay()
    {
        return minInterval + (maxInterval - minInterval) * Mathf.Exp(-decayRate * totalTime);
    }

    void Start()
    {
        Instance = this;
        offset = 0.3f;
        SpriteRenderer sr = JunkArea.GetComponent<SpriteRenderer>();
        Bounds bounds = sr.bounds;
        float junkAreaWidth = bounds.max.x - bounds.min.x;
        float screenWidth = BootMain.Instance.viewWidth;

        //da junkArea ne bude veca (sira) od ekrana
        float currentWidth = bounds.size.x;
        float viewWidth = BootMain.Instance.viewWidth;
        float targetWidth = viewWidth - offset * 2;//* 2 jer sa obe strane zelis da bude razmak
        float scaleMultiplier = targetWidth / currentWidth;
        JunkArea.transform.localScale *= scaleMultiplier;

        bounds = sr.bounds;
        minXJankArea = bounds.min.x + 1.5f * offset; //sve ovde ide * 2 jer zelis unutar fielda da se spawnuju stvari, da ne moze na ivicama
        minYJankArea = bounds.min.y + 1.5f * offset;
        maxXJankArea = bounds.max.x - 1.5f * offset;
        maxYJankArea = bounds.max.y - 1.5f * offset;

        BeginningSpawn();


        //spawning sistem

        actualSpawnInterval = maxInterval;
        decayRate = -Mathf.Log(howCloseToConsiderReferenceReachedMin) / targetTimeToReachMin; //znak minus je jer logaritam broja ispod 1 je negativan
        lastPlayerIntervals = new float[numberForAverage];
        for(int i =0; i < numberForAverage;i++)
        {
            lastPlayerIntervals[i] = maxInterval;
        }
        SetAveragePlayerInterval();
    }
    public void SetAveragePlayerInterval()
    {
        float total = 0f;
        foreach(float interval in lastPlayerIntervals)
        {
            total += interval;
        }
        averagePlayerInterval = total / (float) numberForAverage;
    }
    public void AddNewPlayerInterval()
    {
        int i;
        for (i = 0; i < lastPlayerIntervals.Length - 1; i++)
        {
            lastPlayerIntervals[i] = lastPlayerIntervals[i+1];
        }
        lastPlayerIntervals[i] = currentPlayerInterval;
        currentPlayerInterval = 0f;
    }
    public void BeginningSpawn()
    {
        //BOJAN: ovo je temp mozda? 10 je ovde magican broj, mada je dovoljno jasna skripta da mozda i moze da ostane ovako, ako se broj spawnovanih predmeta na pocetku igre ne menja nikad
        for (int i = 0; i < 10; i++)
        {
            ThrowTrash();
        }
        UpdateHealthBar();
    }
    void Update()
    {
        if (timerForSpawn > actualSpawnInterval)
        {
            referenceSpawnInterval = TimedExponentialDecay();
            SetAveragePlayerInterval();
            //float maxReferenceIntervalDecrease = 0.5f;//50%, moci ce da ubrza speed za 50% maks, ali to ce zavisiti od toga koliko je referenceInterval blizu Min interval, sto je blize to je manje povecanje
            float referenceIntervalProgress = Mathf.Clamp01(totalTime / targetTimeToReachMin);//od 0 do 1 koliko je totalTime blizu targetTime
            float remainingProgress = 1 - referenceIntervalProgress; // od 0 do 1 jos koliko je progresa ostalo
            float minClamp = 1f - remainingProgress * maxReferenceIntervalDecrease;
            float maxClamp = 1f + remainingProgress * maxReferenceIntervalIncrease;
            //ovo se mnozi sa maxReferenceIntervalDecrease ili increase i znaci da su efekti usporavanja i ubrzavanja sve manji sto se blizi minIntervalu, usporavanje postaje manje jer treba da bude teze, ubrzavanje 
            actualSpawnInterval = referenceSpawnInterval * Mathf.Clamp(averagePlayerInterval / referenceSpawnInterval, minClamp, maxClamp);

            timerForSpawn = 0;
            if (trashList.Count > 1)
            {
                ThrowTrash();
            }else
            {
                ThrowTrash();
                ThrowTrash();
                ThrowTrash();
            }
            
        }
        if (timerAddNewTypes > delayAddNewTypes)
        {
            timerAddNewTypes = 0;
            GameplayManager.AddNewTrashType();
            //AddNewType
        }
        timerAddNewTypes += Time.deltaTime;
        
        //BOJAN: je l' se racuna vreme i kad je pauzirana igra?
        totalTime += Time.deltaTime;
        timerForSpawn += Time.deltaTime;
        currentPlayerInterval += Time.deltaTime;
    }
    public void GameOver()
    {
        List<Trash> toRemove = new List<Trash>(trashList);
        foreach (Trash trash in toRemove)
        {
            DeactivateTrash(trash);
        }
        //restartuj spawnrate

        GameOverUIScreenActivate(true);
    }
    public void GameOverUIScreenActivate(bool active)
    {
        Time.timeScale = active ? 0f : 1f;
        BootGameplay.Instance.GameOverUI.SetActive(active);
        BootGameplay.Instance.PauseButton.SetActive(!active);

        /*if (!active)
        {
            BeginningSpawn();

        }*/
    }
    public void ThrowTrash()
    {
        //BOJAN: ovaj object pooling mi je malo cudan. generalno bi trebalo na initu da se instanciraju recimo 20 itema, i onda kasnije da ih vrtis, a ne da ih dodajes u toku runtime-a od 0? ili nisam uspeo da procitam kod dobro...
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

        newTrash.gameObject.transform.position = new Vector2(BootMain.Instance.viewRightX + 10f, 0);//da se ne bi videlo na screenu
        newTrash.Throw(GetRandomPositionInJunkArea());
        trashList.Add(newTrash);

        UpdateHealthBar();

        if (trashList.Count > maxTrash)//> a ne >= zato sto spavnuje trash van vidokruga i player uopste ne moze da reaguje 
        {
            GameOver();
        }
    }
    public Vector2 GetRandomPositionInJunkArea()
    {
        float randomX = UnityEngine.Random.Range(minXJankArea, maxXJankArea);
        float randomY = UnityEngine.Random.Range(minYJankArea, maxYJankArea);
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
        //BOJAN: ja bih ovo skratio na return trashList.Contains(trash)?trash:null; -> ali ovo je vise licna preferenca nego neka pametna stvar
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
            trash.ToggleRigidBody(false);
            trash.DeSelect();
            RemoveFromList(trash);

            deactivatedTrashObjectPooling.Enqueue(trash);
            trash.gameObject.SetActive(false);
            //Destroy(trash.gameObject);
        }
    }

    public void RemoveFromList(Trash trash)
    {
        if (trashList.Contains(trash))
        {
            trashList.Remove(trash);
            UpdateHealthBar();
        }
    }
    private void UpdateHealthBar()
    {
        float fill = (float)(maxTrash - trashList.Count) / maxTrash;
        BootGameplay.Instance.HealthBar.fillAmount = fill;
        BootGameplay.Instance.HealthBar.color = Color.Lerp(Color.red, Color.cyan, fill);
    }

    public int TrashCount()
    {
        return trashList.Count;
    }

    //BOJAN: ovo ne mora da se kompajluje, tako da sakrij ga iza #IF UNITY_EDITOR, a realno ne bi uopste trebalo da stoji u TrashManageru :P
    [ContextMenu("ExportSimulation")]
    void ExportSimulation()
    {
        SpawnIntervalSimulator simulator = new SpawnIntervalSimulator();
        string output = simulator.Simulate(100f, 0.7f,0.1f); 
        string folderPath = Application.dataPath + "/Export";
        string fileName = "100s_0,7s.txt";
        string fullPath = Path.Combine(folderPath, fileName);
        File.WriteAllText(fullPath, output);

        simulator = new SpawnIntervalSimulator();
        output = simulator.Simulate(100f, 2f, 0.1f);
        folderPath = Application.dataPath + "/Export";
        fileName = "100s_2s.txt";
        fullPath = Path.Combine(folderPath, fileName);
        File.WriteAllText(fullPath, output);
    }
    
}












public class SpawnIntervalSimulator
{
    float minInterval = 0.62f;
    float maxInterval = 2.3f;
    float referenceSpawnInterval;
    float actualSpawnInterval;
    float maxReferenceIntervalIncrease = 1f;//bolje je 2
    float maxReferenceIntervalDecrease = 1f;
    float decayRate;
    float targetTimeToReachMin = 60f;
    float totalTime = 0;
    float howCloseToConsiderReferenceReachedMin = 0.01f;


    float[] lastPlayerIntervals;
    int numberForAverage = 5;
    float currentPlayerInterval = 0f;

    float fixedPlayerInterval;
    System.Text.StringBuilder outputBuilder;

    public void Initialize(float fixedPlayerInterval)
    {
        this.fixedPlayerInterval = fixedPlayerInterval;

        referenceSpawnInterval = maxInterval;
        actualSpawnInterval = referenceSpawnInterval;

        decayRate = -Mathf.Log(howCloseToConsiderReferenceReachedMin) / targetTimeToReachMin;

        lastPlayerIntervals = new float[numberForAverage];
        for (int i = 0; i < numberForAverage; i++)
            lastPlayerIntervals[i] = maxInterval; // initial values

        outputBuilder = new System.Text.StringBuilder();
        outputBuilder.AppendLine("Time | RefInterval | ActualInterval | AvgPlayerInterval");
        outputBuilder.AppendLine("--------------------------------------------------------");
    }

    float TimedExponentialDecay()
    {
        //return minInterval + (maxInterval - minInterval) * Mathf.Exp(-decayRate * totalTime);

        return minInterval + (maxInterval - minInterval) * Mathf.Exp(-decayRate * totalTime);
    }

    float GetAveragePlayerInterval()
    {
        float sum = 0f;
        foreach (var val in lastPlayerIntervals)
            sum += val;
        return sum / lastPlayerIntervals.Length;
    }

    void UpdateIntervals(float deltaTime)
    {
        totalTime += deltaTime;
        currentPlayerInterval += deltaTime;

        // simulate player putting trash every X seconds
        if (currentPlayerInterval >= fixedPlayerInterval)
        {
            referenceSpawnInterval = TimedExponentialDecay();

            // shift array
            for (int i = 0; i < numberForAverage - 1; i++)
                lastPlayerIntervals[i] = lastPlayerIntervals[i + 1];
            lastPlayerIntervals[numberForAverage - 1] = currentPlayerInterval;

            float averagePlayerInterval = GetAveragePlayerInterval();
            float referenceIntervalProgress = Mathf.Clamp01(totalTime / targetTimeToReachMin);//od 0 do 1 koliko je totalTime blizu targetTime
            float remainingProgress = 1 - referenceIntervalProgress; // od 0 do 1 jos koliko je progresa ostalo
            float minClamp = 1f - remainingProgress * maxReferenceIntervalDecrease;
            float maxClamp = 1f + remainingProgress * maxReferenceIntervalIncrease;

            actualSpawnInterval = referenceSpawnInterval * Mathf.Clamp(averagePlayerInterval / referenceSpawnInterval, minClamp, maxClamp);

            outputBuilder.AppendLine($"{totalTime:F0}s | RefInterval: {referenceSpawnInterval:F3} | ActualInterval: {actualSpawnInterval:F3} | AvgPlayerInterval: {averagePlayerInterval:F3}");

            currentPlayerInterval = 0f;
        }
    }

    public string Simulate(float simulationDuration, float fixedPlayerInterval, float timestep)
    {
        Initialize(fixedPlayerInterval);

        float dt = timestep;
        while (totalTime < simulationDuration)
        {
            UpdateIntervals(dt);
        }

        return outputBuilder.ToString();
    }
}


