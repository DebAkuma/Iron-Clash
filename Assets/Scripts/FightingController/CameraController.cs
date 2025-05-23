using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform[] targets;
    [SerializeField] private float smoothSpeed = 0.2f;
    [SerializeField] private Vector3 offset;

    void LateUpdate()
    {
        if (targets == null || targets.Length == 0)
        {
            FindPlayer();
            if (targets == null || targets.Length == 0)
                return; // No players found yet
        }

        Transform activeTarget = FindActiveTarget();
        if (activeTarget == null) return;

        Vector3 desiredPosition = activeTarget.position + offset;
        desiredPosition.y = transform.position.y;

        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothPosition;
    }

    Transform FindActiveTarget()
    {
        foreach (Transform target in targets)
        {
            if (target.gameObject.activeInHierarchy) return target;
        }
        return null;
    }


    public void FindPlayer()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        targets = new Transform[playerObjects.Length]; 

        for (int i = 0; i < playerObjects.Length; i++)
        {
            targets[i] = playerObjects[i].transform;
        }
    }
}
