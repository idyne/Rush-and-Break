using System.Collections.Generic;
using UnityEngine;
using FateGames;
public class WallSet : MonoBehaviour
{
    private static MainLevelManager levelManager;
    [SerializeField] private int[] wallSizes = new int[4];
    private List<Wall> walls;
    private bool isPenetrated = false;
    public static int minSize = 999999;
    public static int maxSize = 1;

    public bool IsPenetrated { get => isPenetrated; }

    private void Awake()
    {
        if (!levelManager)
            levelManager = (MainLevelManager)LevelManager.Instance;
        walls = new List<Wall>();
        InstantiateWalls();
    }

    private void InstantiateWalls()
    {
        int wallSize;
        for (int i = 0; i < wallSizes.Length; i++)
        {
            wallSize = wallSizes[i];
            if (wallSize > 0)
            {
                if (wallSize < minSize)
                    minSize = wallSize;
                if (wallSize > maxSize)
                    maxSize = wallSize;
                Wall wall = Instantiate(levelManager.WallPrefab, transform.position + Vector3.right * (3 * i + 1.5f), Quaternion.identity).GetComponent<Wall>();
                wall.WallSet = this;
                wall.MaxSize = wallSize;
                walls.Add(wall);
            }
        }
    }

    public void Penetrate()
    {
        if (!isPenetrated)
        {
            isPenetrated = true;
            Wall wall;
            for (int i = 0; i < walls.Count; i++)
            {
                wall = walls[i];
                if (wall.gameObject.activeSelf)
                    wall.Deactivate();
            }
        }
    }
}
