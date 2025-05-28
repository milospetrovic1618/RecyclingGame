using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class SaveSystem : PersistentSingleton<SaveSystem>
{
    public JsonSerializerSettings JSet;
    public PlayerSave Player;

    private List<string> ChangedSaves;
    private Coroutine QueuedSave;

    private Dictionary<string, GenericSave> Saves;

    protected override void OnInitialize()
    {
        JSet = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        
        Saves = new Dictionary<string, GenericSave>();
        ChangedSaves = new List<string>();
        
        Player = (PlayerSave)Load(new PlayerSave());
        Saves.Add(Player.Name, Player);
    }

    public GenericSave Load(GenericSave save)
    {
        if (PlayerPrefs.HasKey(save.Name))
        {
            return JsonConvert.DeserializeObject<GenericSave>(PlayerPrefs.GetString(save.Name), JSet);
        }
        
        return save;
    }

    public void Flag(GenericSave save)
    {
        if (ChangedSaves.Contains(save.Name))
        {
            return;
        }
        
        ChangedSaves.Add(save.Name);
        if (QueuedSave == null)
        {
            QueuedSave = StartCoroutine(DelayedSave());
        }
    }

    private IEnumerator DelayedSave()
    {
        yield return new WaitForEndOfFrame();
        foreach (string save in ChangedSaves)
        {
            Saves[save].Save();
        }
        ChangedSaves.Clear();
        PlayerPrefs.Save();
        
        QueuedSave = null;
    }






    public void ExportPlayerPrefsToFile(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            Debug.LogWarning("Key  not found in PlayerPrefs");
            return;
        }

        string json = PlayerPrefs.GetString(key);
        string folderPath = Path.Combine(Application.dataPath, "Export");

        string filePath = Path.Combine(folderPath, $"{key}.txt");
        File.WriteAllText(filePath, json);
    }
    public GenericSave LoadFromExportFile(string fileName)
    {
        string filePath = Path.Combine(Application.dataPath, "Export", $"{fileName}.txt");

        string json = File.ReadAllText(filePath);
        GenericSave loadedData = JsonConvert.DeserializeObject<GenericSave>(json, JSet);

        Debug.Log("Loaded save");
        return loadedData;
    }
    [ContextMenu("ExportPlayer")]
    public void ExportPlayer()
    {
        ExportPlayerPrefsToFile(new PlayerSave().Name);
    }
    [ContextMenu("LoadPlayerTest")]
    public void LoadPlayerTest()
    {
        Player = (PlayerSave)LoadFromExportFile(new PlayerSave().Name);
    }
    [ContextMenu("ClearPrefs")]
    public void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}

public abstract class GenericSave
{
    public string Name;
    
    public GenericSave()
    {
        Name = this.GetType().ToString();
    }
    
    public void Save()
    {
        PlayerPrefs.SetString(Name, JsonConvert.SerializeObject(this, SaveSystem.Instance.JSet));
    }
}