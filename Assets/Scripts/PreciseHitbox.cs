using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PreciseHitbox : MonoBehaviour
{
    [Tooltip("Define a opacidade mínima para registrar o clique. 0.1 significa 10% visível.")]
    [Range(0f, 1f)]
    public float alphaThreshold = 0.1f;

    void Start()
    {
        // Pega o componente de imagem atrelado a este mesmo GameObject
        Image buttonImage = GetComponent<Image>();

        // Define o limite de transparência
        buttonImage.alphaHitTestMinimumThreshold = alphaThreshold;
    }
}