using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    public void VoltarAoMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ResetarJogo();

        SceneManager.LoadScene("MenuPrincipal");
    }
}