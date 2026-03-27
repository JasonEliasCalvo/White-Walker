using System.Collections.Generic;
using UnityEngine;

public class TargetLock : MonoBehaviour
{
    [Header("Configuración")]
    public float detectionRadius = 15f; // Qué tan lejos detecta enemigos
    public LayerMask enemyLayer;       // Asegúrate de que tus enemigos estén en una Layer llamada "Enemy"

    private List<Transform> availableTargets = new List<Transform>();

    public void ToggleLock()
    {
        Debug.Log("ToggleLock called");
        // Si ya tenemos un enemigo, lo soltamos
        if (CameraManager.instance.currentEnemy != null)
        {
            CleanLock();
            return;
        }

        // Si no, buscamos uno nuevo
        FindTarget();
    }

    private void FindTarget()
    {
        availableTargets.Clear();

        // 1. Lanzamos una "burbuja" para ver quién está cerca
        Collider[] cols = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        if (cols.Length == 0) return;

        Transform bestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var col in cols)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                bestTarget = col.transform;
            }
        }

        if (bestTarget != null)
        {
            CameraManager.instance.currentEnemy = bestTarget;
            CameraManager.instance.SwitchCameraStyle(CameraManager.CameraStyle.Combat);
        }
    }

    public void CleanLock()
    {
        CameraManager.instance.currentEnemy = null;
        CameraManager.instance.SwitchCameraStyle(CameraManager.CameraStyle.Basic);
    }
}