using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class BlockTrap : MonoBehaviour
{
    private static MainLevelManager levelManager = null;
    private bool isDeactivated = false;

    private void Awake()
    {
        if (!levelManager)
            levelManager = (MainLevelManager)LevelManager.Instance;
    }

    private void Update()
    {
        if (!isDeactivated && levelManager.Troop.Leader.transform.position.z - 3 > transform.position.z)
            Deactivate();
    }

    private void Deactivate()
    {
        isDeactivated = true;
        GetComponent<Collider>().enabled = false;
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        float alpha = 0.6f;
        float time = 0.1f;
        for (int i = 0; i < renderers.Length; i++)
        {
            MeshRenderer rend = renderers[i];
            for (int j = 0; j < rend.materials.Length; j++)
                ColorManager.DoAlphaTransition(rend, j, alpha, time);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        if (tag == "Agent")
        {
            Agent agent = other.GetComponent<Agent>();
            levelManager.Troop.RemoveAgent(agent, false,true);
        }
    }
}
