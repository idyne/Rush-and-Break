using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Rasp : MonoBehaviour
{
    [SerializeField] private Transform raspTransform = null;
    [Range(0, 1f)]
    private static MainLevelManager levelManager = null;
    private bool isDeactivated = false;
    private void Awake()
    {
        if (!levelManager)
            levelManager = (MainLevelManager)LevelManager.Instance;
    }

    private void Update()
    {
        RotateRasp();
        if (!isDeactivated && levelManager.Troop.Leader.transform.position.z - 4 > transform.position.z)
            Deactivate();
        if (!isDeactivated)
            CheckCollisions();
    }

    private void Deactivate()
    {
        isDeactivated = true;
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

    private void RotateRasp()
    {
        raspTransform.Rotate(Vector3.up * Time.deltaTime * 360);
    }

    private void CheckCollisions()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(1f, 2f, 1f), Quaternion.identity, levelManager.AgentLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Agent agent = colliders[i].GetComponent<Agent>();
            levelManager.Troop.RemoveAgent(agent, false, true);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.up * 1.1f, new Vector3(1.5f,2.2f,1.5f));
    }
}
