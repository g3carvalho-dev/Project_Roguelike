using UnityEngine;

public class Porta : MonoBehaviour
{
    public bool bloqueada = true;
    private SpriteRenderer sr;
    public BoxCollider2D colliderSolido;
    public BoxCollider2D colliderTrigger;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (GameManager.Instance != null && GameManager.Instance.salaAtual == 1)
            bloqueada = false;

        AtualizarVisual();
    }

    public void Desbloquear()
    {
        bloqueada = false;
        AtualizarVisual();
        Debug.Log("Porta desbloqueada!");
    }

    public GameObject paredePorta;

    void AtualizarVisual()
    {
        if (sr != null)
            sr.color = bloqueada ? Color.red : Color.green;

        if (paredePorta != null)
            paredePorta.SetActive(bloqueada);
    }

    public void Resetar()
    {
        bloqueada = true;
        AtualizarVisual();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !bloqueada)
        {
            Debug.Log("Proxima sala!");
            GameManager.Instance.AvancarParaProximaSala();
        }
    }
}