using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 8;
    [SerializeField] private float horizontalSpeed = 4;
    private MainLevelManager levelManager = null;

    private void Awake()
    {
        levelManager = (MainLevelManager)LevelManager.Instance;
    }
    private void Update()
    {
        CheckInput();
        MoveForward();
    }

    private void CheckInput()
    {
        MoveHorizontally(Input.GetAxis("Horizontal"));
    }

    private void MoveHorizontally(float rate)
    {
        Vector3 desiredPos = transform.position + transform.right * rate * Time.deltaTime * horizontalSpeed;
        transform.position = desiredPos;
    }

    private void MoveForward()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, Vector3.one / 4, Quaternion.identity, levelManager.WallLayerMask);
        if (colliders.Length == 0 || true)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.forward, Time.deltaTime * speed);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one / 2);
    }
}
