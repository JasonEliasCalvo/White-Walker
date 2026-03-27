using UnityEngine;

public class CombatCameraController : MonoBehaviour
{
    [Header("Settings")]
    public float followSmooth = 5f;
    public float minDistance = 4f; // Distancia mínima de la cámara
    public float heightOffset = 1.5f; // Qué tan arriba está la cámara

    private void LateUpdate()
    {
        Transform player = CameraManager.instance.playerTransform;
        Transform enemy = CameraManager.instance.currentEnemy;

        if (enemy == null) return;

        // 1. Calcular el punto donde miramos (Anchor)
        Vector3 anchor = (player.position + enemy.position) / 2f;
        anchor.y += heightOffset; // Elevamos un poco la mirada para ver mejor

        // 2. Calcular la dirección (Desde el enemigo hacia el jugador)
        // Esto hace que la cámara intente ponerse detrás del jugador siempre
        Vector3 dirToPlayer = (player.position - enemy.position).normalized;

        // 3. Calcular la distancia ideal
        float distBetween = Vector3.Distance(player.position, enemy.position);
        float cameraDist = Mathf.Max(minDistance, distBetween * 1.2f);

        // 4. Posición final de la cámara
        Vector3 targetPos = anchor + dirToPlayer * cameraDist;
        targetPos.y += 1f; // Un pequeño ajuste de altura extra

        // 5. Mover suavemente (Suavizado físico)
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSmooth);

        // 6. Mirar siempre al punto medio
        transform.LookAt(anchor);
    }
}
