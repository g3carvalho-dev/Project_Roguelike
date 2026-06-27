using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocidade = 5f;

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.jogoPausado) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector3 direcao = new Vector3(x, y, 0).normalized;
        transform.position += direcao * velocidade * Time.deltaTime;
    }
}