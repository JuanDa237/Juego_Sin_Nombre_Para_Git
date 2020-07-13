using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextoDaño : MonoBehaviour
{
    private TextMesh texto;

    void Awake() 
    {
        texto = GetComponent<TextMesh>();    
    }

    public void setTexto(int daño)
    {
        texto.text = "-" + daño;
    }

    public void destruir()
    {
        Destroy(gameObject);
    }
}
