using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float suavidade = 5f;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 destino = new Vector3(
            player.position.x,
            player.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            destino,
            suavidade * Time.deltaTime
        );
    }
}