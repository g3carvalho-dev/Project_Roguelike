using UnityEngine;

public enum TipoBonusReliquia { Dano, Velocidade, VidaMaxima, Defesa }

[System.Serializable]
public class Reliquia
{
    public string nome;
    public string descricao;
    public TipoBonusReliquia tipo;
    public float valor;
    public Sprite icone;
}