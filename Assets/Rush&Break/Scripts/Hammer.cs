using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Hammer : MonoBehaviour
{
    [SerializeField] private Transform hammerTransform = null;
    [SerializeField] private Transform colliderTransform = null;
    [Range(0, 1f)]
    [SerializeField] private float initialAngleToStart = 0;
    private static MainLevelManager levelManager = null;
    private bool isDeactivated = false;
    private int lastDownCount = 0;
    private void Awake()
    {
        if (!levelManager)
            levelManager = (MainLevelManager)LevelManager.Instance;
    }

    private void Update()
    {
        RotateHammer();
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

    private void RotateHammer()
    {
        int downCount = (int)((Time.time * 100 + initialAngleToStart * 100f) / 100f) / 2;
        bool toDown = (int)((Time.time * 100 + initialAngleToStart * 100f) / 100f) % 2 == 1;
        float z = Mathf.PingPong(Time.time * 100 + initialAngleToStart * 100f, 100);
        z = -(toDown ? (EaseOutQuart(z / 100) * 100) : z);
        if(lastDownCount < downCount)
        {
            ObjectPooler.Instance.SpawnFromPool("Hammer Effect", transform.position, transform.rotation);
            lastDownCount = downCount;
        }
        Vector3 desiredRot = hammerTransform.rotation.eulerAngles;
        desiredRot.z = z;
        hammerTransform.rotation = Quaternion.Euler(desiredRot);
    }

    private void CheckCollisions()
    {
        Collider[] colliders = Physics.OverlapBox(colliderTransform.position, new Vector3(1f, 2f, 1f), colliderTransform.rotation, levelManager.AgentLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Agent agent = colliders[i].GetComponent<Agent>();
            levelManager.Troop.RemoveAgent(agent, false,true);
        }
    }
    private float EaseOutQuart(float x)
    {
        return 1 - Mathf.Pow(1 - x, 4);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(colliderTransform.position, new Vector3(1f, 2f, 1f) * 2);
    }
}
