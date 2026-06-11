using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("UI Sliders (0 a 10)")]
    [SerializeField] private Slider sliderMaster;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSFX;

    private const string MIXER_MASTER = "MasterVol";
    private const string MIXER_MUSIC = "MusicVol";
    private const string MIXER_SFX = "SFXVol";

    void Start()
    {
        // Recupera o volume salvo anteriormente. Se for a primeira vez jogando, define como 8 (padrão)
        sliderMaster.value = PlayerPrefs.GetFloat(MIXER_MASTER, 8f);
        sliderMusic.value = PlayerPrefs.GetFloat(MIXER_MUSIC, 8f);
        sliderSFX.value = PlayerPrefs.GetFloat(MIXER_SFX, 8f);

        // Aplica os volumes iniciais no Mixer
        AtualizarVolumeMaster(sliderMaster.value);
        AtualizarVolumeMusic(sliderMusic.value);
        AtualizarVolumeSFX(sliderSFX.value);

        // Adiciona listeners via código para detectar quando o jogador clica para mudar as barrinhas
        sliderMaster.onValueChanged.AddListener(AtualizarVolumeMaster);
        sliderMusic.onValueChanged.AddListener(AtualizarVolumeMusic);
        sliderSFX.onValueChanged.AddListener(AtualizarVolumeSFX);
    }

    public void AtualizarVolumeMaster(float valorSlider)
    {
        DefinirVolumeMixer(MIXER_MASTER, valorSlider);
    }

    public void AtualizarVolumeMusic(float valorSlider)
    {
        DefinirVolumeMixer(MIXER_MUSIC, valorSlider);
    }

    public void AtualizarVolumeSFX(float valorSlider)
    {
        DefinirVolumeMixer(MIXER_SFX, valorSlider);
    }

    private void DefinirVolumeMixer(string nomeParametro, float valorSlider)
    {
        float volumeEmDecibeis;

        if (valorSlider <= 0) {
            // Se o slider for zero, muta completamente o canal (-80 decibéis)
            volumeEmDecibeis = -80f;
        } else {
            // Conversão matemática logarítmica de uma escala de 0-10 para Decibéis
            volumeEmDecibeis = Mathf.Log10(valorSlider / 10f) * 20f;
        }

        // Envia o valor calculado para o Audio Mixer
        audioMixer.SetFloat(nomeParametro, volumeEmDecibeis);

        // Salva a configuração localmente para persistir ao fechar o jogo
        PlayerPrefs.SetFloat(nomeParametro, valorSlider);
        PlayerPrefs.Save();
    }

    public void AumentarVolume(Slider sliderAlvo)
    {
        if (sliderAlvo != null && sliderAlvo.value < sliderAlvo.maxValue)
        {
            sliderAlvo.value += 1; // Aumenta 1 "gomo"
        }
    }

    public void DiminuirVolume(Slider sliderAlvo)
    {
        if (sliderAlvo != null && sliderAlvo.value > sliderAlvo.minValue)
        {
            sliderAlvo.value -= 1; // Diminui 1 "gomo"
        }
    }
}
