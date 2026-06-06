using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip somClique;
    public AudioClip somVoltar;
    public AudioClip somIniciarJogo;
    public AudioClip somSairJogo;

    public void TocarSomClique() {

        if (somClique != null) {
            sfxSource.PlayOneShot(somClique);
        }
    }

    public void TocarSomVoltar() {

        if (somVoltar != null) {
            sfxSource.PlayOneShot(somVoltar);
        }
    }

    public void TocarSomIniciarJogo() {

        if (somIniciarJogo != null) {
            sfxSource.PlayOneShot(somIniciarJogo);
        }
    }

    public void TocarSomSairJogo() {

        if (somSairJogo != null) {
            sfxSource.PlayOneShot(somSairJogo);
        }
    }
}
