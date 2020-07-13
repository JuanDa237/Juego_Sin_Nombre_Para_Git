using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CargarPuntajeRecord : MonoBehaviour
{
    public TextMeshProUGUI puntajeTexto;

    void Awake() 
    {
        puntajeTexto.text = PlayerPrefs.GetInt("puntajeRecord", 0).ToString();
    }
}
