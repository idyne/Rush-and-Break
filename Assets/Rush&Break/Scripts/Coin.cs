using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using MoreMountains.NiceVibrations;

public class Coin : MonoBehaviour
{
    private static MainLevelManager levelManager = null;
    [SerializeField] private Transform meshTransform = null;
    private void Awake()
    {
        if (!levelManager)
            levelManager = (MainLevelManager)LevelManager.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        if (tag == "Agent")
        {
            GetComponent<Collider>().enabled = false;
            meshTransform.LeanScale(Vector3.zero, 0.1f);
            ObjectPooler.Instance.SpawnFromPool("Coin Collect Effect", transform.position, Quaternion.identity);
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
            levelManager.Coin++;
        }
    }
}
