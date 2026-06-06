using UnityEngine;

public enum TipoBonus { Dano, Velocidade, Vida, DefesaTemporaria }

[System.Serializable]
public class Item
{
    public string nome;
    public TipoBonus tipoBonus;
    public float valorBonus;
    public int preco;
    public Sprite icone;
}