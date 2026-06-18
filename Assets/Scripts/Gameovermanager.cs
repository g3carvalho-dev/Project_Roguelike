using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void Reviver()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetarJogo();
            GameManager.Instance.VoltarParaCheckpoint();
        }
        else
            SceneManager.LoadScene("SampleScene");
    }

    public void Desistir()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ResetarJogo();

        SceneManager.LoadScene("MenuPrincipal");
    }
}