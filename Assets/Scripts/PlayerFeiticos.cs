using UnityEngine;

public class PlayerFeiticos : MonoBehaviour
{
    public Feitico feitico1;
    public Feitico feitico2;

    private float timerFeitico1;
    private float timerFeitico2;

    bool TemFeitico(Feitico f) => f != null && !string.IsNullOrEmpty(f.nome);

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.jogoPausado) return;

        timerFeitico1 -= Time.deltaTime;
        timerFeitico2 -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q) && TemFeitico(feitico1) && timerFeitico1 <= 0)
        {
            UsarFeitico(feitico1);
            timerFeitico1 = feitico1.cooldown;
        }

        if (Input.GetKeyDown(KeyCode.E) && TemFeitico(feitico2) && timerFeitico2 <= 0)
        {
            UsarFeitico(feitico2);
            timerFeitico2 = feitico2.cooldown;
        }
    }

    void UsarFeitico(Feitico feitico)
    {
        Debug.Log("Usou feitico: " + feitico.nome);

        switch (feitico.tipo)
        {
            case TipoFeitico.BolaDeFogo:
                AtirarBolaDeFogo(feitico);
                break;

            case TipoFeitico.CuraInstantanea:
                Curar(feitico);
                break;

            case TipoFeitico.Onda:
                OndaDeChoque(feitico);
                break;
        }
    }

    void AtirarBolaDeFogo(Feitico feitico)
    {
        Vector3 mouseMundo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseMundo.z = 0;
        Vector2 direcao = (mouseMundo - transform.position).normalized;

        if (feitico.prefabEfeito != null)
        {
            GameObject efeito = Instantiate(feitico.prefabEfeito, transform.position, Quaternion.identity);
            Projetil proj = efeito.GetComponent<Projetil>();
            if (proj != null)
                proj.Iniciar(direcao);
        }
    }

    void Curar(Feitico feitico)
    {
        PlayerStats stats = GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.coracoesAtuais = Mathf.Min(stats.coracoesAtuais + 1, stats.coracoesMaximos);
            stats.onVidaAtualizada?.Invoke();
            Debug.Log("Curou 1 coracao!");
        }
    }

    void OndaDeChoque(Feitico feitico)
    {
        Collider2D[] atingidos = Physics2D.OverlapCircleAll(transform.position, 3f);

        foreach (Collider2D col in atingidos)
        {
            if (col.CompareTag("Inimigo"))
            {
                EnemyStats stats = col.GetComponent<EnemyStats>();
                if (stats != null)
                    stats.ReceberDano(feitico.dano);
            }
        }
    }

    public void DesbloquearFeitico(Feitico novoFeitico)
    {
        if (!TemFeitico(feitico1))
        {
            feitico1 = novoFeitico;
            Debug.Log("Feitico 1 desbloqueado: " + novoFeitico.nome);
        }
        else if (!TemFeitico(feitico2))
        {
            feitico2 = novoFeitico;
            Debug.Log("Feitico 2 desbloqueado: " + novoFeitico.nome);
        }
        else
        {
            Debug.Log("Ja tem 2 feiticos equipados!");
        }
    }
}