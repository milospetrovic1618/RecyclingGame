using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class BootGameplay : MonoBehaviour
{

    public GameObject CameraPrefab;
    public GameObject EventSystemPrefab;
    public static BootGameplay Instance;

    public void Awake()
    {
        Instance = this;

        GameObject camObj = Instantiate(CameraPrefab, new Vector3(0, 0, -1), Quaternion.identity);
        Camera cam = camObj.GetComponent<Camera>();
        CreateWalls(cam);
        Instantiate(EventSystemPrefab, Vector3.zero, quaternion.identity);
        /*
        playerMovement = Instantiate(playerPrefab, Vector3.zero, quaternion.identity).GetComponent<PlayerMovement>();
        Instantiate(townPrefab, Vector3.zero, quaternion.identity);
        Instantiate(EventSystemPrefab, Vector3.zero, quaternion.identity);
        Instantiate(Grass, Vector3.zero, quaternion.identity);
        login = Instantiate(Login, Vector3.zero, quaternion.identity).GetComponent<Login>();
        Instantiate(Put, new Vector3(1, -1.3f, 0), quaternion.identity);
        Instantiate(Ambient, Vector3.zero, quaternion.identity);*/
    }

    public float wallThickness = 1f;
    public float zPosition = 0f;
    public void CreateWalls(Camera cam)
    {
        Vector2 screenBottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector2 screenTopRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        float width = screenTopRight.x - screenBottomLeft.x;
        float height = screenTopRight.y - screenBottomLeft.y;

        CreateWall(new Vector2(0, screenTopRight.y + wallThickness / 2), new Vector2(width, wallThickness)); // Top
        CreateWall(new Vector2(0, screenBottomLeft.y - wallThickness / 2), new Vector2(width, wallThickness)); // Bottom
        CreateWall(new Vector2(screenBottomLeft.x - wallThickness / 2, 0), new Vector2(wallThickness, height)); // Left
        CreateWall(new Vector2(screenTopRight.x + wallThickness / 2, 0), new Vector2(wallThickness, height)); // Right
    }
    void CreateWall(Vector2 position, Vector2 size)
    {
        GameObject wall = new GameObject("Wall");
        wall.transform.position = new Vector3(position.x, position.y, zPosition);

        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.size = size;

        wall.layer = LayerMask.NameToLayer("Default");
    }
}
