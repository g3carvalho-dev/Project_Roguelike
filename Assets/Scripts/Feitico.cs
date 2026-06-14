using UnityEngine;

public enum TipoFeitico { BolaDeFogo, CuraInstantanea, Onda }

[System.Serializable]
public class Feitico
{
    public string nome;
    public TipoFeitico tipo;
    public float dano;
    public float cooldown;
    public GameObject prefabEfeito;
}