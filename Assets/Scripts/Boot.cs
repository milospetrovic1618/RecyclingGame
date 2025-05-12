using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    public static Boot Instance;
    public void Awake()
    {
        Instance = this;
        SceneManager.LoadScene(Scenes.Gameplay.ToString());
    }
}
