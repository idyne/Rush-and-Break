using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    [SerializeField] private GameObject roadPartPrefab = null;
    [SerializeField] private GameObject bonusPrefab = null;

    public void CreateRoad(int numberOfParts)
    {
        for (int i = 0; i < numberOfParts; i++)
            Instantiate(roadPartPrefab, transform.position + Vector3.forward * i * 11, Quaternion.identity, transform);
        Instantiate(bonusPrefab, transform.position + Vector3.forward * numberOfParts * 11, Quaternion.identity, transform);
        //CombineMesh();
        AdjustBoxCollider(numberOfParts);
    }

    private void AdjustBoxCollider(int numberOfParts)
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        Vector3 desiredSize = boxCollider.size;
        desiredSize.z = numberOfParts * 11;
        boxCollider.size = desiredSize;
        Vector3 desiredCenter = boxCollider.center;
        desiredCenter.z = desiredSize.z / 2f;
        boxCollider.center = desiredCenter;
    }

    private void CombineMesh()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }
}
