using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuntajeJugador : MonoBehaviour
{
    public Text puntajeUI;

    private int puntaje;

    public void aumentarPuntaje(int puntos)
    {
        puntaje += puntos;

        puntajeUI.text = puntaje.ToString();
    }
}
