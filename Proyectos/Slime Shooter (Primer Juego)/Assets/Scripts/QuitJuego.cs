using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitJuego : MonoBehaviour
{
    public void Cerrar()
    {
        Application.Quit();
        Debug.Log("Ya Salio");
    }
}
