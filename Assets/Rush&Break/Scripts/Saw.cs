using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Saw : MonoBehaviour
{
    [SerializeField] private Transform sawTransform = null;
    [Range(0, 1f)]
    [SerializeField] private float initialDistanceToStart = 0;
    private static MainLevelManager levelManager = null;
    private Vector3 initialSawPosition;
    private bool isDeactivated = false;
    private void Awake()
    {
        if (!levelManager)
            levelManager = (MainLevelManager)LevelManager.Instance;
        initialSawPosition = sawTransform.position;
    }
    private void Update()
    {
        RotateSaw();
        Move();
        if (!isDeactivated && levelManager.Troop.Leader.transform.position.z - 4 > transform.position.z)
            Deactivate();
        if (!isDeactivated)
            CheckCollisions();
    }

    private void Move()
    {
        float x = Mathf.PingPong(Time.time * 4 + initialDistanceToStart * 10f, 10);
        x = EaseInOutCubic(x / 10) * 10;
        Vector3 desiredPos = initialSawPosition + transform.right * (x - 5);
        sawTransform.position = desiredPos;
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

    private void RotateSaw()
    {
        sawTransform.Rotate(Vector3.forward * Time.deltaTime * 360);
    }

    private void CheckCollisions()
    {
        Collider[] colliders = Physics.OverlapBox(sawTransform.transform.position, new Vector3(0.75f, 1, 0.25f), Quaternion.identity, levelManager.AgentLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Agent agent = colliders[i].GetComponent<Agent>();
            levelManager.Troop.RemoveAgent(agent, false,true);
        }
    }
    private float EaseInOutCubic(float x)
    {
        return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(sawTransform.transform.position, new Vector3(0.75f, 1, 0.25f) * 2);
    }
}
